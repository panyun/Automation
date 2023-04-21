using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Execute.Common
{
    public class WebSocketHelper
    {
        private string url;
        private ClientWebSocket clientWebSocket;
        public Action<byte[]> OnReceive;
        public Action OnOpen;
        public Action OnClose;
        public Queue<byte[]> sendQueues = new Queue<byte[]>();
        public bool Connected => clientWebSocket?.State == WebSocketState.Open;
        public WebSocketHelper(string url)
        {
            this.url = url;
        }
        public async Task Start()
        {
            clientWebSocket = await CreateAsync(url);
            OnOpen?.Invoke();
            _ = Task.Run(async () =>
            {
                while (clientWebSocket.State == WebSocketState.Open)
                {
                    while (sendQueues.Any())
                    {
                        var data = sendQueues.Dequeue();
                        await clientWebSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    SpinWait.SpinUntil(() => sendQueues?.Count > 0, 1000);
                }
            });
            _ = Task.Run(async () =>
            {
                var buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024 * 1);
                try
                {
                    var bufferData = new List<byte>();
                    while (clientWebSocket.State == WebSocketState.Open)
                    {
                        var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        }
                        bufferData.AddRange(buffer.AsSpan(0, result.Count).ToArray());
                        if (result.EndOfMessage)
                        {
                            var data = bufferData.ToArray();
                            _ = Task.Run(() =>
                            {
                                OnReceive?.Invoke(data);
                            });
                            bufferData.Clear();
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
                OnClose?.Invoke();
            });
        }
        public async Task Send(string data)
        {
            sendQueues.Enqueue(Encoding.UTF8.GetBytes(data));
            await Task.CompletedTask;
        }
        /// <summary>
        /// 创建客户端实例
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ClientWebSocket> CreateAsync(string ServerUri)
        {
            var webSocket = new ClientWebSocket();
            webSocket.Options.Proxy = null;

            await webSocket.ConnectAsync(new Uri(ServerUri), CancellationToken.None);
            if (webSocket.State == WebSocketState.Open)
            {
                return webSocket;
            }
            return null;
        }
        public async Task Close()
        {
            if (clientWebSocket != null && clientWebSocket.State == WebSocketState.Open)
            {
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }
}
