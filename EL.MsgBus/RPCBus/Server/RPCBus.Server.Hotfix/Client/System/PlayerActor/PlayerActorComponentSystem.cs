using ET;

namespace RPCBus.Server.Client
{
    [ObjectSystem]
    class PlayerActorComponentAwakeSystem : AwakeSystem<PlayerActorComponent>
    {
        public override void Awake(PlayerActorComponent self)
        {
            TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 2000, () =>
            {
                Console.WriteLine("tst");
            });
            Log.Info($"[STAGE]: Awake()");
        }
    }

    [ObjectSystem]
    class PlayerActorComponentDestroySystem : DestroySystem<PlayerActorComponent>
    {
        public override void Destroy(PlayerActorComponent self)
        {
            Log.Info("[STAGE]： Destroy()");
        }
    }

    public static class PlayerActorComponentSystem
    {
        public static void Add(this PlayerActorComponent self, PlayerActor actor)
        {
            self.Actors.Add(actor.ClientId, actor);
        }

        public static bool Exists(this PlayerActorComponent self, long playerId)
        {
            return self.Actors.ContainsKey(playerId);
        }

        public static PlayerActor Get(this PlayerActorComponent self, long playerId)
        {
            self.Actors.TryGetValue(playerId, out PlayerActor playerActor);
            return playerActor;
        }

        public static PlayerActor Remove(this PlayerActorComponent self, long playerId)
        {
            self.Actors.Remove(playerId, out PlayerActor playerActor);
            return playerActor;
        }
    }
}
