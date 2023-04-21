using Automation.Inspect;
using EL;

namespace Automation.Parser
{
    /// <summary>
    /// 节点高亮
    /// </summary>
    public class ElementActionRequest : RequestBase
    {
        public LightProperty LightProperty { get; set; } = new LightProperty();
    }
    public class ResponseBase : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }
    }
    public class ElementActionResponse : ResponseBase
    {
        public ElementPath ElementPath { get; set; }
    }

}
