using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Basic.Network.Message;
using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EL.Robot.Core
{
    public class WChannel
    {
        public Guid Id { get; set; }
        private IPEndPoint ipep { get; set; }
        public string SocketUrl { get; set; }
        public WChannel(IPEndPoint iPEndPoint)
        {
            ipep = iPEndPoint;
            this.Id = Guid.NewGuid();

        }
        public WChannel(string url)
        {
            SocketUrl = url;
            this.Id = Guid.NewGuid();

        }
        public ClientWebSocket WebSocket { get; set; }
        public static int RpcId = 0;
        public readonly struct RpcInfo
        {
            public readonly Utils.IRequest Request;
            public readonly ELTask<Utils.IResponse> Tcs;

            public RpcInfo(Utils.IRequest request)
            {
                this.Request = request;
                this.Tcs = ELTask<Utils.IResponse>.Create(true);
            }
        }
        private readonly Dictionary<int, RpcInfo> requestCallbacks = new Dictionary<int, RpcInfo>();
        public async ELTask<Utils.IResponse> Call(Utils.IRequest request)
        {
            await Task.CompletedTask;
            if (WebSocket == null || WebSocket.State != WebSocketState.Open)
            {
                if (ipep != null)
                {
                    await ConnectAsync($"ws://{ipep.Address}:{ipep.Port}");
                }
                else
                {
                    await ConnectAsync(SocketUrl);
                }
                //await ConnectAsync(url);
            }
            RpcInfo rpcInfo = new RpcInfo(request);
            //await this.Send(request);
            try
            {
                int rpcId = ++RpcId;
                requestCallbacks[rpcId] = rpcInfo;
                request.RpcId = rpcId;
                var code = Utils.OpcodeTypeComponent.GetOpcode(request.GetType());
                var msg = Utils.OpcodeTypeComponent.GetOpcode(request.GetType()) + JsonHelper.ToJson(request);
                var bytes = Encoding.UTF8.GetBytes(msg);
                await SendAsync(bytes);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return default;
            }
            var response = await rpcInfo.Tcs;
            return response;
        }
        private readonly CircularBuffer sendBuffer = new CircularBuffer();
        private readonly byte[] sendCache = new byte[Packet.OpcodeLength + Packet.ActorIdLength];
        private Queue<byte[]> sendQueues = new Queue<byte[]>();
        public async ELTask Send(object message)
        {
            await ELTask.CompletedTask;
            var property = message.GetType().GetProperty("RpcId");
            int.TryParse(property.GetValue(message, null).ToString(), out int rpc);
            if (rpc < 1)
                property.SetValue(message, ++RpcId, null);
            var code = Utils.OpcodeTypeComponent.GetOpcode(message.GetType());
            var msg = Utils.OpcodeTypeComponent.GetOpcode(message.GetType()) + JsonHelper.ToJson(message);
            //if (code == (ushort)OuterOpcode.ClipboardResponse ||
            //    code == (ushort)OuterOpcode.ClipboardRequest)
            //{
            //    var clipboard = Boot.GetComponent<ClipboardComponent>();
            //    clipboard.CopyToClipboard(msg);
            //    return;
            //}
            var bytes = Encoding.UTF8.GetBytes(msg);
            //(ushort opcode, MemoryStream stream) = MessageSerializeHelper.MessageToStream(message);
            sendQueues.Enqueue(bytes);
            await ELTask.CompletedTask;
        }
        public async ELTask SendAsync(MemoryStream stream)
        {
            await ELTask.CompletedTask;
            if (WebSocket.State != WebSocketState.Open)
            {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }

            var array = new ArraySegment<byte>(stream.GetBuffer());
            await WebSocket.SendAsync(array, WebSocketMessageType.Text, true, CancellationToken.None).ContinueWith((x) =>
           {
           });
        }
        public async ELTask SendAsync(byte[] bytes)
        {
            await ELTask.CompletedTask;
            if (WebSocket.State != WebSocketState.Open)
            {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
            //if (bytes.Length>1024*100)
            //{
            //    byte[] buffer = default;
            //    using (var file = File.OpenRead(@"C:\Users\panyun.li\Desktop\test111.png"))
            //    {
            //        buffer = new byte[file.Length];
            //        file.Read(buffer, 0, buffer.Length);
            //    }
            //    var array1 = new ArraySegment<byte>(buffer);
            //    await WebSocket.SendAsync(array1, WebSocketMessageType.Binary, true, CancellationToken.None).ContinueWith((x) =>
            //    {
            //    });
            //    return;
            //}

            sendQueues.Enqueue(bytes);
        }
        List<byte> bytes = new List<byte>();
        public async ELTask ConnectAsync(string url = "ws://192.168.0.99:10001")
        {
            WebSocket = new ClientWebSocket()
            {
                Options =
                {
                     KeepAliveInterval =TimeSpan.FromMinutes(60),
                     Proxy = null
                 }
            };
            byte[] data = new byte[1024 * 1024];
            var temp = new Uri(url);
            await WebSocket.ConnectAsync(temp, CancellationToken.None);
            _ = Task.Run(async () =>
            {
                while (WebSocket.State == WebSocketState.Open)
                {
                    while (sendQueues.Any())
                    {
                        var data = sendQueues.Dequeue();
                        await WebSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    SpinWait.SpinUntil(() => sendQueues?.Count > 0, 1000);
                }
            });
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(data), CancellationToken.None);
                    if (result.MessageType != WebSocketMessageType.Close)
                    {
                        bytes.AddRange(data.Take(result.Count));
                        if (result.EndOfMessage)
                        {
                            var data = bytes.ToArray();
                            _ = Task.Run(() =>
                            {
                                try
                                {
                                    string para = Encoding.UTF8.GetString(data);
                                    var opcode = ushort.Parse(para.Substring(0, 5));
                                    var msg = para.Substring(5, para.Length - 5);
                                    var type = Utils.OpcodeTypeComponent.GetType(opcode);
                                    var message = JsonHelper.FromJson(type, para.Substring(5, para.Length - 5));
                                    OnRead(opcode, msg);
                                }
                                catch (Exception ex)
                                {

                                }
                            });
                            bytes.Clear();
                        }
                    }
                }
            });
        }
        private void OnRead(ushort opcode, string json)
        {
            RoboatServerComponent.LastRecvTime = TimeHelper.ServerNow();
            Type type = Utils.OpcodeTypeComponent.GetType(opcode);
            object message = MessageSerializeHelper.DeserializeFrom(opcode, type, json);
            if (message is Utils.IResponse response)
            {
                if (!this.requestCallbacks.TryGetValue(response.RpcId, out var action))
                {
                    return;
                }
                this.requestCallbacks.Remove(response.RpcId);
                action.Tcs.SetResult(response);
                return;
            }
            Handle(opcode, message, this).Coroutine();
        }
        public static async ELVoid Handle(ushort opcode, object message, WChannel channel)
        {
            List<W_IMHandler> actions;
            try
            {
                if (!MessageHandlerComponent.W_Handlers.TryGetValue(opcode, out actions))
                {
                    //MessageBox.Show($"消息没有处理: {opcode} {message}");
                    return;
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            foreach (W_IMHandler ev in actions)
            {
                try
                {
                    ev.Handle(channel, message);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        public void Dispose()
        {
            if (WebSocket != null)
                this.WebSocket.Dispose();
            sendQueues?.Clear();
            this.Id = Guid.Empty;
            RoboatServerComponent.cts.Cancel();
        }
    }
}
