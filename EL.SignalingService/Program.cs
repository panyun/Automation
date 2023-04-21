using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using EL.SignalingService.Common;
using EL.SignalingService.Model;

namespace EL.SignalingService
{
    internal class Program
    {
        static WebSocketServer WebSocketServer;
        static ConcurrentDictionary<string, PeerRoom> PeerRooms = new();
        static async Task Main(string[] args)
        {
            WebSocketServer = new WebSocketServer(10005);
            WebSocketServer.OnMessage += WebSocketServer_OnMessage;
            WebSocketServer.OnOpen += WebSocketServer_OnOpen;
            WebSocketServer.OnClose += WebSocketServer_OnClose;
            WebSocketServer.Listen();
            Console.WriteLine("开始监听!");

            var cancellationTokenSource = new CancellationTokenSource();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => cancellationTokenSource.Cancel();
            Console.CancelKeyPress += (s, e) => cancellationTokenSource.Cancel();
            await Task.Delay(-1, cancellationTokenSource.Token).ContinueWith(t => { });
        }
        private static void WebSocketServer_OnOpen(UserToken userToken, byte[] data)
        {
            Console.WriteLine($"{DateTime.Now} 登录用户:{userToken.RemoteAddress}");
        }
        private static async void WebSocketServer_OnMessage(UserToken userToken, byte[] data)
        {
            if (data?.Length < 1)
            {
                return;
            }
            var oldData = Encoding.UTF8.GetString(data);
            try
            {
                var message = oldData.ToObj<Message<dynamic>>();
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} - id:{userToken.ID}:role:{userToken.Role} 获取到消息:{oldData}");
                switch (message.type)
                {
                    case MessageType.Ping:
                        {
                            var peerRoom = GetPeerRoom(userToken);
                            var isReady = false;
                            if (peerRoom != null)
                            {
                                isReady = peerRoom.IsReady;
                            }
                            WebSocketServer.Send(userToken, new Message<ReadyInfo>() { type = MessageType.Ping, data = new ReadyInfo() { ready = isReady } }.ToJson());
                        }
                        break;
                    case MessageType.Join:
                        {
                            userToken.ID = message.room;
                            userToken.Role = message.role;
                            JoinRoom(userToken);
                            var peerRoom = GetPeerRoom(userToken);
                            if (userToken.Role == RoleType.Mobile)
                            {
                                var target = peerRoom.GetOther(userToken);
                                if (target == null)
                                {
                                    peerRoom.AddWatingMsg(RoleType.Editor, oldData);
                                }
                                else
                                {
                                    WebSocketServer.Send(target, oldData);
                                }
                            }
                            if (peerRoom?.IsReady == true)
                            {
                                //遗留信息发送
                                var list = peerRoom.GetWatingMsg(RoleType.Editor);
                                if (list != null && list.Count > 0)
                                {
                                    foreach (var item in list)
                                    {
                                        WebSocketServer.Send(peerRoom.Editor, item);
                                    }
                                    list.Clear();
                                }
                                foreach (var item in peerRoom.GetPeers())
                                {
                                    WebSocketServer.Send(item, new Message<string>() { type = MessageType.Ready }.ToJson());
                                }
                                Console.WriteLine($"{peerRoom.RoomID} 满足两个人，开始一对一通信!");
                            }
                        }
                        break;
                    case MessageType.Start:
                        {
                            if (userToken.IsLogin)
                            {
                                foreach (var item in GetPeerRoom(userToken).GetPeers())
                                {
                                    WebSocketServer.Send(item, new Message<string>() { type = MessageType.Start }.ToJson());
                                }
                            }
                        }
                        break;
                    case MessageType.Stop:
                        {
                            if (userToken.IsLogin)
                            {
                                foreach (var item in GetPeerRoom(userToken).GetPeers())
                                {
                                    WebSocketServer.Send(item, new Message<string>() { type = MessageType.Stop }.ToJson());
                                }
                            }
                        }
                        break;
                    case MessageType.Close:
                        {
                            if (userToken.IsLogin)
                            {
                                foreach (var item in GetPeerRoom(userToken).GetPeers())
                                {
                                    try
                                    {
                                        await WebSocketServer.CloseAsync(item);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                        break;
                    case MessageType.Offer:
                        {
                            if (userToken.IsLogin)
                            {
                                var target = GetPeerRoom(userToken).GetOther(userToken);
                                WebSocketServer.Send(target, oldData);
                            }
                        }
                        break;
                    case MessageType.Answer:
                        {
                            if (userToken.IsLogin)
                            {
                                var target = GetPeerRoom(userToken).GetOther(userToken);
                                WebSocketServer.Send(target, oldData);
                            }
                        }
                        break;
                    case MessageType.Candidate:
                        {
                            if (userToken.IsLogin)
                            {
                                var target = GetPeerRoom(userToken).GetOther(userToken);
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
        private static void WebSocketServer_OnClose(UserToken userToken, byte[] data)
        {
            LeaveRoom(userToken);
            userToken.ID = null;
            userToken.Role = null;
            Console.WriteLine($"{DateTime.Now} 用户{userToken.RemoteAddress}退出");
        }
        public static PeerRoom GetPeerRoom(UserToken userToken)
        {
            if (userToken.IsLogin)
            {
                if (PeerRooms.TryGetValue(userToken.ID, out var PeerRoom))
                {
                    return PeerRoom;
                }
            }
            return null;
        }
        public static void JoinRoom(UserToken userToken)
        {
            Console.WriteLine($"JoinRoom: {userToken.ID}:{userToken.Role} {userToken.RemoteAddress}");
            var peerRoom = GetPeerRoom(userToken);
            if (peerRoom == null)
            {
                if (userToken.IsLogin)
                {
                    PeerRooms.TryAdd(userToken.ID, new PeerRoom(userToken));
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
        public static void LeaveRoom(UserToken userToken)
        {
            Console.WriteLine($"LeaveRoom: {userToken.ID}:{userToken.Role} {userToken.RemoteAddress}");
            var id = userToken.ID;
            var peerRoom = GetPeerRoom(userToken);
            peerRoom.Remove(userToken);
            foreach (var item in peerRoom.GetPeers())
            {
                WebSocketServer.Send(item, new Message<string>() { type = MessageType.Stop }.ToJson());
            }
            if (peerRoom.IsEmpty)
            {
                PeerRooms.TryRemove(id, out _);
                peerRoom.Dispose();
                Console.WriteLine($"清空房间:{id}");
            }
        }
    }
}