using ET;

namespace RPCBus.Server.Gate
{
    [ObjectSystem]
    class UserComponentAwakeSystem : AwakeSystem<AgentComponent>
    {
        public override void Awake(AgentComponent self)
        {
            Log.Info("[GATE]： Awake()");
        }
    }

    [ObjectSystem]
    class UserComponentDestroySystem : DestroySystem<AgentComponent>
    {
        public override void Destroy(AgentComponent self)
        {
            Log.Info("[GATE]： Destroy()");
        }
    }

    /// <summary>
    /// 维护所有在线用户
    /// </summary>
    public static class AgentComponentSystem
    {
        public static void Add(this AgentComponent self, Agent agent)
        {
            //新增相同的机器人信息，要把之前的直接移除掉
            foreach (var item in self.Users.Where(t => t.Value.AccountId == agent.AccountId && t.Value.ClientId == agent.ClientId))
            {
                item.Value.Session.Dispose();
                Log.Info($"Agent Remove： {item.Key}-{agent.AccountId}-{agent.ClientId}");
                self.Users.Remove(item.Key);
            }        
            self.Users.Add(agent.SessionId, agent);
        }

        public static Agent Get(this AgentComponent self, string id)
        {
            self.Users.TryGetValue(id, out Agent value);
            return value;
        }

        public static void Remove(this AgentComponent self, string id)
        {
            self.Users.Remove(id);
        }
    }
}
