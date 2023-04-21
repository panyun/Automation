using ET;
using MongoDB.Bson;
using RPCBus.Protos;
using RPCBus.Server.Gate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Model.P2P
{
    /// <summary>
    /// 保存所有 实体Id 到 Agent 的影射
    /// </summary>
    public class PeerRoomComponent : Entity
    {
        public ConcurrentDictionary<string, PeerRoom> PeerRooms = new();
        public ConcurrentDictionary<string, PeerRoomInfo> SessionPeer = new();
        public PeerRoom GetPeerRoom(PeerRoomInfo userToken)
        {
            if (userToken.IsLogin)
            {
                if (PeerRooms.TryGetValue(userToken.Room, out var PeerRoom))
                {
                    return PeerRoom;
                }
            }
            return null;
        }
        public void JoinRoom(PeerRoomInfo userToken)
        {
            Console.WriteLine($"JoinRoom: {userToken.Room}:{userToken.Role}");
            var peerRoom = GetPeerRoom(userToken);
            if (peerRoom == null)
            {
                if (userToken.IsLogin)
                {
                    var peer = new PeerRoom(userToken);
                    PeerRooms.TryAdd(userToken.Room, peer);
                    SessionPeer.TryAdd(userToken.SessionId, userToken);
                }
            }
            else
            {
                var result = peerRoom?.Add(userToken);
                SessionPeer.TryAdd(userToken.SessionId, userToken);
                if (result != null)
                {
                    try
                    {
                        result.Send(new P2PMessage() { Type = MessageType.Close });
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        public void LeaveRoom(PeerRoomInfo userToken)
        {
            Console.WriteLine($"LeaveRoom: {userToken.Room}:{userToken.Role}");
            var id = userToken.Room;
            var sessionid = userToken.SessionId;
            var peerRoom = GetPeerRoom(userToken);
            peerRoom.Remove(userToken);
            foreach (var item in peerRoom.GetPeers())
            {
                item.Send(new P2PMessage() { Type = MessageType.Stop });
            }
            if (peerRoom.IsEmpty)
            {
                PeerRooms.TryRemove(id, out _);
                SessionPeer.TryRemove(sessionid, out _);
                peerRoom.Dispose();
                Console.WriteLine($"清空房间:{id}");
            }
        }
        public void CheckOutRoom(PeerRoomInfo userToken)
        {
            var room = userToken.Room;
            Console.WriteLine($"CheckOutRoom: {userToken.Room}");
            var peerRoom = GetPeerRoom(userToken);
            //移除session
            foreach (var item in peerRoom.GetPeers())
            {
                item.Session.RemoveComponent<PeerRoomInfo>();
                SessionPeer.TryRemove(item.SessionId, out _);
            }
            PeerRooms.TryRemove(room, out _);
            peerRoom.Dispose();
            Console.WriteLine($"退房:{room}");
        }
        public void LeaveRoom(string SessionId)
        {
            var peerRoomInfo = GetPeerRoomInfoBySessionId(SessionId);
            if (peerRoomInfo != null)
            {
                LeaveRoom(peerRoomInfo);
            }
        }
        public PeerRoomInfo GetPeerRoomInfoBySessionId(string SessionId)
        {
            SessionPeer.TryGetValue(SessionId, out var PeerRoom);
            return PeerRoom;
        }
    }
}
