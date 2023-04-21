using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Component;
using EL.Robot.Core.Request;
using Protos;
using Utils;

namespace EL.Robot.Core.Handler
{
	[MessageHandler]
    public class CatchSimilarHandler : W_AMHandler<CatchSimilarRequest>
    {
        protected override async ELTask Run(WChannel channel, CatchSimilarRequest message)
        {
            await ELTask.CompletedTask;
            await Boot.GetComponent<TimerComponent>().WaitAsync(1);
            var robot = Boot.GetComponent<RobotComponent>();
            robot.IsSelfMachine = message.IsSelfMachine;
            robot.ClientNo = message.ClientNo;
            robot.StartCatchAction?.Invoke();

			CatchUIRequest catchElementRequest = new CatchUIRequest();
            var respose = (CatchUIResponse)await UtilsComponent.Exec(catchElementRequest);
            if (message.Type == 2)
            {
                await Boot.GetComponent<TimerComponent>().WaitAsync(1000);
                catchElementRequest.Msg = "请捕获第二次！";
                var respose1 = (CatchUIResponse)await UtilsComponent.Exec(catchElementRequest);
                GenerateSimilarElementActionRequest generateSimilarElementActionRequest = new GenerateSimilarElementActionRequest
                {
                    ElementPath = respose.ElementPath,
                    LastElementPath = respose1.ElementPath,
                    LightProperty = new LightProperty()
                    {
                        Count = 3,
                        Time = 500
                    }
                };
                var res = (GenerateSimilarElementActionResponse)await UtilsComponent.Exec(generateSimilarElementActionRequest);
                await MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new CatchSimilarResponse()
                {
                    Error = res.Error,
                    Message = res.Message,
                    RpcId = res.RpcId,
                    ElementPath = respose.ElementPath,
                });
                robot.StopCatchAction?.Invoke();
                return;
            }
            if (message.Type == 1)
            {
                GenerateCosineSimilarActionRequest request = new GenerateCosineSimilarActionRequest()
                {
                    ElementPath = respose.ElementPath,
                    CosineValue = message.CosineValue * 0.01,
                    LightProperty = new LightProperty()
                    {
                        Count = 3,
                        Time = 500
                    }
                };
                var res = (GenerateCosineSimilarActionResponse)await UtilsComponent.Exec(request);
                await MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new CatchSimilarResponse()
                {
                    Error = res.Error,
                    Message = res.Message,
                    RpcId = res.RpcId,
                    ElementPath = res.ElementPath,
                });
                robot.StopCatchAction?.Invoke();
            }
            //DispatcherHelper.ExecInspect((inspect, parser) =>
            //{
            //    RequestManager.Start("10000{\"$type\":\"Automation.Inspect.CatchElementRequest, EL.Automation\",\"RpcId\":1,\"Msg\":\"界面探测器启动成功！\"}", async (element, elementPath) =>
            //    {
            //        var isWindows = RequestOptionComponent.IsWindow;
            //        Debug.WriteLine(isWindows.ToString());
            //        Thread.Sleep(100);
            //        var elSqlComponent = Boot.AddComponent<ElSqliteComponent>();
            //        var robot = Boot.GetComponent<RobotComponent>();

            //        if (DispatcherHelper.mainWindowInstance == null)
            //        {
            //            DispatcherHelper.mainWindowInstance = new MainWindow();
            //            DispatcherHelper.mainWindowInstance.Closed += (x, y) =>
            //            {
            //                DispatcherHelper.mainWindowInstance = null;
            //            };
            //        }
            //        var mainWindow = DispatcherHelper.mainWindowInstance;
            //        try
            //        {
            //            mainWindow.Topmost = true;
            //            mainWindow.Show();
            //            mainWindow.Topmost = false;
            //            mainWindow._vm.LoadElementModelByRobot(elementPath, async (element, elementPath) =>
            //            {
            //                await MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new CatchSimilarResponse()
            //                {
            //                    Error = 0,
            //                    Message = "",
            //                    ElementPath = elementPath,
            //                });
            //            });
            //        }
            //        catch (Exception ex)
            //        {
            //            Log.Error(ex);
            //        }
            //    });
            //});
        }
    }
}
