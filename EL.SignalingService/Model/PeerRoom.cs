using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.SignalingService.Model
{
    /// <summary>
    /// 点对点连接的Room
    /// </summary>
    public class PeerRoom : IDisposable
    {
        public PeerRoom(UserToken userToken)
        {
            RoomID = userToken.ID;
            Add(userToken);
            WatingMsg = new Dictionary<string, List<string>>() { { RoleType.Editor, new List<string>() }, { RoleType.Mobile, new List<string>() } };
        }
        public string RoomID { get; set; }
        public UserToken Mobile { get; set; }
        public UserToken Editor { get; set; }
        public Dictionary<string, List<string>> WatingMsg { get; private set; }
        public bool IsReady { get { return Mobile != null && Editor != null; } }
        public bool IsEmpty { get { return Mobile == null && Editor == null; } }
        public UserToken Add(UserToken userToken)
        {
            switch (userToken.Role)
            {
                case RoleType.Editor:
                    {
                        var oldEditor = Editor;
                        Editor = userToken;
                        return oldEditor;
                    }
                case RoleType.Mobile:
                    {
                        var oldMobile = Mobile;
                        Mobile = userToken;
                        return oldMobile;
                    }
            }
            return null;
        }
        public void Remove(UserToken userToken)
        {
            switch (userToken.Role)
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
        public void AddWatingMsg(string type, string msg)
        {
            WatingMsg[type].Add(msg);
        }
        public List<string> GetWatingMsg(string type)
        {
            return WatingMsg[type];
        }
        public List<UserToken> GetPeers()
        {
            var list = new List<UserToken>();
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
        public UserToken GetOther(UserToken userToken)
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
