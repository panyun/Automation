using EL.Async;
using EL.Robot.Component;
using EL.Robot.Component.DTO;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Permissions;

namespace EL.Robot.Core
{
	public static class DesignComponentSystem
	{
		public static Flow CreateRobot(this DesignComponent self, Flow inFlow)
		{
			inFlow.Id = IdGenerater.Instance.GenerateId();
			inFlow.Steps = new List<Node>();
			Node startNode = new()
			{
				ComponentName = nameof(StartComponent)
			};
			inFlow.Steps.Add(startNode);
			inFlow.CreateDate = TimeHelper.ServerNow();
			self.DesignFlowDic.Add(inFlow.Id, inFlow);
			self.CurrentDesignFlow = inFlow;
			return inFlow;
		}
		public static ComponentResponse CreateRobot(this DesignComponent self, CommponetRequest requst)
		{
			var tempFlow = requst.Data as Flow;

			var flow = self.CreateRobot(tempFlow);
			return new ComponentResponse() { Data = flow };
		}
		public static List<Flow> LoadRobots(this DesignComponent self)
		{
			return self.DesignFlowDic.Values.ToList();
		}
		public static Flow StartDesign(this DesignComponent self, long Id)
		{
			self.DesignFlowDic.TryGetValue(Id, out Flow flow);
			self.CurrentDesignFlow = flow;
			return flow;
		}
		public static ComponentResponse StartDesign(this DesignComponent self, CommponetRequest requst)
		{
			var tempFlow = (long)requst.Data;
			var flow = self.StartDesign(tempFlow);
			return new ComponentResponse() { Data = flow };
		}
		public static List<DesignMsg> GetDesignMsg(this DesignComponent self)
		{
			return self.LogMsgs.Where(x => x.Id == self.CurrentDesignFlow.Id).ToList();
		}
		public static List<string> SelectVariable(this DesignComponent self, List<Type> types)
		{
			var keys = new List<string>();
			if (self.CurrentDesignFlow.ParamsManager != null)
				foreach (var item in self.CurrentDesignFlow.ParamsManager)
				{
					if (types.Contains(item.Value.GetType()))
					{
						keys.Add(item.Key);
					}
				}
			return keys;
		}
		public static ComponentResponse SelectVariable(this DesignComponent self, SelectVariableRequest requst)
		{
			var data = self.SelectVariable(requst.Types);
			return new ComponentResponse() { Data = data };
		}
		public static void WriteFlowLog(this DesignComponent self, string msg, long takeTime = default)
		{
			if (string.IsNullOrWhiteSpace(msg)) return;
			var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
			if (flow == null)
			{
				self.WriteLog(msg);
				return;
			}
			var entity = new DesignMsg(flow, msg, takeTime);
			self.LogMsgs.Add(entity);
			self.RefreshLogMsgAction?.Invoke(entity);
		}
		public static void WriteNodeLog(this DesignComponent self, Node node, string msg, long takeTime = default)
		{
			if (string.IsNullOrWhiteSpace(msg)) return;
			var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
			if (node is null)
			{
				self.WriteLog(msg);
				return;
			}
			var entity = new DesignMsg(node, msg, takeTime);
			self.LogMsgs.Add(entity);
			self.RefreshLogMsgAction?.Invoke(entity);
		}
		public static void WriteNodeLog(this DesignComponent self, Node node, Exception ex, long takeTime = default)
		{
			var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
			var entity = new DesignMsg(node, ex, takeTime);
			self.LogMsgs.Add(entity);
			self.RefreshLogMsgAction?.Invoke(entity);
		}
		public static void WriteLog(this DesignComponent self, string msg, bool isException = false)
		{
			var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
			var entity = new DesignMsg(msg, isException);
			self.LogMsgs.Add(entity);
			self.RefreshLogMsgAction?.Invoke(entity);
		}
		public static ComponentResponse FinshDesign(this DesignComponent self, long id)
		{
			//保存数据库
			if (self.CurrentDesignFlow == null)
			{
				self.WriteLog("请先打开流程吧");
				return new ComponentResponse();
			}
			if (self.CurrentDesignFlow.Id != id)
			{
				self.WriteLog("当前设计流程与完成流程不一致");
				return new ComponentResponse();
			}
			self.CurrentDesignFlow = null;
			return new ComponentResponse();
		}
		public static ComponentResponse FinshDesign(this DesignComponent self, CommponetRequest requst)
		{
			var tempFlow = requst.Data as Flow;
			self.DesignFlowDic.TryGetValue(tempFlow.Id, out Flow flow);
			if (flow != default) self.DesignFlowDic.Remove(tempFlow.Id);
			return new ComponentResponse();
		}
		public static bool CreateVariable(this DesignComponent self, Variable variable)
		{
			self.CurrentDesignFlow.Variables.Add(variable);
			self.CurrentDesignFlow.ParamsManager.Add(variable.Name, variable.Value);
			return true;
		}
		public static bool CreateNode(this DesignComponent self, Node node)
		{
			if (self.CurrentDesignFlow == null)
			{
				self.WriteLog("对不起，请先打开机器人哟！");
				return false;
			}
			self.CurrentDesignFlow.Steps.Add(node);
			return true;
		}
		//模拟执行组件
		public static async ELTask<List<ExecLog>> PreviewNodes(this DesignComponent self, List<Node> nodes)
		{
			await ELTask.CompletedTask;
			CommponetRequest commponetRequest = new CommponetRequest()
			{
				Data = new Flow()
				{
					Id = IdGenerater.Instance.GenerateId(),
					Steps = nodes
				}
			};
			await RequestManager.ExecFlow(commponetRequest);
			var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
			return flowComponent.LogMsgs;
		}
		//模拟执行组件
		public static async ELTask<ComponentResponse> PreviewNodes(this DesignComponent self, CommponetRequest request)
		{
			await ELTask.CompletedTask;
			CommponetRequest commponetRequest = new CommponetRequest()
			{
				Data = new Flow()
				{
					Id = IdGenerater.Instance.GenerateId(),
					Steps = request.Data
				}
			};
			return await RequestManager.ExecFlow(commponetRequest);
		}
		public static async Task<List<ExecLog>> RunRobot(this DesignComponent self)
		{
			if (self.CurrentDesignFlow == null)
			{
				self.WriteLog("请先打开机器人才能运行哟！");
				return default;
			}
			try
			{
				var robot = Boot.GetComponent<RobotComponent>();
				await robot.LocalMain(self.CurrentDesignFlow, false);
				var logs = robot.GetComponent<FlowComponent>().LogMsgs;
				return logs;
			}
			catch (Exception ex)
			{
				self.WriteLog(ex.Message);
			}
			return default;

		}
	}
}
