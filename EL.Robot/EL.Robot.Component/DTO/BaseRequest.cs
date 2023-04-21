using Automation.Inspect;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace EL.Robot.Component
{
    public class CommponetRequest : IRequest
    {
        public long FlowId { get; set; }
        public int RpcId { get; set; }
        public int TimeOut { get; set; } = 20000;
        public string ComponentName { get; set; }
        public dynamic Data { get; set; }
    }    
    public class ComponentResponse : IResponse
    {
        public ComponentResponse(int rpcId)
        {
            this.RpcId = rpcId;
        }
        public ComponentResponse()
        {
        }
        public int Error { get; set; } = 0;
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }
        public dynamic Data { get; set; }
    }
}
