namespace RPCBus.Protos
{
    public static partial class OuterOpcode
    {
        public const ushort C2R_Login = 61001;
        public const ushort R2C_Login = 61002;
        public const ushort C2G_Enter = 61003;
        public const ushort G2C_Enter = 61004;
        public const ushort C2G_Ping = 61005;
        public const ushort G2C_Ping = 61006;
        public const ushort G2C_Abandoned = 61007;
        public const ushort C2G_MsgAgent = 61008;
        public const ushort G2C_MsgAgent = 61009;
        public const ushort C2G_FileAgent = 61010;
        public const ushort G2C_FileAgent = 61011;
        public const ushort G2C_OnLine = 61012;
        public const ushort G2C_OffLine = 61013;
        public const ushort C2M_TestRobotCase = 61014;
        public const ushort M2C_TestRobotCase = 61015;
        public const ushort P2PMessage = 61016;

        public const ushort MAX = 61016;
    }
}
