
using EL.Net.Network.Message;
using Utils;
using Object = Utils.Object;

namespace Protos
{
    // TODO: [Client <=> Realm]
    [Utils.Message(OuterOpcode.C2R_Login)]
    public partial class C2R_Login : Object, IRequest
    {
        public int RpcId { get; set; }

        public long AccountId { get; set; }
        public long ClientId { get; set; }

        public int ClientType { get; set; }

    }

    [Utils.Message(OuterOpcode.R2C_Login)]
    public partial class R2C_Login : Object, IResponse
    {
        public int RpcId { get; set; }

        public int Error { get; set; }

        public string Message { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }
        public long AccountId { get; set; }

        public long ClientId { get; set; }

        public long Key { get; set; }

    }

    // TODO: [Client <=> Gate]
    [Utils.Message(OuterOpcode.C2G_Enter)]
    public partial class C2G_Enter : Object, IRequest
    {
        public int RpcId { get; set; }

        public long Key { get; set; }

    }

    [Utils.Message(OuterOpcode.G2C_Enter)]
    public partial class G2C_Enter : Object, IResponse
    {
        public int RpcId { get; set; }

        public int Error { get; set; }

        public string Message { get; set; }

    }

    [Utils.Message(OuterOpcode.C2G_Ping)]
    public partial class C2G_Ping : Object, IRequest
    {
        public int RpcId { get; set; }

    }

    [Utils.Message(OuterOpcode.G2C_Ping)]
    public partial class G2C_Ping : Object, IResponse
    {
        public int RpcId { get; set; }

        public int Error { get; set; }

        public string Message { get; set; }

        public long Time { get; set; }

    }

    [Utils.Message(OuterOpcode.G2C_Abandoned)]
    public partial class G2C_Abandoned : Object, IActorMessage
    {
        public int RpcId { get; set; }

        public int Code { get; set; }

        public string Reason { get; set; }

    }

    [Utils.Message(OuterOpcode.C2G_MsgAgent)]
    public partial class C2G_MsgAgent : Object, IAgentRequest
    {
        public int RpcId { get; set; }

        public string Content { get; set; }

        public long TargetClientId { get; set; }

        public long SelfClientId { get; set; }

    }

    [Utils.Message(OuterOpcode.G2C_MsgAgent)]
    public partial class G2C_MsgAgent : Object, IAgentResponse
    {
        public int RpcId { get; set; }
        public int Error { get; set; }
        public string Message { get; set; }
        public string Content { get; set; }
        public long TargetClientId { get; set; }
        public long SelfClientId { get; set; }
    }

    [Utils.Message(OuterOpcode.C2G_FileAgent)]
    public partial class C2G_FileAgent : Object, IAgentMessage
    {
        public int RpcId { get; set; }

        public string Content { get; set; }

        public long TargetClientId { get; set; }

        public long SelfClientId { get; set; }

    }

    [Utils.Message(OuterOpcode.G2C_FileAgent)]
    public partial class G2C_FileAgent : Object, IAgentMessage
    {
        public int RpcId { get; set; }

        public string Content { get; set; }

        public long TargetClientId { get; set; }

        public long SelfClientId { get; set; }

    }

    [Utils.Message(OuterOpcode.G2C_OnLine)]
    public partial class G2C_OnLine : Object, IMessage
    {
        public int RpcId { get; set; }
        public long AccountId { get; set; }

        public long ClientId { get; set; }

        public string ClientType { get; set; }

    }

    [Utils.Message(OuterOpcode.G2C_OffLine)]
    public partial class G2C_OffLine : Object, IMessage
    {
        public int RpcId { get; set; }
        public long AccountId { get; set; }

        public long ClientId { get; set; }

        public string ClientType { get; set; }

    }

    [Utils.Message(OuterOpcode.C2M_TestRobotCase)]
    public partial class C2M_TestRobotCase : Object, IActorLocationRequest
    {
        public int RpcId { get; set; }

        public long ActorId { get; set; }

        public int N { get; set; }

    }

    [Utils.Message(OuterOpcode.M2C_TestRobotCase)]
    public partial class M2C_TestRobotCase : Object, IActorLocationResponse
    {
        public int RpcId { get; set; }

        public int Error { get; set; }

        public string Message { get; set; }
        public string StackTrace { get; set; }

        public int N { get; set; }

    }

}

