using EL.Async;
using EL.Robot.Component;
using EL.Robot.Component.DTO;
using EL.Robot.Core.SqliteEntity;
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
			inFlow.ViewSort = TimeHelper.ServerNow();
			self.DesignFlowDic.Add(inFlow.Id, inFlow);
			self.CurrentDesignFlow = inFlow;
			self.Features.Add(new Features()
			{
				Id = inFlow.Id,
				Name = inFlow.Name,
				ViewSort = inFlow.ViewSort,
				CreateDate = inFlow.CreateDate,
				HeadImg = inFlow.HeadImg
			});
			return inFlow;
		}
		public static ComponentResponse CreateRobot(this DesignComponent self, CommponetRequest requst)
		{
			var tempFlow = requst.Data as Flow;
			var flow = self.CreateRobot(tempFlow);
			return new ComponentResponse() { Data = flow };
		}
		public static async ELTask<int> SaveRobot(this DesignComponent self)
		{
			if (!self.DesignFlowDic.Any()) return default;
			int row = 0;
			foreach (var flow in self.DesignFlowDic.Values)
			{
				string flowData = JsonHelper.ToJson(flow);
				var features = new Features()
				{
					Id = flow.Id,
					Name = flow.Name,
					HeadImg = flow.HeadImg,
					CreateDate = flow.CreateDate,
					ViewSort = flow.ViewSort,
				};
				string featuresData = JsonHelper.ToJson(features);
				var logs = self.LogMsgs.Where(x => x.Id == flow.Id).ToList();
				string log = JsonHelper.ToJson(logs);
				row += await RobotDataManagerService.SaveFlow(flow.Id, flowData, featuresData, log);
			}
			return row;
		}
		public static List<Features> LoadRobots(this DesignComponent self)
		{
			self.FlowDatas = RobotDataManagerService.LoadFlows();
			List<Features> features = new();
			if (self.FlowDatas is null) return features;
			self.FlowDatas.ForEach(x =>
			{
				var feature = JsonHelper.FromJson<Features>(x.Features);
				features.Add(feature);
			});
			self.Features = features;
			return features;
		}
		public static Flow StartDesign(this DesignComponent self, long Id)
		{
			self.DesignFlowDic.TryGetValue(Id, out Flow flow);
			if (flow == null)
			{
				var flowData = self.FlowDatas.FirstOrDefault(x => x.Id == Id);
				flow = JsonHelper.FromJson<Flow>(flowData.Content);
				self.DesignFlowDic.Add(flow.Id, flow);
				var logs = JsonHelper.FromJson<List<DesignMsg>>(flowData.DesignMsg);
				if (logs is not null)
					self.LogMsgs.AddRange(logs);
			}

			self.CurrentDesignFlow = flow;
			self.CurrentDesignFlow.ViewSort = TimeHelper.ServerNow();
			var fea = self.Features.FirstOrDefault(x => x.Id == Id);
			fea.ViewSort = self.CurrentDesignFlow.ViewSort;
			//self.LoadParams();
			return flow;
		}
		private static void LoadParams(this DesignComponent self)
		{
			if (self.CurrentDesignFlow == null) return;
			if (self.CurrentDesignFlow.Steps == null) return;
			foreach (var node in self.CurrentDesignFlow.Steps)
			{
				var parameter = node.Parameters.FirstOrDefault(x => x.Key == nameof(Node.OutParameterName));
				if (parameter == null)
					continue;
				self.CurrentDesignFlow.SetFlowParam(parameter.Value + "", null);
				if (node.ComponentName.ToLower() == nameof(SetVariableComponent).ToLower())
				{
					var parameterValue = node.Parameters.FirstOrDefault(x => x.Key == nameof(SetVariableComponent.VariableValue));
					self.CurrentDesignFlow.SetFlowParam(parameter.Value + "", parameterValue.Value);
				}
			}

		}
		public static List<DesignMsg> GetMsg(this DesignComponent self)
		{
			return self.LogMsgs.Where(x => self.CurrentDesignFlow.Id == x.Id).ToList();
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
			if (self.CurrentDesignFlow == null) return new List<string>();
			if (self.CurrentDesignFlow.ParamsManager != null)
				foreach (var item in self.CurrentDesignFlow.ParamsManager)
				{
					if (types.Contains(item.Value.Value.GetType()))
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
			long id = default;
			if (self.CurrentDesignFlow != null) id = self.CurrentDesignFlow.Id;
			var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
			var entity = new DesignMsg(id, msg, isException);
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
		public static bool CreateNode(this DesignComponent self, Node node)
		{
			if (self.CurrentDesignFlow == null)
			{
				self.WriteLog("对不起，请先打开机器人哟！");
				return false;
			}
			self.CurrentDesignFlow.Steps.Add(node);
			var parameter = node.Parameters.FirstOrDefault(x => x.Key == nameof(Node.OutParameterName));
			self.CurrentDesignFlow.SetFlowParam(parameter.Value.Value + "", null);
			if (node.ComponentName.ToLower() == nameof(SetVariableComponent).ToLower())
			{
				var parameterValue = node.Parameters.FirstOrDefault(x => x.Key == nameof(SetVariableComponent.VariableValue));
				self.CurrentDesignFlow.SetFlowParam(parameter.Value.Value + "", parameterValue.Value);
			}
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
