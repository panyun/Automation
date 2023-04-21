using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EL.SignalingService.Model
{
    /// <summary>
    /// WebSocketServer
    /// </summary>
    public class WebSocketServer
    {
        /// <summary>
        /// 核心监听方法
        /// </summary>
        HttpListener listener;
        /// <summary>
        /// 服务端监听的端口  作为服务端口
        /// </summary>
        public int ListenPort;
        /// <summary>
        /// 监听的端口
        /// </summary>
        /// <param name="port"></param>
        public WebSocketServer(int port)
        {
            ListenPort = port;
        }
        /// <summary>
        /// websocket 事件
        /// </summary>  
        /// <param name="UserToken"></param>
        public delegate void WebSocketHandler(UserToken userToken, byte[] data);
        /// <summary>
        /// 新用户的事件
        /// </summary>
        public event WebSocketHandler OnOpen;
        /// <summary>
        /// 新用户的事件
        /// </summary>
        public event WebSocketHandler OnClose;
        /// <summary>
        /// 新用户的事件
        /// </summary>
        public event WebSocketHandler OnMessage;
        /// <summary>
        /// 异步发送队列
        /// </summary>
        private ConcurrentDictionary<UserToken, ConcurrentQueue<byte[]>> Sends = new();
        /// <summary>
        /// 开始监听
        /// </summary>
        /// <returns></returns>
        public WebSocketServer Listen()
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://*:{ListenPort}/");
                listener.Start();
                ServerStart();
                Task.Run(async () =>
                {
                    while (true)
                    {
                        HttpListenerContext httpListenerContext = await listener.GetContextAsync();
                        if (!httpListenerContext.Request.IsWebSocketRequest)
                        {
                            _ = Task.Run(() =>
                            {
                                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "webrtc.html");
                                byte[] buffer = null;
                                if (httpListenerContext.Request.Url?.PathAndQuery.StartsWith("/webrtc") == true && File.Exists(path))
                                {
                                    buffer = File.ReadAllBytes(path);
                                }
                                else
                                {
                                    string responseString = $"<HTML><BODY> 需支持WebSocket才可以正常访问!{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")}</BODY></HTML>";
                                    buffer = Encoding.UTF8.GetBytes(responseString);
                                }
                                //对客户端输出相应信息.
                                httpListenerContext.Response.StatusCode = 200;
                                httpListenerContext.Response.ContentType = "text/html;charset=utf-8";
                                httpListenerContext.Response.ContentLength64 = buffer.Length;
                                Stream output = httpListenerContext.Response.OutputStream;
                                output.Write(buffer, 0, buffer.Length);
                                output.Close();
                            });
                        }
                        //来一个新的链接
                        ThreadPool.QueueUserWorkItem(r => { _ = Accept(httpListenerContext); });
                    }
                });
            }
            catch (HttpListenerException e)
            {
                if (e.ErrorCode == 5)
                {
                    throw new Exception($"CMD管理员权限 输入: netsh http add urlacl url=http://*:{ListenPort}/ user=Everyone", e);
                }
            }
            return this;
        }
        /// <summary>
        /// 一个新的连接
        /// </summary>
        /// <param name="s"></param>
        public async Task Accept(HttpListenerContext httpListenerContext)
        {
            HttpListenerWebSocketContext webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);
            UserToken userToken = new UserToken();
            userToken.ConnectTime = DateTime.Now;
            userToken.WebSocket = webSocketContext.WebSocket;
            userToken.RemoteAddress = httpListenerContext.Request.RemoteEndPoint;
            userToken.IPAddress = ((IPEndPoint)userToken.RemoteAddress).Address;
            try
            {
                ConcurrentQueue<byte[]> sendQueues = new ConcurrentQueue<byte[]>();
                Sends.TryAdd(userToken, sendQueues);
                #region 异步单队列发送任务
                _ = Task.Run(async () =>
                {
                    while (userToken.WebSocket != null && userToken.WebSocket.State == WebSocketState.Open)
                    {
                        if (sendQueues?.Count > 0)
                        {
                            while (true)
                            {
                                if (sendQueues.TryDequeue(out var data))
                                {
                                    await userToken.WebSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        SpinWait.SpinUntil(() => sendQueues?.Count > 0, 1000);
                    }
                });
                #endregion
                newAcceptHandler(userToken);
                var buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024 * 1);
                try
                {
                    var WebSocket = userToken.WebSocket;
                    var bufferData = new List<byte>();
                    while (WebSocket != null && WebSocket.State == WebSocketState.Open)
                    {
                        var result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await userToken.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        }
                        bufferData.AddRange(buffer.Take(result.Count).ToArray());
                        if (result.EndOfMessage)
                        {
                            var data = bufferData.ToArray();
                            _ = Task.Run(() =>
                            {
                                try
                                {
                                    OnMessage?.Invoke(userToken, data);
                                }
                                catch (Exception)
                                {
                                }
                            });
                            bufferData.Clear();
                        }
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                newQuitHandler(userToken);
            }
        }
        /// <summary>
        /// 新的链接
        /// </summary>
        public void newAcceptHandler(UserToken userToken)
        {
            if (OnOpen != null)
            {
                OnOpen(userToken, null);
            }
            else
            {
                Console.WriteLine("一个新的用户:" + userToken.RemoteAddress.ToString());
            }
        }
        /// <summary>
        /// 服务开始
        /// </summary>
        public void ServerStart()
        {
            Console.WriteLine("服务开启:local:" + ListenPort);
        }
        /// <summary>
        /// 用户退出
        /// </summary>
        public void newQuitHandler(UserToken userToken)
        {
            if (OnClose != null)
            {
                OnClose(userToken, null);
            }
            else
            {
                Console.WriteLine("用户退出:" + userToken.RemoteAddress.ToString());
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public async Task CloseAsync(UserToken token)
        {
            await token.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        /// <summary>
        /// 对客户发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Send(UserToken userToken, string data)
        {
            if (userToken == null)
            {
                return;
            }
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} - id:{userToken.ID}:role:{userToken.Role} 发送消息:{data}");
            Send(userToken, Encoding.UTF8.GetBytes(data));
        }
        /// <summary>
        /// 对客户发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Send(UserToken token, byte[] data)
        {
            try
            {
                if (token == null)
                {
                    return;
                }
                Sends.AddOrUpdate(token, new ConcurrentQueue<byte[]>(new List<byte[]>() { data }), (k, v) =>
                {
                    v.Enqueue(data);
                    return v;
                });
            }
            catch (Exception)
            { }
        }
    }
}
