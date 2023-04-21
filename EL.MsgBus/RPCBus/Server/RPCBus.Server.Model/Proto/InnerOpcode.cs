namespace RPCBus.Protos
{
	public static partial class InnerOpcode
	{
		 public const ushort R2G_GetLoginKey = 60001;
		 public const ushort G2R_GetLoginKey = 60002;
		 public const ushort G2P_Enter = 60003;
		 public const ushort P2G_Enter = 60004;
		 public const ushort G2P_Leave = 60005;
		 public const ushort DisconnectRequest = 60006;
		 public const ushort DisconnectResponse = 60007;
		 public const ushort RemoveNotification = 60008;
		 public const ushort FungibleToken = 60009;
		 public const ushort FTObjectAdd = 60010;
		 public const ushort MsgAgentRequest = 60011;
		 public const ushort MsgAgentResponse = 60012;
		 public const ushort LogWrite = 60013;
		 public const ushort DBLog = 60014;

		 public const ushort MAX = 60014;
	}
}
