using Automation.Inspect;
using Protos;

namespace EL.Robot.Core.Request
{
    [Utils.Message(OuterOpcode.CatchRequest)]
    public class CatchRequest : Utils.Object, IAgentRequset
    {
        public int RpcId { get; set; }
        public bool IsSelfMachine { get; set; } = true;
        public string ClientNo { get; set; } = "RPAII";
        public Utils.IRequest AgentRequest { get; set; }
    }

    [Utils.Message(OuterOpcode.CatchResponse)]
    public class CatchResponse : Utils.IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public ElementPath ElementPath { get; set; }
    }
    [Utils.Message(OuterOpcode.LocationRequest)]
    public class LocationRequest : Utils.IRequest
    {
        public long ComponentId { get; set; }
        public int RpcId { get; set; }
        public List<ExecLog> LogMsgs { get; set; }
    }
}
