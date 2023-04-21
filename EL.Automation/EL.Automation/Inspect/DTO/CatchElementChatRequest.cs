using EL;

namespace Automation.Inspect
{
    /// <summary>
    /// 聊天窗口抓取
    /// </summary>
    public class CatchElementChatRequest : IRequest
    {
        public int RpcId { get; set; }
        public string Msg { get; set; }
    }
    public class CatchElementChatResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public ElementPath ElementPath { get; set; }
        public string StackTrace { get; set; }
    }

}
