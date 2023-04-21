using ET;

namespace ET
{
    public interface IAgentMessage : IMessage
    {
        public long TargetClientId { get; set; }

        public long SelfClientId { get; set; }
    }

    public interface IAgentRequest : IRequest
    {
        public long TargetClientId { get; set; }

        public long SelfClientId { get; set; }
    }

    public interface IAgentResponse : IResponse
    {
        public long TargetClientId { get; set; }

        public long SelfClientId { get; set; }
    }
}
