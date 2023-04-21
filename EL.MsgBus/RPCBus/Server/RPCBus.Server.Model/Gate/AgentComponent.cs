using System.Collections.Generic;
using ET;

namespace RPCBus.Server.Gate
{
    /// <summary>
    /// 保存所有 实体Id 到 Agent 的影射
    /// </summary>
    public class AgentComponent : Entity
    {
        public Dictionary<string, Agent> Users = new Dictionary<string, Agent>();
    }
}
