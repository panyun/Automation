using EL.Async;
using EL.Robot.Component;
using System.Security.Permissions;
using System.Text;

namespace EL.Robot.Core
{
	public class NodeComponentAwake : AwakeSystem<NodeComponent>
	{
		public override void Awake(NodeComponent self)
		{
			var components = typeof(ElementActionComponent).Assembly.GetTypes().Where(t => t.BaseType == typeof(BaseComponent));
			var programComponents = typeof(LoopComponent).Assembly.GetTypes().Where(t => t.BaseType == typeof(BaseComponent));
			foreach (var item in components)
			{
				var entity = self.AddComponent(item);
				self.ChildComponent.Add(item.Name, entity);
			}
			foreach (var item in programComponents)
			{
				var entity = self.AddComponent(item);
				self.ChildComponent.Add(item.Name, entity);
			}
		}
	}
	public static class NodeComponentSystem
	{
		public static object GetNodeDicValue(this Node self, string key)
		{
			var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
			if (self == null) throw new ArgumentNullException(nameof(self));
			if (self.DictionaryParam != null)
			{
				self.DictionaryParam.TryGetValue(key, out var value);
				if (value == default)
					throw new ELNodeHandlerException($"获取属性[{key}]为空！");
				if (value.ToString().StartsWith("@"))
					return flowComponent.MainFlow.GetFlowDicValue(key);
				return value;
			}
			return flowComponent.MainFlow.GetFlowDicValue(key);
		}
		public static async ELTask Main(this NodeComponent self)
		{
			await Async.ELTask.CompletedTask;
			var robot = Boot.GetComponent<RobotComponent>();
			var flowComponent = robot.GetComponent<FlowComponent>();

			await self.Exec(flowComponent.CurrentFlow.Steps);
		}
		public static void InitNodeDictionaryParam(this NodeComponent self, Node node)
		{

			node.DictionaryParam = new Dictionary<string, object>();
			if (node.Parameters == null)
				return;
			foreach (var x in node.Parameters)
			{
				if (x == null) continue;
				if (x.Key == null) continue;
				var key = x.Key.Trim().ToLower();
				if (node.DictionaryParam.ContainsKey(key.ToLower()))
					node.DictionaryParam[key] = x.Value;
				else
					node.DictionaryParam.Add(key, x.Value);
			}
		}
		public static async Async.ELTask Exec(this NodeComponent self, List<Node> nodes)
		{
			if (nodes == null || nodes.Count == 0) return;
			foreach (var node in nodes)
			{
				self.InitNodeDictionaryParam(node);
				self.StartNodeAction?.Invoke(node);
				BaseProperty tryInfo = node.GetBaseProperty();
				var robot = Boot.GetComponent<RobotComponent>();
				var flowComponent = robot.GetComponent<FlowComponent>();
				node.Flow = flowComponent.CurrentFlow;
				if (node.Ignore)
				{
					flowComponent.WriteNodeLog(node, $"当前节点忽略");
					continue;
				}
				try
				{
					await self.TryExec(node, async () =>
					{
						robot.CurrentNode = node;
						if (robot.ExecState == ExecState.IsStop)
							return;
						if (robot.ExecState == ExecState.IsContinue || robot.ExecState == ExecState.IsBreak)
							return;
						flowComponent.WriteNodeLog(node, $"执行开始");
						var takeTimeComponent = TakeTimeComponent.StartNew();
						//前置延时
						var time = tryInfo?.PreTimeDelay <= 0 ? 1 : tryInfo.PreTimeDelay;
						//await Boot.GetComponent<TimerComponent>().WaitAsync(time);
						//await Boot.GetComponent<TimerComponent>().WaitAsync(10000);
						//await Task.Delay(time);
						Thread.Sleep(time);
						//检查是否有断点
						if (robot.ExecState == ExecState.None)
							robot.ExecState = node.Flow.IsDebug && node.Debug ? ExecState.IsPaused : ExecState.None; //断点暂停
						if (robot.ExecState == ExecState.IsPaused || robot.ExecState == ExecState.IsStep)
						{
							robot.PausedAction?.Invoke();
							robot.ELTaskPaused = ELTask<bool>.Create();
							flowComponent.WriteNodeLog(node, $"进入调试");
							await robot.ELTaskPaused;
							flowComponent.WriteNodeLog(node, $"调试等待", takeTimeComponent.Stop());
							if (robot.ExecState == ExecState.IsStop)
								return;
							robot.NoneAction?.Invoke();
						}

						string msg = default;
						var component = self.FindComponent(node.ComponentName);
						INodeContent content = default;
						if (component == null)
							throw new ELNodeHandlerException($"未找到处理程序{node.ComponentName}");
						msg = self.GetNodeDicValue(node);
						flowComponent.WriteNodeLog(node, msg);
						content = await component.Main(self.CreateNodeContent(node));
						msg = node.Flow.SetFlowParam(node.OutParameterName, (object)content.Out);
						msg = node.Flow.SetFlowParam("current", (object)content.Out);

						flowComponent.WriteNodeLog(node, msg);
						flowComponent.WriteNodeLog(node, $"执行完成", takeTimeComponent.Stop());
						self.EndNodeAction?.Invoke(new NodeState { Node = node, IsSucess = true, Msg = "" });
						time = tryInfo.RearTimeDelay <= 0 ? 1 : tryInfo.RearTimeDelay;
						//后置延时
						//await Boot.GetComponent<TimerComponent>().WaitAsync(tryInfo.RearTimeDelay <= 0 ? 1 : tryInfo.RearTimeDelay);
						Thread.Sleep(time);
						if (content.Value)
							await self.Exec(node.Steps);
						if (robot.ExecState == ExecState.IsContinue || robot.ExecState == ExecState.IsBreak)
							return;
					});
				}
				catch (Exception ex)
				{
					self.EndNodeAction?.Invoke(new NodeState { Node = node, IsSucess = false, Msg = ex.Message });
					flowComponent.WriteNodeLog(node, ex);
					if (tryInfo.Exceptionhandling == 2)
						continue;
					else return;
				}

			}
		}
		public static async ELTask TryExec(this NodeComponent self, Node node, Func<ELTask> action)
		{
			await ELTask.CompletedTask;
			BaseProperty tryInfo = node.GetBaseProperty();
			string exMsg = string.Empty;
			if (tryInfo == null)
			{
				try
				{
					await action.Invoke();
					return;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			//终止流程
			if (tryInfo.Exceptionhandling == 1)
			{
				try
				{
					await action.Invoke();
					return;
				}
				catch (Exception ex)
				{
					exMsg = ex.Message;
				}
			}
			else if (tryInfo.Exceptionhandling == 2)
			{
				try
				{
					await action.Invoke();
					return;
				}
				catch (Exception ex)
				{
					exMsg = ex.Message;
				}
			}
			else
			{
				int index = 0;
				while (index < tryInfo.RetryCount)
				{
					try
					{
						await action.Invoke();
						return;
					}
					catch (Exception ex)
					{
						exMsg = ex.Message;
						index++;
					}
				}
			}
			if (!string.IsNullOrEmpty(tryInfo.CustomExceptionInfo))
				exMsg = tryInfo.CustomExceptionInfo;
			throw new Exception(exMsg);
		}
		public static string GetNodeDicValue(this NodeComponent self, Node node)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (node.DictionaryParam == default)
				return default;
			foreach (var item in node.Parameters)
			{
				if (item == default) continue;
				var val = item.Value == null ? "" : item.Value.ToString();
				val = val.Length > 500 ? val.Substring(0, 500) : val;
				stringBuilder.Append($"\r\n         [输入参数 {item.Title} = {val}]");
			}
			return stringBuilder.ToString() + "\r\n";
		}
		public static NodeContent CreateNodeContent(this NodeComponent self, Node node)
		{
			return NodeContent.Create(node.Flow, node);
		}
		public static INodeCompoonent FindComponent(this NodeComponent self, string name)
		{
			var coms = self.Components;
			if (string.IsNullOrEmpty(name)) throw new ELNodeHandlerException("组件名称为空！");
			name = name.Trim().ToLower();
			foreach (var com in coms)
			{
				if (com.Key.Name.ToLower() == name.ToLower())
					return com.Value as INodeCompoonent;
				var nodeInfo = com.Value as INodeCompoonent;
				if (nodeInfo.Config.ComponentName.Trim().ToLower() == name)
					return nodeInfo;
			}
			return default;
		}
	}
}
