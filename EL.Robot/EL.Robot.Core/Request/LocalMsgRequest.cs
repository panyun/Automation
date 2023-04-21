using Protos;
using Object = Utils.Object;

namespace EL.Robot.Core
{
    [Utils.Message(OuterOpcode.LocalMsgRequest)]
    public class LocalMsgRequest : Object, Utils.IRequest
    {
        public int RpcId { get; set; }
        public long ClientId { get; set; }
        public string Content { get; set; }
    }
    [Utils.Message(OuterOpcode.LocalMsgResponse)]
    public class LocalMsgResponse : Object, Utils.IResponse
    {
        public int RpcId { get; set; }
        public long ClientId { get; set; }
        public int Error { get; set; }
        public string Message { get; set; }
    }
}
