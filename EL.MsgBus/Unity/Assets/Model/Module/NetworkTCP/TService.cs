using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    public sealed class TService : AService
    {
        private readonly Dictionary<long, WebSocketChannel> idChannels = new Dictionary<long, WebSocketChannel>();
        private HttpListener listener;

        public HashSet<long> NeedStartSend = new HashSet<long>();
        private IPEndPoint localEndPoint;

        public TService(ThreadSynchronizationContext threadSynchronizationContext, ServiceType serviceType)
        {
            this.ServiceType = serviceType;
            this.ThreadSynchronizationContext = threadSynchronizationContext;
        }

        public TService(ThreadSynchronizationContext threadSynchronizationContext, IPEndPoint ipEndPoint, ServiceType serviceType)
        {
            this.localEndPoint = ipEndPoint;
            this.ServiceType = serviceType;
            this.ThreadSynchronizationContext = threadSynchronizationContext;

            listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{ipEndPoint.Port}/");
            listener.Start();
            Log.Info($"新增端口服务:{ipEndPoint}");
            this.ThreadSynchronizationContext.PostNext(this.AcceptAsync);
        }
        #region 网络线程

        private async Task OnAcceptComplete(HttpListenerContext httpListenerContext)
        {
            if (this.listener == null)
            {
                return;
            }
            try
            {
                HttpListenerWebSocketContext webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null, TimeSpan.FromSeconds(3));
                long id = this.CreateAcceptChannelId(0);
                WebSocketChannel channel = new WebSocketChannel(id, httpListenerContext.Request.RemoteEndPoint, webSocketContext.WebSocket, this);
                this.idChannels.Add(channel.Id, channel);
                long channelId = channel.Id;

                this.OnAccept(channelId, channel.RemoteAddress);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }



        private void AcceptAsync()
        {
            _ = Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (this.listener?.IsListening == true)
                    {
                        HttpListenerContext s = await this.listener.GetContextAsync();
                        if (!s.Request.IsWebSocketRequest)
                        {
                            s.Response.StatusCode = 404;
                            s.Response.StatusDescription = "Not Is WebSocket!";
                            s.Response.Close();
                            continue;
                        }
                        await OnAcceptComplete(s);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"端口服务异常:{localEndPoint} {ex.Message}");
                }
                Log.Info($"端口服务退出:{localEndPoint}");
            }, TaskCreationOptions.LongRunning);
            _ = Task.Factory.StartNew(() =>
            {
                while (this.listener != null)
                {
                    Log.Info($"端口:{localEndPoint} 服务状态:{this.listener?.IsListening}");
                    Thread.Sleep(10 * 1000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        private WebSocketChannel Create(IPEndPoint ipEndPoint, long id)
        {
            WebSocketChannel channel = new WebSocketChannel(id, ipEndPoint, this);
            this.idChannels.Add(channel.Id, channel);
            return channel;
        }

        protected override void Get(long id, IPEndPoint address)
        {
            if (this.idChannels.TryGetValue(id, out WebSocketChannel _))
            {
                return;
            }
            this.Create(address, id);
        }

        public WebSocketChannel Get(long id)
        {
            WebSocketChannel channel = null;
            this.idChannels.TryGetValue(id, out channel);
            return channel;
        }
        public override bool IsDispose()
        {
            return this.listener == null || this.ThreadSynchronizationContext == null;
        }

        public override void Dispose()
        {
            Log.Info($"端口服务释放:{localEndPoint}");
            try
            {
                this.listener?.Close();
            }
            catch (Exception)
            {
            }
            this.listener = null;
            ThreadSynchronizationContext = null;

            foreach (long id in this.idChannels.Keys.ToArray())
            {
                WebSocketChannel channel = this.idChannels[id];
                channel.Dispose();
            }
            this.idChannels.Clear();
        }

        public override void Remove(long id)
        {
            if (this.idChannels.TryGetValue(id, out WebSocketChannel channel))
            {
                channel.Dispose();
            }

            this.idChannels.Remove(id);
        }

        protected override void Send(long channelId, long actorId, MemoryStream stream)
        {
            try
            {
                WebSocketChannel aChannel = this.Get(channelId);
                if (aChannel == null)
                {
                    this.OnError(channelId, ErrorCode.ERR_SendMessageNotFoundTChannel);
                    return;
                }
                aChannel.Send(actorId, stream);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public override void Update()
        {
            foreach (long channelId in this.NeedStartSend)
            {
                WebSocketChannel tChannel = this.Get(channelId);
                tChannel?.Update();
            }
            this.NeedStartSend.Clear();
        }
        #endregion

    }
}