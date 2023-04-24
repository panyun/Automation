using EL.Robot.Component;
using EL.Robot.Core.SqliteEntity;
using MySqlX.XDevAPI.Relational;
using Newtonsoft.Json;

namespace EL.Robot.Core
{
	public static class FlowComponentSystem
	{
		public static async Async.ELTask Main(this FlowComponent self)
		{
			await Async.ELTask.CompletedTask;
			var time = TakeTimeComponent.StartNew();
			var nodeComponent = self.GetComponent<NodeComponent>();
			self.CurrentFlow = self.MainFlow;
			self.StartFlowAction?.Invoke();
			self.WriteFlowLog("脚本开始");
			await nodeComponent.Main();
			var takeTime = time.Stop();
			self.FlowHistory.TakeTime = takeTime;
			self.WriteFlowLog("脚本结束", takeTime);
			self.EndFlow();
			self.EndFlowAction?.Invoke();
		}
		public static void EndFlow(this FlowComponent self)
		{
			var entitys = self.LogMsgs.Where(x => x.IsException).ToList();
			if (entitys == null || entitys.Count == 0)
			{
				self.FlowHistory.State = 0;
				return;
			}
			self.FlowHistory.State = 1;
			self.FlowHistory.ExNodeName = entitys[0].Name;
			self.FlowHistory.ExMsg = entitys[0].Msg;
		}
		public static void WriteFlowLog(this FlowComponent self, string msg, long takeTime = default)
		{
			if (string.IsNullOrWhiteSpace(msg)) return;
			var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
			var entity = new ExecLog(flow, msg, takeTime);
			self.LogMsgs.Add(entity);
			self.RefreshLogMsgAction?.Invoke(entity);
		}
		public static Dictionary<long, Node> GetSteps(this FlowComponent self)
		{
			var stemps = self.GetSteps(self.CurrentFlow.Steps);
			foreach (var item in stemps)
			{
				if (!self.Steps.ContainsKey(item.Key))
					self.Steps.Add(item.Key, item.Value);
			}
			return self.Steps;
		}
		private static Dictionary<long, Node> GetSteps(this FlowComponent self, List<Node> nodes)
		{
			Dictionary<long, Node> steps = new Dictionary<long, Node>();
			if (nodes == null || nodes.Count == 0) return default;
			foreach (var node in nodes)
			{
				steps.Add(node.Id, node);
				if (node.Steps != null)
				{
					var dics = GetSteps(self, node.Steps);
					if (dics != null && dics.Values.Count > 0)
						dics.Values.ToList().ForEach(x => steps.Add(x.Id, x));
				}
			}
			return steps;
		}

		public static void WriteNodeLog(this FlowComponent self, Node node, string msg, long takeTime = default)
		{
			if (string.IsNullOrWhiteSpace(msg)) return;
			var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
			var entity = new ExecLog(node, msg, takeTime);
			self.LogMsgs.Add(entity);
			self.RefreshLogMsgAction?.Invoke(entity);
		}
		public static void WriteNodeLog(this FlowComponent self, Node node, Exception ex, long takeTime = default)
		{
			var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
			var entity = new ExecLog(node, ex, takeTime);
			self.LogMsgs.Add(entity);
			self.RefreshLogMsgAction?.Invoke(entity);
		}
		public static string SetFlowParam(this Flow self, string key, object value)
		{
			if (string.IsNullOrWhiteSpace(key)) return default;
			key = key.ToLower().Trim().TrimStart('@');
			if (string.IsNullOrWhiteSpace(key)) return default;
			var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
			string msg = $"         [变量赋值 {key}={JsonHelper.ToJson(value)}]";
			if (self.ParamsManager == null) self.ParamsManager = new Dictionary<string, object>();
			if (self.ParamsManager.ContainsKey(key))
				self.ParamsManager[key] = value;
			else
				self.ParamsManager.Add(key, value);
			flowComponent.RefreshVariablesAction?.Invoke(key);
			return msg;
		}
		public static void InitParam(this FlowComponent self, Node startNode)
		{
			var flow = self.CurrentFlow;
			flow.ParamsManager = new Dictionary<string, object>();
			var takeTimeComponent = TakeTimeComponent.StartNew();
			self.WriteNodeLog(startNode, "变量初始化开始");
			if (flow.Variables != null && flow.Variables.Count > 0)
			{
				foreach (var variable in flow.Variables)
				{
					var val = GetVariablesValue(variable);
					var msg = flow.SetFlowParam(variable.Name, val);
					self.WriteNodeLog(startNode, msg);
				}
			}
			if (flow.InParams != null && flow.InParams.Count > 0)
			{
				//添加输入变量
				foreach (var item in flow.InParams)
				{
					var msg = flow.SetFlowParam(item.Key, item.Value);
					self.WriteNodeLog(startNode, msg);
				}
			}
			if (flow.OutParams != null && flow.OutParams.Count > 0)
			{
				//添加输入变量
				foreach (var item in flow.OutParams)
				{
					var msg = flow.SetFlowParam(item.Key, item.Value);
					self.WriteNodeLog(startNode, msg);
				}
			}
			self.WriteNodeLog(startNode, "变量初始化结束", takeTimeComponent.Stop());
		}
		
		public static object GetVariablesValue(Variable variable)
		{
			if (variable == null) return default;
			var type = variable.Type;
			if (type == default) return default;
			object value = default;
			bool isSucess = true;
			if (type == typeof(string))
			{
				value = variable.Value + "";
				isSucess = true;
			}
			else if (type == typeof(int) || type == typeof(double) || type == typeof(float))
			{
				isSucess = double.TryParse(variable.Value, out var val);
				value = val;
			}
			else if (type == typeof(bool))
			{
				isSucess = bool.TryParse(variable.Value, out var val);
				value = val;

			}
			else if (type == typeof(object))
			{
				try
				{
					value = JsonConvert.DeserializeObject<Dictionary<string, object>>(variable.Value);
				}
				catch (Exception)
				{

					isSucess = false;
				}


			}
			else if (type == typeof(Array))
			{
				try
				{
					value = JsonConvert.DeserializeObject<List<object>>(variable.Value);
				}
				catch (Exception)
				{
					isSucess = false;
				}
			}
			else if (type == typeof(Table))
			{
				//[{"c1":"r1","c2":"r1"},{"c1":"r2","c2":"r2"}]

			}
			else if (type == typeof(DateTime))
			{
				isSucess = DateTime.TryParse(variable.Value, out var val);
				value = val;
			}
			if (!isSucess)
			{
				throw new ELNodeHandlerException($"初始化函数转化失败,初始类型{type}，初始值{variable.Value}");
			}
			return value;
		}
		public static object GetFlowDicValue(this Flow self, string key)
		{
			var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
			if (self == null) throw new ArgumentNullException(nameof(self));
			if (self.ParamsManager != null && self.ParamsManager.ContainsKey(key))
			{
				self.ParamsManager.TryGetValue(key, out var value);
				if (value == default)
					throw new ELNodeHandlerException($"获取参数[{key}]为空！");
				return value;
			}
			//if (self.Variables != null && self.Variables.Exists(x => x.Name == key))
			//    return self.Variables.FirstOrDefault(x => x.Name == key).Value;
			throw new ELNodeHandlerException($"获取参数[{key}]为空！");
		}
		public static List<Node> GetFlowNods(this FlowComponent self)
		{
			if (self == default || self.MainFlow == default)
				return default;
			return self.GetFlowNods(self.MainFlow?.Steps);
		}
		private static List<Node> GetFlowNods(this FlowComponent self, List<Node> nodes)
		{
			List<Node> result = new List<Node>();
			if (nodes == null || nodes.Count == 0) return new List<Node>();
			foreach (var node in nodes)
			{
				result.Add(node);
				var datas = self.GetFlowNods(node.Steps);
				result.AddRange(datas);
			}
			return result;
		}
	}
}

