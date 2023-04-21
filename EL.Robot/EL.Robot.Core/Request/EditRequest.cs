using Automation.Inspect;
using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core.Request
{
    [Utils.Message(OuterOpcode.EditRequest)]
    public class EditRequest : Utils.Object, IAgentRequset
    {
        public int RpcId { get; set; }
        public ElementPath ElementPath { get; set; }
        public Utils.IRequest AgentRequest { get; set; }
    }

    [Utils.Message(OuterOpcode.EditResponse)]
    public class EditResponse : Utils.IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public ElementPath ElementPath { get; set; }
    }
}
