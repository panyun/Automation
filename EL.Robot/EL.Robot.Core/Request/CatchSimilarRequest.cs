using Automation.Inspect;
using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core.Request
{
    [Utils.Message(OuterOpcode.CatchSimilarRequest)]
    public class CatchSimilarRequest : Utils.Object, IAgentRequset
    {
        public int RpcId { get; set; }
        public bool IsSelfMachine { get; set; } = false;
        public string ClientNo { get; set; } = "RPAII";
        public int Type { get; set; }
        public double CosineValue { get; set; }
        public Utils.IRequest AgentRequest { get; set; }
    }

    [Utils.Message(OuterOpcode.CatchSimilarResponse)]
    public class CatchSimilarResponse : Utils.IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public ElementPath ElementPath { get; set; }
    }
}
