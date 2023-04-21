using Automation.Inspect;
using EL;

namespace Automation.Parser
{
    public class ElementPropertyActionRequest : RequestBase
    {

    }
    public class ElementPropertyActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int RpcId { get; set; }
        public ElementPath ElementPath { get; set; }
    }
}
