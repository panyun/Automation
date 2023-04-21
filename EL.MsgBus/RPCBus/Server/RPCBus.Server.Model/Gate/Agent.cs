using ET;

namespace RPCBus.Server.Gate
{
    public sealed class Agent : Entity
    {
        public string SessionId { get; set; }
        public long AccountId { get; set; }
        public long ClientId { get; set; }
        public int ClientType { get; set; }
        /// <summary>
        /// 在线玩家在 Player 进程中的 ActorId
        /// </summary>
        public long PlayerActorId { get; set; }
        public long SessionTServiceId { get; set; }
        public Session Session { get; set; }
        public string ClientTypeInfo
        {
            get
            {
                return GetType(this.ClientType);
            }
        }
        public static string GetType(int type)
        {
            if (type == 1) return "PC机器人";
            if (type == 2) return "移动机器人";
            if (type == 3) return "设计器";
            return "客服端";
        }
    }
}
