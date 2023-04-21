using Automation.Inspect;
using EL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
    public class HighlightActionRequest : RequestBase
    {
        public LightProperty LightProperty { get; set; } = new LightProperty();
        public string Title { get; set; }
        public string XPath { get; set; }
    }
    public class HighlightActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }
        public int Count { get; set; }
    }
}
