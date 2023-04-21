using EL;
using EL.Overlay;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Inspect
{
	public enum CatchType
	{
		/// <summary>
		/// 普通节点
		/// </summary>
		Element,
		/// <summary>
		/// 窗口
		/// </summary>
		Window,
		/// <summary>
		/// 微信窗口
		/// </summary>
		WxChat
	}
	public class CatchUIRequest : IRequest
	{
		public int RpcId { get; set; }
		/// <summary>
		/// 界面探测器打开时显示信息
		/// </summary>
		public string Msg { get; set; }
		/// <summary>
		/// 捕获模式
		/// </summary>
		public Mode Mode { get; set; } = Mode.None;
		/// <summary>
		/// 捕获类型
		/// </summary>
		public CatchType CatchType { get; set; } = CatchType.Element;
	}
	public class CatchUIResponse : IResponse
	{
		public int Error { get; set; }
		public string Message { get; set; }
		public int RpcId { get; set; }
		public ElementPath ElementPath { get; set; }
		public string StackTrace { get; set; }
		[JsonIgnore]
		[BsonIgnore]
		public ElementIns CurrentElement { get; set; }

	}

}
