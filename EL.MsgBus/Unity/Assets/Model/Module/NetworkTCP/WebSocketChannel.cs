using MongoDB.Driver;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ET
{
    /// <summary>
    /// websocket 管道
    /// </summary>
    public class WebSocketChannel : AChannel
    {
        private const int CacheLength = 1024 * 10;//10kb
        private readonly TService Service;
        public bool IsWebSocket { get; set; } = true;
        public bool IsClientConnect { get; set; }

        private WebSocket webSocket = null;
        private readonly BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>();
        public WebSocketChannel(long id, IPEndPoint ipEndPoint, WebSocket webSocket, TService service)
        {
            IsClientConnect = true;
            this.ChannelType = ChannelType.Accept;
            this.Id = id;
            this.Service = service;
            this.RemoteAddress = ipEndPoint;
            this.webSocket = webSocket;
            this.Service.ThreadSynchronizationContext.PostNext(() =>
            {
                Console.WriteLine($"新增id:{this.Id} 启动任务! ");
                _ = this.StartRecv();
                this.StartSend();
            });
        }
        public WebSocketChannel(long id, IPEndPoint ipEndPoint, TService service)
        {
            this.IsClientConnect = false;
            this.IsWebSocket = true;
            this.ChannelType = ChannelType.Connect;
            this.Id = id;
            this.Service = service;
            this.RemoteAddress = ipEndPoint;
            this.Service.ThreadSynchronizationContext.PostNext(() =>
            {
                _ = ConnectAsync();
            });
        }
        private async Task ConnectAsync()
        {
            var ClientWebSocket = new ClientWebSocket();
            var url = $"ws://{this.RemoteAddress.Address}:{this.RemoteAddress.Port}";
            await ClientWebSocket.ConnectAsync(new Uri(url), CancellationToken.None);
            webSocket = ClientWebSocket;
            await this.StartRecv();
            this.StartSend();
        }
        private async Task StartRecv()
        {
            if (this.webSocket == null)
            {
                return;
            }

            var buffer = ArrayPool<byte>.Shared.Rent(CacheLength);
            var bufferData = new List<byte>();
            try
            {
                if (IsWebSocket)
                {
                    while (webSocket != null && webSocket.State == WebSocketState.Open)
                    {
                        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine($"客户端{this.Id} 主动断开连接!");
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                            break;
                        }
                        bufferData.AddRange(buffer.Take(result.Count).ToArray());
                        if (result.EndOfMessage)
                        {
                            if (bufferData.Count > 10000)
                            {
                                Log.Debug($"接收到数据长度 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 长度: {bufferData.Count}");
                            }
                            OnRead(bufferData.ToArray());
                            bufferData.Clear();
                        }
                    }
                    Log.Info("websocket断开连接");
                }
                else
                {
                    throw new Exception("websocket 握手失败!");
                }
            }
            catch (Exception e)
            {
                var msg = e.Message;
                if (msg.Contains("The remote party closed the WebSocket connection without completing the close handshake"))
                {
                    msg = "连接主动断开!";
                    Log.Warning($"{this.RemoteAddress} 接收异常:WebsocketState: {webSocket.State} ex： {msg}");
                }
                else
                {
                    Log.Error(e);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
                bufferData.Clear();
                this.OnError(ErrorCode.ERR_WebsocketConnectError);
            }
        }
        public void StartSend()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        if (this.IsDisposed)
                        {
                            return;
                        }
                        var bytes = this.queue.Take();
                        await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                        if (bytes.Length > 10000)
                        {
                            Console.WriteLine($"正常发送 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 长度:{bytes.Length}");
                        }
                        else
                        {
                            Console.WriteLine($"正常发送 长度:{bytes.Length}");
                        }
                    }
                }
                catch (Exception e)
                {
                    this.OnError(ErrorCode.ERR_WebsocketSendError);
                    var msg = e.Message;
                    if (msg.Contains("The remote party closed the WebSocket connection without completing the close handshake"))
                    {
                        msg = "连接主动断开!";
                        Log.Warning($"{this.RemoteAddress} 接收异常:WebsocketState: {webSocket.State} ex： {msg}");
                    }
                    else
                    {
                        Log.Error(e);
                    }
                }
            });
        }
        private void OnRead(MemoryStream memoryStream)
        {
            try
            {
                long channelId = this.Id;
                this.Service.OnRead(channelId, memoryStream);
            }
            catch (Exception e)
            {
                Log.Error($"{this.RemoteAddress} {memoryStream.Length} {e}");
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                this.OnError(ErrorCode.ERR_PacketParserError);
            }
        }
        private void OnRead(byte[] byteMsg)
        {
            var memoryStream = MessageSerializeHelper.GetStream(byteMsg.Length);
            memoryStream.Write(byteMsg, 0, byteMsg.Length);
            OnRead(memoryStream);
        }
        public void Send(long actorId, MemoryStream stream)
        {
            if (this.IsDisposed)
            {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
            if (IsWebSocket && webSocket?.State == WebSocketState.Open)
            {
                var opcode = BitConverter.ToUInt16(stream.GetBuffer(), Packet.KcpOpcodeIndex);
                var type = OpcodeTypeComponent.Instance.GetType(opcode);
                var msg = stream.GetBuffer().ToStr(2, (int)(stream.Length - 2));
                var sendMsg = opcode + msg;
                byte[] temp = Encoding.UTF8.GetBytes(sendMsg);
                queue.Add(temp);
                //webSocket.SendAsync(temp, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Log.Error("连接还未创建，无法发送!");
            }
        }
        public void Update()
        {

        }
        private void OnError(int error)
        {
            Log.Info($"WebSocketChannel OnError: {error} {this.RemoteAddress}");

            long channelId = this.Id;

            this.Service.Remove(channelId);

            this.Service.OnError(channelId, error);
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            Log.Info($"channel dispose: {this.Id} {this.RemoteAddress}");

            long id = this.Id;
            this.Id = 0;
            this.Service.Remove(id);
            try
            {
                Log.Info($"服务端 Dispose :{this.Id} 中断服务!");
                webSocket?.Abort();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            Log.Info($"服务端 Dispose :释放weboscket资源!");
            this.webSocket?.Dispose();
            this.IsWebSocket = false;
            this.queue.Dispose();
        }
    }
}
