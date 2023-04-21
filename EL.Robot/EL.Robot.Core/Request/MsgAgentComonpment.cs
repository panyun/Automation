using EL.Async;
using EL.Robot.Core.Websocket;
using Newtonsoft.Json;
using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using UResponse = Utils.IResponse;

namespace EL.Robot.Core.Request
{
    public class MsgAgentComonpment
    {
        public static MsgAgentComonpment Instance = new MsgAgentComonpment();
        /// <summary>
        /// 最近目标用户
        /// </summary>
        public long TargetClientId { get; set; }
        public long SelfClientId { get; set; }
        public readonly Dictionary<int, RpcInfo> RequestCallbacks = new Dictionary<int, RpcInfo>();
    }
    public static class MsgAgentComonpmentSystem
    {
        public static void OnRead(this MsgAgentComonpment self, Utils.IMessage msgAgent)
        {
            if (msgAgent is C2G_MsgAgent msgRequset)
            {
                var para = msgRequset.Content;
                var opcode = ushort.Parse(para.Substring(0, 5));

                var msg = para.Substring(5, para.Length - 5);
                if (opcode == 62002)
                {
                    var robot = EL.Boot.GetComponent<RobotComponent>();
                    var flowInfo = JsonHelper.FromJson<FlowRequestJson>(msg);
                    //robot.RpaJson = flowInfo.Flow.ToString().Replace("\r\n", "").Replace(" ", "");
                    robot.RpaJson = JsonHelper.ToJson(flowInfo.Flow);
                }
                var type = OpcodeTypeComponent.GetType(opcode);
                try
                {
                    var message = JsonHelper.FromJson(type, para.Substring(5, para.Length - 5));
                    if (message is IAgentRequset request)
                    {
                        request.RpcId = msgRequset.RpcId;
                        request.AgentRequest = msgRequset;
                        Handle(opcode, message, RoboatServerComponent.CurrentTChannel).Coroutine();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }

            }
            if (msgAgent is G2C_MsgAgent msgResponse)
            {
                var para = msgResponse.Content;
                var opcode = ushort.Parse(para.Substring(0, 5));
                var msg = para.Substring(5, para.Length - 5);
                var type = OpcodeTypeComponent.GetType(opcode);
                var message = JsonHelper.FromJson(type, para.Substring(5, para.Length - 5));
                if (message is UResponse response)
                {
                    if (!self.RequestCallbacks.TryGetValue(response.RpcId, out var action))
                        return;
                    self.RequestCallbacks.Remove(response.RpcId);
                    action.Tcs.SetResult(response);
                }
            }
        }
        public class FlowRequestJson
        {
            public Flow Flow { get; set; }
            public bool IsDebug { get; set; }
            public int RpcId { get; set; }
        }
        public class Flow
        {
            public long id { get; set; }
            public string name { get; set; }
            public List<Steps> steps { get; set; }
        }
        public class Steps
        {
            public long id { get; set; }
            public string name { get; set; }
            public string componentName { get; set; }
            public List<Steps> steps { get; set; }
            [JsonProperty(PropertyName = "switch")]
            public List<Steps[]> Switch { get; set; }
            public bool debug { get; set; }
            public bool ignore { get; set; }
        }
        public static async ELTask OnReply(this MsgAgentComonpment self, C2G_MsgAgent g2C_MsgAgent, UResponse response)
        {
            await ELTask.CompletedTask;
            var msg = OpcodeTypeComponent.GetOpcode(response.GetType()) + JsonHelper.ToJson(response);
            await RoboatServerComponent.CurrentTChannel.Send(new G2C_MsgAgent()
            {
                RpcId = g2C_MsgAgent.RpcId,
                Content = msg,
                TargetClientId = g2C_MsgAgent.TargetClientId,
                SelfClientId = g2C_MsgAgent.SelfClientId
            });
        }
        public static async ELTask Send(this MsgAgentComonpment self, Utils.IRequest request)
        {
            await ELTask.CompletedTask;
            var msg = OpcodeTypeComponent.GetOpcode(request.GetType()) + JsonHelper.ToJson(request);
            await RoboatServerComponent.CurrentTChannel.Send(new C2G_MsgAgent()
            {
                Content = msg,
                TargetClientId = MsgAgentComonpment.Instance.TargetClientId,
                SelfClientId = MsgAgentComonpment.Instance.SelfClientId
            });
        }
        public static int RpcId = 0;
        public static async ELTask<Utils.IResponse> Call(this MsgAgentComonpment self, Utils.IRequest request)
        {
            await ELTask.CompletedTask;
            int rpcId = ++RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            self.RequestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;
            var msg = OpcodeTypeComponent.GetOpcode(request.GetType()) + JsonHelper.ToJson(request);
            C2G_MsgAgent c2G_MsgAgent = new C2G_MsgAgent();
            c2G_MsgAgent.SelfClientId = MsgAgentComonpment.Instance.TargetClientId;
            c2G_MsgAgent.TargetClientId = MsgAgentComonpment.Instance.SelfClientId;
            var agent = (G2C_MsgAgent)await RoboatServerComponent.CurrentTChannel.Call(c2G_MsgAgent);
            self.OnRead(agent);
            return await rpcInfo.Tcs;
        }
        public static async ELVoid Handle(ushort opcode, object message, WChannel channel)
        {
            List<W_IMHandler> actions;
            if (!MessageHandlerComponent.W_Handlers.TryGetValue(opcode, out actions))
            {
                //MessageBox.Show($"消息没有处理: {opcode} {message}");
                return;
            }

            foreach (W_IMHandler ev in actions)
            {
                try
                {
                    ev.Handle(channel, message);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
