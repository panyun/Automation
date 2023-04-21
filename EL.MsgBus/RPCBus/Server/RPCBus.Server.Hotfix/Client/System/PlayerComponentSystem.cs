using ET;
using RPCBus.Server.EventType;

namespace RPCBus.Server.Client
{
    [ObjectSystem]
    class PlayerComponentAwakeSystem : AwakeSystem<PlayerComponent>
    {
        public override void Awake(PlayerComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    class PlayerComponentDestroySystem : DestroySystem<PlayerComponent>
    {
        public override void Destroy(PlayerComponent self)
        {
            Log.Info("[STAGE]： Destroy()");
        }
    }

    public static class PlayerComponentSystem
    {
        /// <summary>
        /// 直接从缓存中取出 Player 对象， 在 PlayerActor 中的逻辑可通过该接口来取数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId"></param>
        /// <returns>成功返回缓存中的 Player 对象，失败抛出异常 </returns>
        public static Player Get(this PlayerComponent self, long playerId)
        {
            return self.getFromCache(playerId)
                ?? throw new Exception($"缓存中没有 {playerId} 的数据。");
        }

        /// <summary>
        /// 查询玩数据(优先缓存，其次数据库)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId"></param> 
        /// <returns>成功返回 Player 对象， 如果缓存和数据库中都没有找到匹配的对象，则抛出异常</returns>
        public static async ETTask<Player> Query(this PlayerComponent self, long playerId)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.PlayerId, playerId))
            {
                Player player = self.getFromCache(playerId)
                    ?? await self.getFromDb(playerId)
                    ?? throw new Exception($"数据库中没有 {playerId} 的数据。");
                return player;
            }
        }

        /// <summary>
        /// 从缓存中移除
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId"></param>
        public static void Remove(this PlayerComponent self, long playerId)
        {
            self.Cache.Remove(playerId);
        }


        /// <summary>
        /// 创建玩家数据缓存（优先缓存，数据库加载次之，最后注册）。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public static async ETTask<Player> Create(this PlayerComponent self, Registration a)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.PlayerId, a.Id))
            {
                var cache = self.getFromCache(a.Id);
                if (cache != null)
                    return cache;
                var db = await self.getFromDb(a.Id);
                if (db != null)
                    return db;
                var register = await self.register(a);
                return register;
            }
        }
        public static async Task<Player> AddCache(this PlayerComponent self, long playerId,Player player)
        {
            await ETTask.CompletedTask;
            player.LastTimestamp = TimeHelper.ServerNow();
            self.Cache.Add(playerId, player);
            return player;
        }

        /// <summary>
        /// 从缓存取对象
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private static Player getFromCache(this PlayerComponent self, long playerId)
        {
            if (self.Cache.TryGetValue(playerId, out Player player))
            {
                player.LastTimestamp = TimeHelper.ServerNow();
            }
            return player;
        }

        /// <summary>
        /// 创建隶属于玩家的各个组件
        /// </summary>
        /// <param name="self"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        private static async ETTask<Player> register(this PlayerComponent self, Registration a)
        {
            Player player = self.AddChildWithId<Player>(a.Id);
            try
            {
                List<Entity> registerEntities = new List<Entity>();
                foreach (Type type in self.ObjectTypes)
                {

                    var entity = player.AddComponent(type);
                    if (entity is ISerializeToEntity)
                        registerEntities.Add(entity);
                    self.GetCreateSystem(entity.GetType())?.Run(entity, a);
                }
                player.LastTimestamp = TimeHelper.ServerNow();
                self.Cache.Add(a.Id, player);

                var registerEvent = EventHelper.Create<RegisterPlayer>();
                registerEvent.Player = player;
                await EventHelper.DispatchAsync(registerEvent);
                await DBComponent.Instance.Save(a.Id, registerEntities);

                Log.Info($"[STAGE]: 新建 {a.Id} 的数据");

                return player;
            }
            catch (Exception e)
            {
                Log.Error(e);
                player.Dispose();
            }

            return null;
        }

        /// <summary>
        /// 获得玩家的登记信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private static async ETTask<RoleComponent> getRegistration(this PlayerComponent self, long playerId)
        {
            return await MongoHelper.QueryOne<RoleComponent>(playerId, (f) => f.Id == playerId, typeof(RoleComponent).Name);
        }

        /// <summary>
        /// 从数据反序列化各个组件
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private static async ETTask<Player> getFromDb(this PlayerComponent self, long playerId)
        {
            var role = await self.getRegistration(playerId);
            if (role == null)
            {
                return null;
            }
            var val = await self.load(role);
            return val;
        }

        private static async ETTask<Player> load(this PlayerComponent self, RoleComponent a)
        {
            Player player = self.AddChildWithId<Player>(a.Id);
            try
            {
                List<Entity> appendEntities = new List<Entity>();
                foreach (Type type in self.ObjectTypes)
                {
                    Entity component = await DBComponent.Instance.Query<Entity>(a.Id, type.Name);
                    if (component != null)
                    {
                        player.AddComponent(component);
                    }
                    else
                    {
                        appendEntities.Add(player.AddComponent(type));
                    }
                }

                if (appendEntities.Count > 0)
                {
                    foreach (Entity entity in appendEntities)
                    {
                        self.GetCreateSystem(entity.GetType())?.Run(entity, new Registration(a.Id, a.Nickname));
                    }

                    await DBComponent.Instance.Save(a.Id, appendEntities.Where(x => x is ISerializeToEntity).ToList());
                }

                player.LastTimestamp = TimeHelper.ServerNow();
                self.Cache.Add(a.Id, player);

                Log.Info($"[STAGE]: 载入 {a.Id} 的数据");
                return player;
            }
            catch (Exception e)
            {
                player.Dispose();
                Log.Error(e);
            }

            return null;
        }
    }
}
