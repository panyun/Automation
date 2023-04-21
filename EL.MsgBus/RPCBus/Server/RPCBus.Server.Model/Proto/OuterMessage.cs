using ET;
using ProtoBuf;
using System.Collections.Generic;
using Object = ET.Object;
namespace RPCBus.Protos
{
    // TODO: [Client <=> Realm]
    [ResponseType(typeof(R2C_Login))]
    [Message(OuterOpcode.C2R_Login)]
    [ProtoContract]
    public partial class C2R_Login : Object, IRequest
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public long AccountId { get; set; }

        [ProtoMember(2)]
        public long ClientId { get; set; }

        [ProtoMember(3)]
        public int ClientType { get; set; }
    }

    [Message(OuterOpcode.R2C_Login)]
    [ProtoContract]
    public partial class R2C_Login : Object, IResponse
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(91)]
        public int Error { get; set; }

        [ProtoMember(92)]
        public string Message { get; set; }

        [ProtoMember(1)]
        public string Address { get; set; }

        [ProtoMember(2)]
        public int Port { get; set; }

        [ProtoMember(3)]
        public long AccountId { get; set; }

        [ProtoMember(4)]
        public long ClientId { get; set; }

        [ProtoMember(5)]
        public long Key { get; set; }
    }

    // TODO: [Client <=> Gate]
    [ResponseType(typeof(G2C_Enter))]
    [Message(OuterOpcode.C2G_Enter)]
    [ProtoContract]
    public partial class C2G_Enter : Object, IRequest
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public long Key { get; set; }

    }

    [Message(OuterOpcode.G2C_Enter)]
    [ProtoContract]
    public partial class G2C_Enter : Object, IResponse
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(91)]
        public int Error { get; set; }

        [ProtoMember(92)]
        public string Message { get; set; }

    }

    [ResponseType(typeof(G2C_Ping))]
    [Message(OuterOpcode.C2G_Ping)]
    [ProtoContract]
    public partial class C2G_Ping : Object, IRequest
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

    }

    [Message(OuterOpcode.G2C_Ping)]
    [ProtoContract]
    public partial class G2C_Ping : Object, IResponse
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(91)]
        public int Error { get; set; }

        [ProtoMember(92)]
        public string Message { get; set; }

        [ProtoMember(1)]
        public long Time { get; set; }

    }

    [Message(OuterOpcode.G2C_Abandoned)]
    [ProtoContract]
    public partial class G2C_Abandoned : Object, IActorMessage
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public int Code { get; set; }

        [ProtoMember(2)]
        public string Reason { get; set; }

    }

    [Message(OuterOpcode.C2G_MsgAgent)]
    [ProtoContract]
    public partial class C2G_MsgAgent : Object, IAgentRequest
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public string Content { get; set; }

        [ProtoMember(2)]
        public long TargetClientId { get; set; }

        [ProtoMember(3)]
        public long SelfClientId { get; set; }

    }

    [Message(OuterOpcode.G2C_MsgAgent)]
    [ProtoContract]
    public partial class G2C_MsgAgent : Object, IAgentResponse
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public string Content { get; set; }

        [ProtoMember(2)]
        public long TargetClientId { get; set; }

        [ProtoMember(3)]
        public long SelfClientId { get; set; }
        public int Error { get; set; }
        public string Message { get; set; }
    }
    [Message(OuterOpcode.P2PMessage)]
    [ProtoContract]
    public partial class P2PMessage : Object, IMessage  
    {
        [ProtoMember(1)]
        public string Type { get; set; }    

        [ProtoMember(2)]
        public string Role { get; set; }

        [ProtoMember(3)]
        public string Room { get; set; }

        [ProtoMember(4)]
        public Dictionary<string, object> Data { get; set; }
    }
    [Message(OuterOpcode.C2G_FileAgent)]
    [ProtoContract]
    public partial class C2G_FileAgent : Object, IAgentMessage
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public string Content { get; set; }

        [ProtoMember(2)]
        public long TargetClientId { get; set; }

        [ProtoMember(3)]
        public long SelfClientId { get; set; }

    }

    [Message(OuterOpcode.G2C_FileAgent)]
    [ProtoContract]
    public partial class G2C_FileAgent : Object, IAgentMessage
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public string Content { get; set; }

        [ProtoMember(2)]
        public long TargetClientId { get; set; }

        [ProtoMember(3)]
        public long SelfClientId { get; set; }

    }

    [Message(OuterOpcode.G2C_OnLine)]
    [ProtoContract]
    public partial class G2C_OnLine : Object, IMessage
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public long AccountId { get; set; }

        [ProtoMember(2)]
        public long ClientId { get; set; }

        [ProtoMember(3)]
        public string ClientType { get; set; }

    }

    [Message(OuterOpcode.G2C_OffLine)]
    [ProtoContract]
    public partial class G2C_OffLine : Object, IMessage
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(1)]
        public long AccountId { get; set; }

        [ProtoMember(2)]
        public long ClientId { get; set; }

        [ProtoMember(3)]
        public string ClientType { get; set; }

    }

    [ResponseType(typeof(M2C_TestRobotCase))]
    [Message(OuterOpcode.C2M_TestRobotCase)]
    [ProtoContract]
    public partial class C2M_TestRobotCase : Object, IActorLocationRequest
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(93)]
        public long ActorId { get; set; }

        [ProtoMember(1)]
        public int N { get; set; }

    }

    [Message(OuterOpcode.M2C_TestRobotCase)]
    [ProtoContract]
    public partial class M2C_TestRobotCase : Object, IActorLocationResponse
    {
        [ProtoMember(90)]
        public int RpcId { get; set; }

        [ProtoMember(91)]
        public int Error { get; set; }

        [ProtoMember(92)]
        public string Message { get; set; }

        [ProtoMember(1)]
        public int N { get; set; }

    }

}
