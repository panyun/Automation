using Automation.Inspect;
using EL;

namespace Automation.Parser
{
    public class GenerateHtmlActionRequest : RequestBase
    {
        public LightProperty LightProperty { get; set; } = new LightProperty();
    }
    public class GenerateHtmlActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }
        public string Html { get; set; }
    }

}
