using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Model.P2P
{
    public class PeerRoomInfo : Entity
    {
        public string Room { get; set; }
        public string Role { get; set; }
        public Session Session { get; set; }
        public string SessionId { get; set; }
        public void Send(IMessage message)
        {
            if (message == null)
            {
                return;
            }
            Session?.Send(message);
        }
        public bool IsLogin { get { return !string.IsNullOrEmpty(Room) && !string.IsNullOrEmpty(Role) && Session != null; } }
    }
}
