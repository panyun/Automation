using EL.Robot.Component;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core
{
	public class NodeComponent : Entity
	{
		public Action<Node> StartNodeAction { get; set; }
		public Action<NodeState> EndNodeAction { get; set; }
		public List<ExecLog> LogMsgs { get; set; } = new List<ExecLog>();
		public Dictionary<string, Entity> ChildComponent { get; } = new Dictionary<string, Entity>();
	}
	public class ExecLog
	{
		public ExecLog(Node node, string msg, long takeTime = default) : this(node?.Id ?? default, node?.Name ?? default, msg, "组件", default, takeTime)
		{

		}
		public ExecLog(Node node, Exception ex, long takeTime = default) : this(node?.Id ?? default, node?.Name ?? default, default, "组件", ex, takeTime)
		{

		}
		public ExecLog(Flow flow, string msg, long takeTime = default) : this(flow.Id, flow.Name, msg, "流程", default, takeTime)
		{

		}
		private ExecLog(long id, string name, string msg, string type, Exception ex = default, long takeTime = default)
		{
			Name = name;
			Id = id;
			Time = DateTime.Now.ToString("yy-MM-dd HH:mm:ss fff");
			Type = type;
			IsException = ex != default;
			Msg = ex != null ? ex.Message : msg;
			StackTrace = ex != null ? ex.StackTrace : default;
			TakeTime = takeTime;
			if (TakeTime != default)
			{
				string tempMsg = $"耗时{TakeTime}ms";
				if (TakeTime > 1000)
					tempMsg = $"耗时{TakeTime / 1000f}s";
				ShowMsg = $" [{DateTime.Now:y-M-d HH:mm:ss fff}] [{Type}] [{Name}/{Id}] 信息：【 ---{tempMsg}  {Msg} ---】";
				return;
			}
			ShowMsg = $" [{DateTime.Now:y-M-d HH:mm:ss fff}] [{Type}] [{Name}/{Id}] 信息：【 --- {Msg} ---】";
		}
		/// <summary>
		/// 流程名称
		/// </summary>
		public string Name { get; set; }
		public long Id { get; set; }
		public string Type { get; set; }
		/// <summary>
		/// 是否异常
		/// </summary>
		public bool IsException { get; set; }
		[BsonIgnoreIfDefault]
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		/// <summary>
		/// 异常堆栈
		/// </summary>
		public string StackTrace { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Time { get; set; }
		public long TakeTime { get; set; }
		public string Msg { get; set; }
		public string ShowMsg { get; set; }
		public override string ToString()
		{
			return ShowMsg;
		}
	}
}
