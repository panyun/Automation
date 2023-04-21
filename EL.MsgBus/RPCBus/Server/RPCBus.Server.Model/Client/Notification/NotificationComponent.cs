using ET;
using System.Collections.Generic;

namespace RPCBus.Server.Client
{
    //[PlayerObject]
    public class NotificationComponent : Entity, ISerializeToEntity
    {
        public HashSet<long> Receipts = new HashSet<long>();
    }
}
