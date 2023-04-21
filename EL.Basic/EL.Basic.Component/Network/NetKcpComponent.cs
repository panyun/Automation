using EL.Async;
using EL.Basic.Network.Message;
using EL.Net.Network;
using EL.Net.Network.Message;
using System.Net;

namespace EL.Basic.Network
{
    [ObjectSystem]
    public class NetKcpComponentAwakeSystem : AwakeSystem<NetKcpComponent, int>
    {
        public override void Awake(NetKcpComponent self, int sessionStreamDispatcherType)
        {
            self.SessionStreamDispatcherType = sessionStreamDispatcherType;

            self.Service = new TService(NetThreadComponent.Instance.ThreadSynchronizationContext, ServiceType.Outer);
            self.Service.ErrorCallback += self.OnError;
            self.Service.ReadCallback += self.OnRead;

            NetThreadComponent.Instance.Add(self.Service);
        }
    }
    public class NetKcpComponent : Entity
    {
        public AService Service;
        public int SessionStreamDispatcherType { get; set; }
    }
    public static class NetKcpComponentSystem
    {
        public static void OnRead(this NetKcpComponent self, long channelId, MemoryStream memoryStream)
        {
            Session session = self.GetChild<Session>(channelId);
            if (session == null)
            {
                return;
            }

            session.LastRecvTime = TimeHelper.ClientNow();
            SessionStreamDispatcher.Instance.Dispatch(self.SessionStreamDispatcherType, session, memoryStream);
        }

        public static void OnError(this NetKcpComponent self, long channelId, int error)
        {
            Session session = self.GetChild<Session>(channelId);
            if (session == null)
            {
                return;
            }
            session.Error = error;
            session.Dispose();
        }

        // 这个channelId是由CreateAcceptChannelId生成的
        public static void OnAccept(this NetKcpComponent self, long channelId, IPEndPoint ipEndPoint)
        {
            Session session = self.AddChildWithId<Session, AService>(channelId, self.Service);
            session.RemoteAddress = ipEndPoint;

            session.AddComponent<SessionAcceptTimeoutComponent>();
            // 客户端连接，2秒检查一次recv消息，10秒没有消息则断开
            session.AddComponent<SessionIdleCheckerComponent, int>(2000);
        }

        public static Session Get(this NetKcpComponent self, long id)
        {
            Session session = self.GetChild<Session>(id);
            return session;
        }

        public static Session Create(this NetKcpComponent self, IPEndPoint realIPEndPoint)
        {
            long channelId = RandomHelper.RandInt64();
            Session session = self.AddChildWithId<Session, AService>(channelId, self.Service);
            session.RemoteAddress = realIPEndPoint;
            session.AddComponent<SessionIdleCheckerComponent, int>(2000);

            self.Service.GetOrCreate(session.Id, realIPEndPoint);

            return session;
        }
    }
}
