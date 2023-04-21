using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server
{
    namespace EventType
    {
        public struct AppStart
        {
        }
        // 注册玩家
        public struct RegisterPlayer
        {
            public Client.Player Player;
        }

        // 在线玩家进入游戏
        public struct EnterGame
        {
            public Client.PlayerActor Actor;
            public Client.Player Player;
        }

        // 在线玩家离开游戏
        public struct LeaveGame
        {
            public Client.PlayerActor Actor;
            public Client.Player Player;
        }
    }
}
