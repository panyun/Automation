using ET;

namespace RPCBus.Server.Gate
{

    [ObjectSystem]
    class SessionKeyComponentAwakeSystem : AwakeSystem<SessionKeyComponent>
    {
        public override void Awake(SessionKeyComponent self)
        {
            Log.Info("[GATE]： Awake()");
        }
    }

    [ObjectSystem]
    class SessionKeyComponentDestorySystem : DestroySystem<SessionKeyComponent>
    {
        public override void Destroy(SessionKeyComponent self)
        {
            self.Clear();
            Log.Info("[GATE]： Destroy()");
        }
    }


    public static class SessionKeyComponentSystem
    {
        public static void Add(this SessionKeyComponent self, long key, Passport passport)
        {
            self.Keys.Add(key, passport);
            self.watchTimeout(key).Coroutine();
            Log.Debug($"[GATE]: 创建 <{passport.AccountId}:{passport.ClientId}@{passport.SessionId}> 的验证 key（{key}）");
        }

        public static void Clear(this SessionKeyComponent self)
        {
            self.Keys.Clear();
        }

        public static Passport TakeOut(this SessionKeyComponent self, long key)
        {
            self.Keys.Remove(key, out Passport passport);
            return passport;
        }

        private static async ET.ETVoid watchTimeout(this SessionKeyComponent self, long key)
        {
            await TimerComponent.Instance.WaitAsync(40000);

            if (self.Keys.Remove(key, out Passport passport))
            {
                Log.Debug($"[GATE]: 移除 <{passport.AccountId}:{passport.ClientId}@{passport.SessionId}> 的超时 key({key})");
            }
        }
    }
}
