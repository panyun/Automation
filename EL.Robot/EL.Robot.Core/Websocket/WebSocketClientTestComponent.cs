using EL.Async;
using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EL.Robot.Core.Websocket
{
    public class WebSocketClientTestComponent : Entity
    {
        public ClientWebSocket WebSocket { get; set; }
        public readonly Dictionary<int, RpcInfo> RequestCallbacks = new Dictionary<int, RpcInfo>();

    }
    public class WebSocketClientTestComponentAwake : AwakeSystem<WebSocketClientTestComponent>
    {
        public override void Awake(WebSocketClientTestComponent self)
        {


        }
    }

    public static class WebSocketClientTestComponentSystem
    {
        public static async ELTask ConnectAsync(this WebSocketClientTestComponent self, string url = "ws://192.168.0.99:10001")
        {
            self.WebSocket = new ClientWebSocket()
            {
                Options =
                {
                     KeepAliveInterval =TimeSpan.FromMinutes(60),
                }
            };
            await self.WebSocket.ConnectAsync(new Uri(url), CancellationToken.None);
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var array = new byte[4096];
                    var result = await self.WebSocket.ReceiveAsync(new ArraySegment<byte>(array), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string para = Encoding.Default.GetString(array, 0, result.Count);
                        var opcode = ushort.Parse(para.Substring(0, 5));
                        var msg = para.Substring(5, para.Length - 5);
                        var type = OpcodeTypeComponent.GetType(opcode);
                        var message = JsonHelper.FromJson(type, para.Substring(5, para.Length - 5));
                        if (message is G2C_OnLine online)
                        {
                            RoboatServerComponent.Add(online);
                        }
                        if (message is Utils.IResponse response)
                        {
                            if (!self.RequestCallbacks.TryGetValue(response.RpcId, out var action))
                                return;
                            self.RequestCallbacks.Remove(response.RpcId);
                            action.Tcs.SetResult(response);
                        }
                    }
                }
            });
        }
        public static async ELTask SendAsync(this WebSocketClientTestComponent self, byte[] bytes)
        {
            await ELTask.CompletedTask;
            //if (bytes.Length == 53|| bytes.Length == 54)
            //{
            //    byte[] buffer = default;
            //    using (var file = File.OpenRead(@"C:\Users\panyun.li\Desktop\test11.png"))
            //    {
            //        buffer = new byte[file.Length];
            //        file.Read(buffer, 0, buffer.Length);
            //    }
            //    var array1 = new ArraySegment<byte>(buffer);
            //    Log.Info("开始发送");
            //    await self.WebSocket.SendAsync(array1, WebSocketMessageType.Binary, true, CancellationToken.None).ContinueWith((x) =>
            //    {
            //        Log.Info("发送成功");
            //    });
            //    return;
            //}
            Log.Info("Send--> {0}", Encoding.UTF8.GetString(bytes));
            var array = new ArraySegment<byte>(bytes);
            _ = self.WebSocket.SendAsync(array, WebSocketMessageType.Text, true, CancellationToken.None).ContinueWith((x) =>
            {
            });
        }
        public static int RpcId = 0;
        public static async ELTask CallTest(this WebSocketClientTestComponent self, Utils.IRequest request, string url = default)
        {
            await ELTask.CompletedTask;
            try
            {

                if (!string.IsNullOrEmpty(url))
                    await self.ConnectAsync(url);
                await ELTask.CompletedTask;
                int rpcId = ++RpcId;
                RpcInfo rpcInfo = new RpcInfo(request);
                self.RequestCallbacks[rpcId] = rpcInfo;
                request.RpcId = rpcId;
                //待发送字符串
                var msg = OpcodeTypeComponent.GetOpcode(request.GetType()) + JsonHelper.ToJson(request);
                var bytes = Encoding.UTF8.GetBytes(msg);
                await self.SendAsync(bytes); //执行发送

                //return await rpcInfo.Tcs;
                //byte[] bytes2 = new byte[2];
                //bytes2[0] = 0;
                //bytes2[0] = 1;
                //var s = System.Text.Encoding.UTF8.GetString(bytes2);
                //byte[] sendBytes = default;
                ////int pageSize = (int)0xFFFF - 1024;
                //int pageSize =  1024*50;
                //if (bytes.Length + 1 < pageSize)
                //{
                //    sendBytes = new byte[bytes.Length + 1];
                //    sendBytes[0] = 0; //单次发送直接添加结束标识符
                //    Array.Copy(bytes, 0, sendBytes, 1, bytes.Length);
                //    await self.SendAsync(sendBytes);
                //}
                //else
                //{
                //    int num = 0;
                //    bool isPage = true;
                //    //已经发送的字节数据
                //    int taked = 0;
                //    //是否存在下一页
                //    while (isPage)
                //    {
                //        //分页byte
                //        var temp = bytes.Skip(num * pageSize).Take(pageSize).ToArray();
                //        taked += temp.Length;
                //        //如果发送的byte 大于 总字符串则继续 。否则最后一次发送
                //        isPage = bytes.Length > taked;
                //        sendBytes = new byte[temp.Length + 1];
                //        sendBytes[0] = 1;   //如果不是最后一次发送则为1
                //        var length = temp.Length;
                //        if (!isPage)
                //            sendBytes[0] = 0; //末次发送则添加结束标识符
                //        Array.Copy(temp, 0, sendBytes, 1, temp.Length);//组合发送数组
                //        Thread.Sleep(10);
                //        await self.SendAsync(sendBytes); //执行发送
                //        num++;
                //    }
                //}
                //return await rpcInfo.Tcs;
            }
            catch (Exception ex)
            {
            
            }
        }
        public static async ELTask<Utils.IResponse> Call(this WebSocketClientTestComponent self, Utils.IRequest request, string url = default)
        {
            await ELTask.CompletedTask;
            try
            {

                if (!string.IsNullOrEmpty(url))
                    await self.ConnectAsync(url);
                await ELTask.CompletedTask;
                int rpcId = ++RpcId;
                RpcInfo rpcInfo = new RpcInfo(request);
                self.RequestCallbacks[rpcId] = rpcInfo;
                request.RpcId = rpcId;
                //待发送字符串
                var msg = OpcodeTypeComponent.GetOpcode(request.GetType()) + JsonHelper.ToJson(request);
                var bytes = Encoding.UTF8.GetBytes(msg);
                await self.SendAsync(bytes); //执行发送

                return await rpcInfo.Tcs;
                //byte[] bytes2 = new byte[2];
                //bytes2[0] = 0;
                //bytes2[0] = 1;
                //var s = System.Text.Encoding.UTF8.GetString(bytes2);
                //byte[] sendBytes = default;
                ////int pageSize = (int)0xFFFF - 1024;
                //int pageSize =  1024*50;
                //if (bytes.Length + 1 < pageSize)
                //{
                //    sendBytes = new byte[bytes.Length + 1];
                //    sendBytes[0] = 0; //单次发送直接添加结束标识符
                //    Array.Copy(bytes, 0, sendBytes, 1, bytes.Length);
                //    await self.SendAsync(sendBytes);
                //}
                //else
                //{
                //    int num = 0;
                //    bool isPage = true;
                //    //已经发送的字节数据
                //    int taked = 0;
                //    //是否存在下一页
                //    while (isPage)
                //    {
                //        //分页byte
                //        var temp = bytes.Skip(num * pageSize).Take(pageSize).ToArray();
                //        taked += temp.Length;
                //        //如果发送的byte 大于 总字符串则继续 。否则最后一次发送
                //        isPage = bytes.Length > taked;
                //        sendBytes = new byte[temp.Length + 1];
                //        sendBytes[0] = 1;   //如果不是最后一次发送则为1
                //        var length = temp.Length;
                //        if (!isPage)
                //            sendBytes[0] = 0; //末次发送则添加结束标识符
                //        Array.Copy(temp, 0, sendBytes, 1, temp.Length);//组合发送数组
                //        Thread.Sleep(10);
                //        await self.SendAsync(sendBytes); //执行发送
                //        num++;
                //    }
                //}
                //return await rpcInfo.Tcs;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

    }
}
