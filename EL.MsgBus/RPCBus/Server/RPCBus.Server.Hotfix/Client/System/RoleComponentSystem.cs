namespace RPCBus.Server.Client
{
    [PlayerSystem]
    class RoleComponentCreateSystem : CreateSystem<RoleComponent>
    {
        public override void Create(RoleComponent self, Registration a)
        {
            self.Nickname = a.Nickname;
            self.Test = "test";
            self.IP = "sss";
        }
    }

    public static class RoleComponentSystem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IncreaseCurrency()
        {
            return true;
        }
        public static bool InCreaseAward()
        {
            return false;
        }

    }
}
