using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core.Request
{
    [Utils.Message(OuterOpcode.ExecSqlRequest)]
    public class ExecSqlRequest : Utils.Object, IAgentRequset
    {
        /// <summary>
        /// RpcId
        /// </summary>
        public int RpcId { get; set; }
        /// <summary>
        /// cmd命令
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// type 1   add  2 update  3 del 4 query
         /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// agent
        /// </summary>
        public Utils.IRequest AgentRequest { get; set; }

    }
    [Utils.Message(OuterOpcode.ExecSqlResponse)]
    public class ExecSqlResponse : Utils.Object, Utils.IResponse
    {
        public int RpcId { get; set; }

        public int Error { get; set; }
        public object Data { get; set; }

        public string Message { get; set; }
    }
}
