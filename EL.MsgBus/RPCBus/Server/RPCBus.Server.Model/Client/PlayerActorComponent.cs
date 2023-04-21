using System.Collections.Generic;
using ET;

namespace RPCBus.Server.Client
{
    public class PlayerActorComponent : Entity
    {
        public Dictionary<long, PlayerActor> Actors = new Dictionary<long, PlayerActor>();
    }
}
