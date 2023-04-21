using ET;

namespace RPCBus.Server.Gate
{
    public class Passport
    {
        public string SessionId { get; set; }
        public string Nickname { get; set; }
        public long AccountId { get; set; }
        public long ClientId { get; set; }
        public int ClientType { get; set; }
    }

    public class SessionKeyComponent : Entity
    {
        public Dictionary<long, Passport> Keys = new Dictionary<long, Passport>();
    }
}
