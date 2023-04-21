using EL.Async;
using EL.Robot.Core.Request;
using Fleck;
using Protos;
using System.Net.WebSockets;
using Utils;

namespace EL.Robot.Core
{
    public class WebSocketGateComponent : Entity
    {
        //public Dictionary<long, LoginRequest> Clients { get; set; }
        public WebSocketServer WebSocket { get; set; }

    }
    public class WebSocketGateComonentAwake : AwakeSystem<WebSocketGateComponent>
    {
        public override void Awake(WebSocketGateComponent self)
        {
            //self.Clients = new Dictionary<long, LoginRequest>();
            self.Main();
        }
    }
    public static class WebSocketGateComponentSystem
    {

        public static async void Main(this WebSocketGateComponent self)
        {
            await ELTask.CompletedTask;
            Log.Info("ws://0.0.0.0:11001");
            self.WebSocket = new WebSocketServer("ws://0.0.0.0:11001");
            self.WebSocket.Start(async socket =>
            {
                await ELTask.CompletedTask;
                socket.OnOpen = () =>
                {
                    if (RoboatServerComponent.CurrentUser == null) return;
                    LocalMsgRequest localMsgRequest = new()
                    {
                        ClientId = RoboatServerComponent.CurrentUser.ClientId,
                        Content = "",
                        RpcId = ++WChannel.RpcId
                    };
                    var msg = Utils.OpcodeTypeComponent.GetOpcode(localMsgRequest.GetType()) + JsonHelper.ToJson(localMsgRequest);
                    socket.Send(msg);
                    //self.Clients.Add(socket);
                };
                socket.OnClose = () =>
                {
                //foreach (var item in self.Clients)
                //{

                //    item.Value.WebSocket
                //}
            };
                socket.OnMessage = async message =>
                {
                //Log.Info(message);
                //self.RouterAsync(message, socket);
                //await socket.Send(message);
            };
            });
        }
        public static async Task RouterAsync(this WebSocketGateComponent self, string msg, IWebSocketConnection webSocket)
        {

            ushort opcode = default;
            try
            {
                opcode = ushort.Parse(msg.Substring(0, 4));
            }
            catch (Exception)
            {
                throw new Exception("消息格式不正确！");
            }
            Type type = OpcodeTypeComponent.GetType(opcode);
            var entity = JsonHelper.FromJson(type, msg.Substring(4));
            if (type.Name == nameof(LoginRequest))
            {
                var loginRequest = entity as LoginRequest;
                var enter = await loginRequest.LoginAsync();
                //loginRequest.WebSocket = webSocket;
                //self.Clients.Add(loginRequest.UserId, loginRequest);
            }
        }
    }

}
