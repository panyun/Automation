using EL.Async;
using EL.Http;
using EL.Robot.Component;
using EL.Robot.Component.PIP;
using EL.Robot.Core.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Protos;
using System.Text;
using System.Windows.Media.Animation;
using Utils;

namespace EL.Robot.Core.Handler
{
    [MessageHandler]
    public class RunRequestHandler : W_AMHandler<RunRequest>
    {
        protected override async ELTask Run(WChannel channel, RunRequest message)
        {
            var robot = Boot.GetComponent<RobotComponent>();
            if (robot.State == 1)
            {
                var flowName = robot.GetComponent<FlowComponent>().MainFlow.Name;
                MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new RunResponse()
                {
                    Error = 600,
                    Message = $"当前[{flowName}]流程正在执行中！",
                    RpcId = message.RpcId
                }).Coroutine();
                return;
            }
            //调用机器人入口
            _ = Task.Run(async () =>
              {
                  var robot = Boot.GetComponent<RobotComponent>();

                  try
                  {
                      try
                      {
                          robot.IsSelfMachine = message.IsSelfMachine;
                          robot.ClientNo = message.ClientNo;
                          if (string.IsNullOrWhiteSpace(robot.ClientNo))
                              robot.IsSelfMachine = true;
                      }
                      catch (Exception)
                      {
                          robot.IsSelfMachine = true;
                          robot.ClientNo = "RPAII";
                      }
                      Flow flow = message.Flow;
                      flow.IsDebug = message.IsDebug;
                      if (!flow.IsDebug && flow.IsPip)
                      {
                          var flowScript = new FlowScript() { Id = flow.Id };
                          var pip = Boot.GetComponent<RobotComponent>().GetComponent<PIPServerComponent>();
                          var result = await pip.StartAsync();
                          pip.Receive = (data) =>
                          {
                              var logs = robot.GetComponent<FlowComponent>().LogMsgs;
                              var id = IdGenerater.Instance.GenerateInstanceId() + "";
                              MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new RunMsg()
                              {
                                  Error = 0,
                                  Message = JsonHelper.ToJson(logs),
                                  RpcId = message.RpcId
                              }).Coroutine();
                          };
                          if (result.Item1)
                          {
                              if (!pip.Ready)
                              {
                                  SpinWait.SpinUntil(() => pip.Ready, 60 * 1000);
                              }
                              if (pip.Ready)
                              {
                                  pip.Send(flowScript);
                              }
                              else
                              {
                                  System.Windows.MessageBox.Show($"服务连接超时!");
                              }
                          }
                          else
                          {
                              System.Windows.MessageBox.Show(result.Item2);
                          }
                      }
                      else
                      {
                          await robot.Main(flow); // 添加到执行队列
                          var logs = robot.GetComponent<FlowComponent>().LogMsgs;
                          var id = IdGenerater.Instance.GenerateInstanceId() + "";
                          MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new RunMsg()
                          {
                              Error = 0,
                              Message = JsonHelper.ToJson(logs),
                              RpcId = message.RpcId
                          }).Coroutine();
                      }
                  }
                  catch (Exception ex)
                  {
                      Log.Error(ex);
                  }
                  finally
                  {
                      robot.State = 0;
                  }
              });
            MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new RunResponse()
            {
                Error = 0,
                Message = "",
                RpcId = message.RpcId
            }).Coroutine();
        }
    }

}
