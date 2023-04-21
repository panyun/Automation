using MongoDB.Bson;
using RPCBus.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Model.P2P
{
    /// <summary>
    /// 点对点连接的Room
    /// </summary>
    public class PeerRoom : IDisposable
    {
        public PeerRoom(PeerRoomInfo peerRoomInfo)
        {
            RoomID = peerRoomInfo.Room;
            Add(peerRoomInfo);
            WatingMsg = new Dictionary<string, List<P2PMessage>>() { { RoleType.Editor, new List<P2PMessage>() }, { RoleType.Mobile, new List<P2PMessage>() } };
        }
        public string RoomID { get; set; }
        public PeerRoomInfo Mobile { get; set; }
        public PeerRoomInfo Editor { get; set; }
        public Dictionary<string, List<P2PMessage>> WatingMsg { get; private set; }
        public bool IsReady { get { return Mobile != null && Editor != null; } }
        public bool IsEmpty { get { return Mobile == null && Editor == null; } }
        public PeerRoomInfo Add(PeerRoomInfo peerRoomInfo)
        {
            switch (peerRoomInfo.Role)
            {
                case RoleType.Editor:
                    {
                        var oldEditor = Editor;
                        Editor = peerRoomInfo;
                        return oldEditor;
                    }
                case RoleType.Mobile:
                    {
                        var oldMobile = Mobile;
                        Mobile = peerRoomInfo;
                        return oldMobile;
                    }
            }
            return null;
        }
        public void Remove(PeerRoomInfo peerRoomInfo)
        {
            switch (peerRoomInfo.Role)
            {
                case RoleType.Editor:
                    {
                        WatingMsg[RoleType.Editor].Clear();
                        Editor = null;
                    }
                    break;
                case RoleType.Mobile:
                    {
                        WatingMsg[RoleType.Mobile].Clear();
                        Mobile = null;
                    }
                    break;
            }
        }
        public void AddWatingMsg(string type, P2PMessage msg)
        {
            if (msg != null)
            {
                WatingMsg[type].Add(msg);
            }
        }
        public List<P2PMessage> GetWatingMsg(string type)
        {
            return WatingMsg[type];
        }
        public List<PeerRoomInfo> GetPeers()
        {
            var list = new List<PeerRoomInfo>();
            if (Editor != null)
            {
                list.Add(Editor);
            }
            if (Mobile != null)
            {
                list.Add(Mobile);
            }
            return list;
        }
        /// <summary>
        /// 获取另外一方
        /// </summary>
        public PeerRoomInfo GetOther(PeerRoomInfo userToken)
        {
            if (userToken.Role == RoleType.Editor)
            {
                return Mobile;
            }
            else
            {
                return Editor;
            }
        }
      
        public void Dispose()
        {
            RoomID = null;
            Mobile = null;
            Editor = null;
            WatingMsg.Clear();
            WatingMsg = null;
        }
    }
}
