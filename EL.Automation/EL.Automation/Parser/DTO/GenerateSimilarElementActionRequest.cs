using Automation.Inspect;
using Automation.Parser;
using EL;

namespace Automation.Parser
{
    public class GenerateSimilarElementActionRequest : RequestBase
    {
        public ElementPath LastElementPath { get; set; }
        public LightProperty LightProperty { get; set; } = new LightProperty();
    }
    
    public class GenerateSimilarElementActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int RpcId { get; set; }
        public ElementPath ElementPath { get; set; }
        /// <summary>
        /// 相似条数
        /// </summary>
        public int Count { get; set; }
    }
}
