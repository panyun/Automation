using EL.Net.Network.Message;
using EL.Robot.Component;
using Protos;

namespace EL.Robot.Core.Request
{
    public interface IAgentRequset : Utils.IRequest
    {
        public Utils.IRequest AgentRequest { get; set; }
    }
    [Utils.Message(OuterOpcode.RunRequest)]
    public class RunRequest : Utils.Object, IAgentRequset
    {
        public Flow Flow { get; set; }
        public bool IsSelfMachine { get; set; }
        public string ClientNo { get; set; }
        public bool IsDebug { get; set; }
        public int RpcId { get; set; }
        public Utils.IRequest AgentRequest { get; set; }
    }
    [Utils.Message(OuterOpcode.RunResponse)]
    public class RunResponse : Utils.Object, Utils.IResponse
    {
        public int RpcId { get; set; }

        public int Error { get; set; }

        public string Message { get; set; }
    }
    [Utils.Message(OuterOpcode.RunMsg)]
    public class RunMsg : Utils.Object, Utils.IResponse
    {
        public int RpcId { get; set; }

        public int Error { get; set; }

        public string Message { get; set; }
    }
}
