using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ET;

namespace RPCBus.Server.Realm
{
    public class Account : Entity, ISerializeToEntity
    {
        public string Identity { get; set; }
        public long PlayerId { get; set; }
        public int Type { get; set; }
    }
}
