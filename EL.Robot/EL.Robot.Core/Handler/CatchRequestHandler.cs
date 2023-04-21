using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Component;
using EL.Robot.Core.Request;
using Protos;
using System.Configuration;
using Utils;

namespace EL.Robot.Core.Handler
{
	[MessageHandler]
	public class CatchRequestHandler : W_AMHandler<Request.CatchRequest>
	{
		protected override async ELTask Run(WChannel channel, Request.CatchRequest message)
		{
			await ELTask.CompletedTask;
			await Boot.GetComponent<TimerComponent>().WaitAsync(1);
			var robot = Boot.GetComponent<RobotComponent>();
			robot.IsSelfMachine = message.IsSelfMachine;
			robot.ClientNo = message.ClientNo;
			robot.StartCatchAction?.Invoke();
			CatchUIRequest catchElementRequest = new CatchUIRequest();
			var respose = (CatchUIResponse)await UtilsComponent.Exec(catchElementRequest);
			await MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new Request.CatchResponse()
			{
				Error = respose.Error,
				Message = respose.Message,
				RpcId = message.RpcId,
				ElementPath = respose.ElementPath,
			});
			robot.StopCatchAction?.Invoke();
			//DispatcherHelper.ExecInspect((x) =>
			//{
			//    var robot = Boot.GetComponent<RobotComponent>();
			//    robot.IsSelfMachine = message.IsSelfMachine;
			//    robot.ClientNo = message.ClientNo;
			//    robot.StartCatchAction?.Invoke();
			//    RequestManager.Start("10000{\"$type\":\"Automation.Inspect.CatchElementRequest, EL.Automation\",\"RpcId\":1,\"Msg\":\"界面探测器启动成功！\"}", async (element, elementPath) =>
			//    {
			//        robot.StopCatchAction?.Invoke();
			//        if (element == default || elementPath == default)
			//        {
			//            ThreadSynchronizationContext.Instance.PostNext(async () =>
			//            {
			//                await MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new CatchResponse()
			//                {
			//                    Error = 601,
			//                    Message = "用户主动取消",
			//                    RpcId = message.RpcId,
			//                    ElementPath = default,
			//                });
			//            });
			//            return;
			//        }
			//        ThreadSynchronizationContext.Instance.PostNext(async () =>
			//        {
			//            await MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new CatchResponse()
			//            {
			//                Error = 0,
			//                Message = "",
			//                RpcId = message.RpcId,
			//                ElementPath = elementPath,
			//            });
			//        });
			//    });
			//});
		}
	}
}
