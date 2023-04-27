using EL.Async;
using EL.Robot.Component;
using EL.Robot.Component.DTO;
using EL.Robot.Core.SqliteEntity;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace EL.Robot.Core
{
	public static class DesignComponentSystem
	{
		public static Flow CreateRobot(this DesignComponent self, Flow inFlow)
		{
			inFlow.Id = IdGenerater.Instance.GenerateId();
			inFlow.DesignSteps = new List<Node>();
			Node startNode = new()
			{
				ComponentName = nameof(StartComponent),
				Name = "开始流程"
			};
			inFlow.DesignSteps.Add(startNode);
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
		public static async ELTask<int> SaveRobot(this DesignComponent self, bool isAll = true)
		{
			if (!self.DesignFlowDic.Any()) return default;
			if (isAll)
			{
				int row = 0;
				foreach (var flow in self.DesignFlowDic.Values)
					row += await SaveAsync(flow);
				return row;
			}
			return await SaveAsync(self.CurrentDesignFlow);
			async Task<int> SaveAsync(Flow flow)
			{
				self.CurrentDesignFlow.Steps.Clear();
				self.GetSteps();
				self.CurrentDesignFlow.DesignSteps.Clear();
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
				return await RobotDataManagerService.SaveFlow(flow.Id, flowData, featuresData, log);
			}
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
		public static async ELTask<Flow> StartDesign(this DesignComponent self, long Id)
		{
			await self.SaveRobot(false);
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
			self.CurrentDesignFlow.DesignSteps.Clear();
			self.GetDesignSteps(self.CurrentDesignFlow.Steps);
			self.CurrentDesignFlow.DesignSteps.ForEach(x => x.Steps.Clear());
			self.CurrentDesignFlow.ViewSort = TimeHelper.ServerNow();
			var fea = self.Features.FirstOrDefault(x => x.Id == Id);
			fea.ViewSort = TimeHelper.ServerNow();
			self.RefreshAllStepCMD();
			return flow;
		}
		private static void GetSteps(this DesignComponent self)
		{
			var tops = self.CurrentDesignFlow.DesignSteps.Where(x => x.DesignParent == null);
			self.CurrentDesignFlow.Steps.AddRange(tops);
			foreach (var item in tops)
			{
				self.Get(item);
			}
		}
		private static void Get(this DesignComponent self, Node node)
		{
			var steps = self.CurrentDesignFlow.DesignSteps.Where(x => x.DesignParent == node);
			foreach (var item in steps)
			{
				item.DesignParent = null;
				node.Steps.Add(item);
				self.Get(item);
			}

		}

		private static void GetDesignSteps(this DesignComponent self, List<Node> steps)
		{
			foreach (var item in steps)
			{
				self.CurrentDesignFlow.DesignSteps.Add(item);
				self.GetDesignSteps(item.Steps);
			}
		}
		public static void RefreshAllStepCMD(this DesignComponent self)
		{
			self.ClearNodeCmdAction?.Invoke();
			var entity = self.CurrentDesignFlow.DesignSteps.FirstOrDefault(x => x.IsNew);
			if (entity == null) self.CurrentDesignFlow.DesignSteps[self.CurrentDesignFlow.DesignSteps.Count-1].IsNew = true;
			foreach (var step in self.CurrentDesignFlow.DesignSteps)
			{
				if (step.LinkNode != null)
				{
					self.CurrentDesignFlow.DesignSteps.Where(x => x.DesignParent == step).ToList().ForEach((x =>
					{
						x.IsView = step.IsView;
					}));
					var index = self.CurrentDesignFlow.DesignSteps.IndexOf(step) + 1;
					self.RefreshNodeCmdAction?.Invoke(step, index + step.DisplayExp);
					continue;
				}
				if (step.IsView)
				{
					var index = self.CurrentDesignFlow.DesignSteps.IndexOf(step) + 1;
					self.RefreshNodeCmdAction?.Invoke(step, index + step.DisplayExp);
				}
			}
			
			self.RefreshNodeCmdEndAction();
		}
		public static BaseComponent GetComponentInfo(this DesignComponent self, string componentName)
		{
			return self.NodeComponentRoot.FindComponent(componentName);
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
					if (item.Value == null || item.Value.Types == default) continue;
					var isExist = item.Value.Types.Where(x => types.Contains(x));
					if (isExist.Any())
						keys.Add(item.Key);
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

		public static void RefreshStepCMD(this DesignComponent self, Node node)
		{
			var index = self.CurrentDesignFlow.DesignSteps.IndexOf(node) + 1;
			self.RefreshNodeCmdAction?.Invoke(node, index + node.DisplayExp);
			if (node.Steps == null) return;
			foreach (var item in node.Steps)
			{
				if (!item.IsView)
					self.RefreshStepCMD(item);
			}
		}
		public static bool CreateNode(this DesignComponent self, Node node)
		{
			if (self.CurrentDesignFlow == null)
			{
				self.WriteLog("对不起，请先打开机器人哟！");
				return false;
			}
			self.CreateNode(node, self.CurrentDesignFlow.DesignSteps.Count);
			return true;
		}
		public static bool CreateNode(this DesignComponent self, Node node, int index)
		{
			if (self.CurrentDesignFlow == null)
			{
				self.WriteLog("对不起，请先打开机器人哟！");
				return false;
			}

			node.IsNew = true;
			var parent = self.FindParent(index);
			if (parent != null)
				node.DesignParent = parent;
			self.CurrentDesignFlow.DesignSteps.Insert(index, node);
			if (node.ComponentName == nameof(IFStartComponent))
			{
				var component = self.GetComponentInfo(nameof(IFEndComponent));
				component.GetConfig();
				var temp = new Node()
				{
					Id = IdGenerater.Instance.GenerateId(),
					Name = component.Config.CmdDisplayName,
					ComponentName = component.Config.ComponentName,

				};
				node.LinkNode = temp;
				self.CurrentDesignFlow.DesignSteps.Insert(index + 1, temp);
			}
			self.RefreshAllStepCMD();
			var parameter = node.Parameters.FirstOrDefault(x => x.Key == nameof(Node.OutParameterName));
			if (parameter == null || parameter.Value == null)
				return true;
			self.CurrentDesignFlow.SetFlowParam(parameter.Value.Value + "", null);
			if (node.ComponentName.ToLower() == nameof(SetVariableComponent).ToLower())
			{
				var parameterValue = node.Parameters.FirstOrDefault(x => x.Key == nameof(SetVariableComponent.VariableValue));
				self.CurrentDesignFlow.SetFlowParam(parameter.Value.Value + "", parameterValue.Value);
			}
			return true;
		}
		public static Node FindParent(this DesignComponent self, int index)
		{
			Node node = default;
			for (int i = index - 1; i > 0; i--)
			{
				node = self.CurrentDesignFlow.DesignSteps[i];
				if (node.LinkNode != null)
				{
					var endIndex = self.CurrentDesignFlow.DesignSteps.IndexOf(node.LinkNode);
					if (index <= endIndex) return node;
				}
			}
			return default;
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
