using EL;

namespace Automation.Parser
{
    public class StartMsgActionRequest : RequestBase
    {
        public OutType OutType { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
   
    public enum OutType
    {
        Sqlite = 1,
        HttpApi = 4,
    }
    public class StartMsgActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }
        public long TaskId { get; set; }
    }

}
