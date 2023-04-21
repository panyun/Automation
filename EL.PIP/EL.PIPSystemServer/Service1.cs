using EL.PIPSystemServer.Common;
using EL.SignalingService.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EL.PIPSystemServer
{
    partial class Service1 : ServiceBase
    {
        public int Port;
        static ConcurrentDictionary<string, Room> Rooms = new ConcurrentDictionary<string, Room>();
        public Service1(int port)
        {
            InitializeComponent();
            Port = port;
        }
        private WebSocketServer WebSocketServer;
        protected override void OnStart(string[] args)
        {
            Start();
        }
        public void Start()
        {
            try
            {
                //获取一个有效的端口
                if (!int.TryParse(EnvironmentVarialbeHelper.Get("PIPSystemServer"), out var port))
                {
                    port = -1;
                }
                Port = PortManage.GetPort(Port, port);
                EnvironmentVarialbeHelper.Set("PIPSystemServer", Port.ToString());
                Log.Write($"服务启动，端口:{Port} 开启服务监听!");
                WebSocketServer = new WebSocketServer(Port);
                WebSocketServer.OnMessage += WebSocketServer_OnMessage; ;
                WebSocketServer.OnOpen += WebSocketServer_OnOpen;
                WebSocketServer.OnClose += WebSocketServer_OnClose;
                WebSocketServer.Listen();
                Log.Write($"服务启动:{WebSocketServer.Start}");
            }
            catch (Exception ex)
            {
                Log.Write($"服务异常停止:{ex.Message} - {ex.StackTrace}");
                this.Stop();
                throw;
            }
        }



        private void WebSocketServer_OnOpen(UserToken userToken, byte[] data)
        {
            Log.Write($"登录用户:{userToken.RemoteAddress}");
        }

        private void WebSocketServer_OnMessage(UserToken userToken, byte[] data)
        {
            if (data?.Length < 1)
            {
                return;
            }
            var oldData = Encoding.UTF8.GetString(data);
            try
            {
                var message = oldData.ToObj<Message<dynamic>>();
                Log.Write($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} - id:{userToken.ID}:获取到消息:{oldData}");
                switch (message.type)
                {
                    case MessageType.Ping:
                        {
                            if (userToken.IsLogin)
                            {
                                WebSocketServer.Send(userToken, new Message<dynamic>() { type = MessageType.Ping }.ToJson());
                            }
                        }
                        break;
                    case MessageType.Join:
                        {
                            userToken.ID = message.room;
                            userToken.Role = message.role;
                            JoinRoom(userToken);
                            if (Rooms.TryGetValue(userToken.ID, out var room))
                            {
                                if (room.IsReady)
                                {
                                    foreach (var item in room.GetPeers())
                                    {
                                        WebSocketServer.Send(item, new Message<dynamic>() { type = MessageType.Ready }.ToJson());
                                    }
                                }
                            }
                        }
                        break;
                    case MessageType.Data:
                        {
                            if (userToken.IsLogin)
                            {
                                var target = GetRoom(userToken).GetOther(userToken);
                                WebSocketServer.Send(target, oldData);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                //TODO 正式环境需要移除。
                WebSocketServer.Send(userToken, $"source:{oldData} error:{ex.Message} static:{ex.StackTrace}");
            }
        }
        private void WebSocketServer_OnClose(UserToken userToken, byte[] data)
        {
            LeaveRoom(userToken);
            userToken.ID = null;
            userToken.Role = null;
            Log.Write($"退出用户:{userToken.RemoteAddress}");
        }
        public Room GetRoom(UserToken userToken)
        {
            if (userToken.IsLogin)
            {
                if (Rooms.TryGetValue(userToken.ID, out var PeerRoom))
                {
                    return PeerRoom;
                }
            }
            return null;
        }
        public void JoinRoom(UserToken userToken)
        {
            Console.WriteLine($"JoinRoom: {userToken.ID}:{userToken.Role} {userToken.RemoteAddress}");
            var peerRoom = GetRoom(userToken);
            if (peerRoom == null)
            {
                if (userToken.IsLogin)
                {
                    Rooms.TryAdd(userToken.ID, new Room(userToken));
                }
            }
            else
            {
                var result = peerRoom?.Add(userToken);
                if (result != null)
                {
                    try
                    {
                        _ = WebSocketServer.CloseAsync(result);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        public void LeaveRoom(UserToken userToken)
        {
            Console.WriteLine($"LeaveRoom: {userToken.ID}:{userToken.Role} {userToken.RemoteAddress}");
            var id = userToken.ID;
            var peerRoom = GetRoom(userToken);
            peerRoom.Remove(userToken);
            foreach (var item in peerRoom.GetPeers())
            {
                WebSocketServer.Send(item, new Message<string>() { type = MessageType.Stop }.ToJson());
            }
            if (peerRoom.IsEmpty)
            {
                Rooms.TryRemove(id, out _);
                peerRoom.Dispose();
                Console.WriteLine($"清空房间:{id}");
            }
        }

        protected override void OnStop()
        {
            Log.Write("服务停止，开始关闭服务");
            WebSocketServer?.Close();
            foreach (var item in Rooms)
            {
                try
                {
                    item.Value.Dispose();
                }
                catch (Exception)
                {
                }
            }
            Rooms.Clear();
            Log.Write("关闭服务");
        }
    }
}
