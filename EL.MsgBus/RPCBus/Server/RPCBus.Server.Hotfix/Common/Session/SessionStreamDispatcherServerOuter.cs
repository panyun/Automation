using System;
using System.IO;
using System.Text;
using ET;
using RPCBus.Protos;
using RPCBus.Server.Client;
using RPCBus.Server.Gate;
using RPCBus.Server.Model.P2P;

namespace RPCBus.Server
{
    [SessionStreamDispatcher(SessionStreamDispatcherType.SessionStreamDispatcherServerOuter)]
    public class SessionStreamDispatcherServerOuter : ISessionStreamDispatcher
    {
        public void Dispatch(Session session, MemoryStream memoryStream)
        {
            var tserver = (session.AService as TService);
            var channel = tserver.Get(session.Id);
            ushort opcode = default;
            object message = default;
            Type type = default;
            if (channel.IsWebSocket)
            {
                string para = default;
                try
                {
                    para = System.Text.Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                    if (para.Length > 10000)
                    {
                        Log.Debug($"para {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 长度: {para.Length}");
                    }
                    else
                    {
                        Log.Debug($"para {para.Length}:{para}");
                    }
                    opcode = ushort.Parse(para.Substring(0, 5));
                    var s = para.Substring(5, para.Length - 5);
                    type = OpcodeTypeComponent.Instance.GetType(opcode);
                    if (opcode == Protos.OuterOpcode.P2PMessage)
                    {
                        _ = DispatchP2PMessageAsync(session, opcode, type, s);
                        return;
                    }
                    message = JsonHelper.FromJson(type, para.Substring(5, para.Length - 5));
                }
                catch (Exception ex)
                {
                    Log.Error($"websocket 协议出错！para: {para} ex:{ex.Message}");
                    return;
                }
            }
            else
            {
                opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.KcpOpcodeIndex);
                type = OpcodeTypeComponent.Instance.GetType(opcode);
                message = MessageSerializeHelper.DeserializeFrom(opcode, type, memoryStream);
            }

            if (message is IAgentResponse)
            {
                OpcodeHelper.LogMsg(session.DomainZone(), opcode, message);
                DispatchAsync(session, opcode, message).Coroutine();
                return;
            }
            if (message is IResponse response)
            {
                session.OnRead(opcode, response);
                return;
            }
            if (opcode != (ushort)Protos.OuterOpcode.C2G_Ping && opcode != (ushort)Protos.OuterOpcode.G2C_Ping)
            {
                OpcodeHelper.LogMsg(session.DomainZone(), opcode, message);
            }
            DispatchAsync(session, opcode, message).Coroutine();
        }

        public async ETVoid DispatchAsync(Session session, ushort opcode, object message)
        {
            // 根据消息接口判断是不是Actor消息，不同的接口做不同的处理
            switch (message)
            {
                case IAgentRequest agentMessage:
                    {
                        long targetId = default;
                        long selfId = default;
                        try
                        {
                            var agent = (message as C2G_MsgAgent);
                            targetId = agent.TargetClientId;
                            selfId = agent.SelfClientId;
                            var agentMsg = session.DomainScene().GetComponent<AgentComponent>().Users.Values.Where(x => x.ClientId == targetId).First();
                            var sendEntity = session.DomainScene().GetComponent<NetKcpComponent>().Children.Values.Where(x => x.Id == agentMsg.SessionTServiceId).First();
                            var sendSession = sendEntity as Session;
                            ThreadSynchronizationContext.Instance.PostNext(() =>
                            {
                                sendSession.Send(message as IMessage);
                            });
                        }
                        catch (Exception ex)
                        {
                            Log.Debug($"Request Agent：{selfId} => {targetId} Not Found!");
                            var response = Activator.CreateInstance<G2C_MsgAgent>();
                            response.TargetClientId = targetId;
                            response.SelfClientId = selfId;
                            response.Message = $"TargetId {targetId} Not Found!";
                            response.Error = 404;
                            response.RpcId = agentMessage.RpcId;
                            ThreadSynchronizationContext.Instance.PostNext(() =>
                            {
                                session.Send(response);
                            });
                            Log.Error(ex);
                        }
                    }
                    break;
                case IAgentResponse agentResponse:
                    try
                    {
                        var playerId = (message as G2C_MsgAgent).SelfClientId;
                        var agentMsg = session.DomainScene().GetComponent<AgentComponent>().Users.Values.Where(x => x.ClientId == playerId).First();
                        var sendEntity = session.DomainScene().GetComponent<NetKcpComponent>().Children.Values.Where(x => x.Id == agentMsg.SessionTServiceId).First();
                        var sendSession = sendEntity as Session;
                        ThreadSynchronizationContext.Instance.PostNext(() =>
                        {
                            sendSession.Send(message as IMessage);
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Debug($"Response {agentResponse.TargetClientId} => {agentResponse.SelfClientId} Not Found!");
                        Log.Error(ex);
                    }
                    break;
                case IActorLocationRequest actorLocationRequest: // gate session 收到 actor rpc 消息，先向 actor 发送rpc请求，再将请求结果返回客户端
                    {

                        break;
                    }
                case IActorLocationMessage actorLocationMessage:
                    {

                        break;
                    }

                case IActorRequest actorRequest:  // 分发IActorRequest消息，目前没有用到，需要的自己添加
                    {
                        Gate.Agent agent = session.GetComponent<Gate.SessionAgentComponent>().Agent;
                        int rpcId = actorRequest.RpcId; // 这里要保存客户端的rpcId
                        long instanceId = session.InstanceId;
                        IResponse response = await ActorMessageSenderComponent.Instance.Call(agent.PlayerActorId, actorRequest);
                        response.RpcId = rpcId;
                        // session 可能已经断开了，所以这里需要判断
                        if (session.InstanceId == instanceId)
                        {
                            session.Reply(response);
                        }
                        break;
                    }
                case IActorMessage actorMessage:  // 分发IActorMessage消息，目前没有用到，需要的自己添加
                    {
                        // 将接受到的 IActorMessage 转发到 Stage 进程
                        Gate.Agent agent = session.GetComponent<Gate.SessionAgentComponent>().Agent;
                        ActorMessageSenderComponent.Instance.Send(agent.PlayerActorId, actorMessage);
                        break;
                    }

                default:
                    {
                        // 非Actor消息
                        MessageDispatcherComponent.Instance.Handle(session, opcode, message);
                        break;
                    }
            }
        }
        public async ETVoid DispatchP2PMessageAsync(Session session, ushort opcode, Type type, string data)
        {
            //接收到p2p请求的消息，对其进行处理
            try
            {
                var message = JsonHelper.FromJson(type, data) as P2PMessage;
                if (message == null)
                {
                    return;
                }
                var agent = session.GetComponent<SessionAgentComponent>().Agent;
                var PeerRoomComponent = session.DomainScene().GetComponent<PeerRoomComponent>();
                var loginInfo = session.GetComponent<PeerRoomInfo>();
                switch (message.Type)
                {
                    case MessageType.Join:
                        {
                            var PeerRoomInfo = session.AddComponent<PeerRoomInfo>();
                            PeerRoomInfo.Room = message.Room;
                            PeerRoomInfo.Role = message.Role;
                            PeerRoomInfo.Session = session;
                            PeerRoomInfo.SessionId = agent.SessionId;
                            PeerRoomComponent.JoinRoom(PeerRoomInfo);
                            var peerRoom = PeerRoomComponent.GetPeerRoom(PeerRoomInfo);
                            if (PeerRoomInfo.Role == RoleType.Mobile)
                            {
                                var target = peerRoom.GetOther(PeerRoomInfo);
                                if (target == null)
                                {
                                    peerRoom.AddWatingMsg(RoleType.Editor, message);
                                }
                                else
                                {
                                    target.Send(message);
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
                                        peerRoom.Editor.Send(item);
                                    }
                                    list.Clear();
                                }
                                foreach (var item in peerRoom.GetPeers())
                                {
                                    item.Send(new P2PMessage() { Type = MessageType.Ready });
                                }
                                Console.WriteLine($"{peerRoom.RoomID} 满足两个人，开始一对一通信!");
                            }
                        }
                        break;
                    case MessageType.Start:
                        {
                            if (loginInfo?.IsLogin == true)
                            {
                                foreach (var item in PeerRoomComponent.GetPeerRoom(loginInfo).GetPeers())
                                {
                                    item.Send(new P2PMessage() { Type = MessageType.Start });
                                }
                            }
                        }
                        break;
                    case MessageType.Stop:
                        {
                            if (loginInfo?.IsLogin == true)
                            {
                                foreach (var item in PeerRoomComponent.GetPeerRoom(loginInfo).GetPeers())
                                {
                                    item.Send(new P2PMessage() { Type = MessageType.Stop });
                                }
                            }
                        }
                        break;
                    case MessageType.Close:
                        {
                            if (loginInfo?.IsLogin == true)
                            {
                                foreach (var item in PeerRoomComponent.GetPeerRoom(loginInfo).GetPeers())
                                {
                                    item.Send(new P2PMessage() { Type = MessageType.Close });
                                }
                            }
                        }
                        break;
                    case MessageType.CheckOut:
                        {
                            if (loginInfo?.IsLogin == true)
                            {
                                PeerRoom peerRoom = PeerRoomComponent.GetPeerRoom(loginInfo);
                                foreach (var item in peerRoom.GetPeers())
                                {
                                    item.Send(new P2PMessage() { Type = MessageType.CheckOut });
                                }
                                PeerRoomComponent.CheckOutRoom(loginInfo);
                            }
                        }
                        break;
                    case MessageType.Offer:
                        {
                            if (loginInfo?.IsLogin == true)
                            {
                                var target = PeerRoomComponent.GetPeerRoom(loginInfo).GetOther(loginInfo);
                                target.Send(message);
                            }
                        }
                        break;
                    case MessageType.Answer:
                        {
                            if (loginInfo?.IsLogin == true)
                            {
                                var target = PeerRoomComponent.GetPeerRoom(loginInfo).GetOther(loginInfo);
                                target.Send(message);
                            }
                        }
                        break;
                    case MessageType.Candidate:
                        {
                            if (loginInfo?.IsLogin == true)
                            {
                                var target = PeerRoomComponent.GetPeerRoom(loginInfo).GetOther(loginInfo);
                                target.Send(message);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"P2P 协议出错！para: {data} ex:{ex.Message}");
            }
        }
    }
}