using EL.PIPSystemServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EL.SignalingService.Model
{
    /// <summary>
    /// 点对点连接的Room
    /// </summary>
    public class Room : IDisposable
    {
        public Room(UserToken userToken)
        {
            RoomID = userToken.ID;
            Add(userToken);
        }
        public string RoomID { get; set; }
        public UserToken Client { get; set; }
        public UserToken Server { get; set; }
        public bool IsReady { get { return Client != null && Server != null; } }
        public bool IsEmpty { get { return Client == null && Server == null; } }
        public UserToken Add(UserToken userToken)
        {
            switch (userToken.Role)
            {
                case RoleType.Server:
                    {
                        var oldServer = Server;
                        Server = userToken;
                        return oldServer;
                    }
                case RoleType.Client:
                    {
                        var oldClient = Client;
                        Client = userToken;
                        return oldClient;
                    }
            }
            return null;
        }
        public void Remove(UserToken userToken)
        {
            switch (userToken.Role)
            {
                case RoleType.Server:
                    {
                        Server = null;
                    }
                    break;
                case RoleType.Client:
                    {
                        Client = null;
                    }
                    break;
            }
        }
        public List<UserToken> GetPeers()
        {
            var list = new List<UserToken>();
            if (Server != null)
            {
                list.Add(Server);
            }
            if (Client != null)
            {
                list.Add(Client);
            }
            return list;
        }
        /// <summary>
        /// 获取另外一方
        /// </summary>
        public UserToken GetOther(UserToken userToken)
        {
            if (userToken.Role == RoleType.Server)
            {
                return Client;
            }
            else
            {
                return Server;
            }
        }

        public void Dispose()
        {
            RoomID = null;
            Client = null;
            Server = null;
        }
    }
}
