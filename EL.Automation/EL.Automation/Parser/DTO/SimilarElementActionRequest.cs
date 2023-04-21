using Automation.Inspect;
using EL;

namespace Automation.Parser
{
    /// <summary>
    /// 节点高亮
    /// </summary>
    public class SimilarElementActionRequest : RequestBase
    {
        public LightProperty LightProperty { get; set; } = new LightProperty();
        public bool IsElementPath = false;
    }

    public class SimilarElementActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public List<ElementUIA> Elements { get; set; }
        public List<ElementPath> ElementPaths { get; set; }
        public string StackTrace { get; set; }
    }

}
