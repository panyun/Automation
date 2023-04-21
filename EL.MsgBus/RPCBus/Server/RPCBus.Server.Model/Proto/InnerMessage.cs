using ET;
using ProtoBuf;
using System.Collections.Generic;
using Object = ET.Object;
namespace RPCBus.Protos
{
// TODO: [Realm <=> Gate]
	[ResponseType(typeof(G2R_GetLoginKey))]
	[Message(InnerOpcode.R2G_GetLoginKey)]
	[ProtoContract]
	public partial class R2G_GetLoginKey: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		//int64 ActorId = 93;
		//int64 ActorId = 93;
		[ProtoMember(1)]
		public long AccountId { get; set; }

        [ProtoMember(2)]
        public long ClientId { get; set; }

		[ProtoMember(3)]
		public string Nickname { get; set; }

		[ProtoMember(4)]
		public string SessionId { get; set; }

		[ProtoMember(5)]
		public int ClientType { get; set; }
    }

	[Message(InnerOpcode.G2R_GetLoginKey)]
	[ProtoContract]
	public partial class G2R_GetLoginKey: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long Key { get; set; }

	}

// TODO: [Gate <=> Player]
	[ResponseType(typeof(P2G_Enter))]
	[Message(InnerOpcode.G2P_Enter)]
	[ProtoContract]
	public partial class G2P_Enter: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

        [ProtoMember(1)]
        public long AccountId { get; set; }

        [ProtoMember(2)]
        public long ClientId { get; set; }

        [ProtoMember(3)]
		public string Nickname { get; set; }

		[ProtoMember(4)]
		public string SessionId { get; set; }

		[ProtoMember(5)]
		public long ClientActorId { get; set; }

		[ProtoMember(6)]
		public int ClientType { get; set; }

	}

	[Message(InnerOpcode.P2G_Enter)]
	[ProtoContract]
	public partial class P2G_Enter: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public long PlayerActorId { get; set; }

	}

	[Message(InnerOpcode.G2P_Leave)]
	[ProtoContract]
	public partial class G2P_Leave: Object, IActorMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string SessionId { get; set; }

	}

	[ResponseType(typeof(DisconnectResponse))]
	[Message(InnerOpcode.DisconnectRequest)]
	[ProtoContract]
	public partial class DisconnectRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public int Code { get; set; }

		[ProtoMember(2)]
		public int Delay { get; set; }

		[ProtoMember(3)]
		public string Reason { get; set; }

	}

	[Message(InnerOpcode.DisconnectResponse)]
	[ProtoContract]
	public partial class DisconnectResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

	}

	[Message(InnerOpcode.RemoveNotification)]
	[ProtoContract]
	public partial class RemoveNotification: Object, IActorNotification
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(93)]
		public long NotificationId { get; set; }

		[ProtoMember(1)]
		public long Receiver { get; set; }

	}

	[Message(InnerOpcode.FungibleToken)]
	[ProtoContract]
	public partial class FungibleToken: Object
	{
		[ProtoMember(1)]
		public string ItemId { get; set; }

		[ProtoMember(2)]
		public long ConfigId { get; set; }

	}

	[Message(InnerOpcode.FTObjectAdd)]
	[ProtoContract]
	public partial class FTObjectAdd: Object, IActorNotification
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(93)]
		public long NotificationId { get; set; }

		[ProtoMember(1)]
		public long Receiver { get; set; }

		[ProtoMember(2)]
		public FungibleToken Value { get; set; }

	}

	[ResponseType(typeof(MsgAgentResponse))]
	[Message(InnerOpcode.MsgAgentRequest)]
	[ProtoContract]
	public partial class MsgAgentRequest: Object, IActorRequest
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(1)]
		public string Content { get; set; }

        [ProtoMember(2)]
        public long AccountId { get; set; }

        [ProtoMember(3)]
        public long ClientId { get; set; }
    }

	[Message(InnerOpcode.MsgAgentResponse)]
	[ProtoContract]
	public partial class MsgAgentResponse: Object, IActorResponse
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

		[ProtoMember(91)]
		public int Error { get; set; }

		[ProtoMember(92)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public string Content { get; set; }

        [ProtoMember(2)]
        public long AccountId { get; set; }

        [ProtoMember(3)]
        public long ClientId { get; set; }

    }

	[Message(InnerOpcode.LogWrite)]
	[ProtoContract]
	public partial class LogWrite: Object, IActorMessage
	{
		[ProtoMember(90)]
		public int RpcId { get; set; }

        [ProtoMember(1)]
        public long AccountId { get; set; }

        [ProtoMember(2)]
        public long ClientId { get; set; }

        [ProtoMember(3)]
		public long Date { get; set; }

		[ProtoMember(4)]
		public long ProcessId { get; set; }

		[ProtoMember(5)]
		public long SceneId { get; set; }

		[ProtoMember(6)]
		public string SceneType { get; set; }

		[ProtoMember(7)]
		public string KeyWorld { get; set; }

		[ProtoMember(8)]
		public string Component { get; set; }

		[ProtoMember(9)]
		public List<DBLog> DBLogs = new List<DBLog>();

	}

	[Message(InnerOpcode.DBLog)]
	[ProtoContract]
	public partial class DBLog: Object
	{
		[ProtoMember(1)]
		public string Tag { get; set; }

		[ProtoMember(2)]
		public string Msg { get; set; }

	}

}
