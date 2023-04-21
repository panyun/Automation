namespace RPCBus.Server.Client
{
    public class Registration
    {
        public Registration(long playerId, string nickname)
        {
            Id = playerId;
            Nickname = nickname;
        }

        /// <summary>
        /// 玩家Id （clientid）
        /// </summary>
        public long Id;

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname;
    }
}
