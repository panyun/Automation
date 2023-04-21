using Automation.Inspect;
using EL;

namespace Automation.Parser
{
    /// <summary>
    /// 父节点
    /// </summary>
    public class ParentElementActionRequest : RequestBase
    {
        public LightProperty LightProperty { get; set; } = new LightProperty();
        /// <summary>
        /// 是否返回路径信息
        /// </summary>
        public bool IsElementPath { get; set; } = false;
    }

    public class ParentElementActionActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }
        public List<ElementUIA> Elements { get; set; }
        public List<ElementPath> ElementPaths { get; set; }
    }

}
