using Automation;
using EL;
using EL.Async;
using EL.Robot.Component;
using EL.Robot;
using SixLabors.ImageSharp;
using System.Security.Cryptography.X509Certificates;

namespace EL.Robot.Component
{
	public interface INodeContent
	{
		public Flow CurrentFlow { get; set; }
		public Node CurrentNode { get; set; }
		public bool Value { get; set; }
		/// <summary>
		/// 输出参数Value
		/// </summary>
		public ValueInfo Out { get; set; }
		public List<string> Msg { get; set; }
	}
	public class NodeContent : INodeContent
	{
		public List<string> Msg { get; set; }
		/// <summary>
		/// 当前流程
		/// </summary>
		public Flow CurrentFlow { get; set; }
		/// <summary>
		/// 当前节点
		/// </summary>
		public Node CurrentNode { get; set; }
		/// <summary>
		/// 返回值
		/// </summary>
		public bool Value { get; set; }
		/// <summary>
		/// 输出参数Value
		/// </summary>
		public ValueInfo Out { get; set; }
		public static NodeContent Create(Flow flow, Node node)
		{
			return new NodeContent()
			{
				CurrentFlow = flow,
				CurrentNode = node
			};
		}
	}
	public interface INodeCompoonent
	{
		public Config Config { get; set; }
		public Dictionary<string, ValueInfo> Parameter { get; set; }
		public ELTask<INodeContent> Main(INodeContent self);
		public Config GetConfig();
		public ParameterInfo GetExpression(CommponetRequest commponetRequest, Dictionary<string, ValueInfo> paramsManager);
	}
	public class BaseComponent : Entity, INodeCompoonent
	{
		public BaseComponent()
		{
			Config = new Config
			{
				ComponentName = this.GetType().Name
			};
          
        }
		public Config Config { get; set; }

		public string DisplayName { get; set; }
		public Dictionary<string, ValueInfo> Parameter { get; set; } = new Dictionary<string, ValueInfo>();
		public virtual ParameterInfo GetExpression(CommponetRequest commponetRequest, Dictionary<string, ValueInfo> paramsManager)
		{
			return default;
		}
		public virtual Config GetConfig()
		{
			Config.OutParameterName = Config.ComponentName.TrimEnd("Component".ToArray());
			Config.DefalutValue = string.Empty;
			if (string.IsNullOrEmpty(Config.CmdDisplayName))
				Config.CmdDisplayName = Config.ButtonDisplayName;
			return Config;
		}
		public virtual async ELTask<INodeContent> Main(INodeContent self)
		{
			await ELTask.CompletedTask;
			//self.CurrentNode.OutParameterName ??= Config.ComponentName + "Instance" + Parameter.Count;
			//if (Parameter.ContainsKey(self.CurrentNode.OutParameterName))
			//{
			//	Parameter[self.CurrentNode.OutParameterName] = self.Out;
			//}
			//else
			//{
			//	Parameter.Add(self.CurrentNode.OutParameterName, self.Out);
			//}
			//self.Value = true;
			return self;
		}
	}
	public class Config
	{
		/// <summary>
		/// 组件名称
		/// </summary>
		public string ComponentName { get; set; }
		/// <summary>
		/// 显示名称
		/// </summary>
		public string ButtonDisplayName { get; set; }
		public string CmdDisplayName { get; set; }
		/// <summary>
		/// 图片
		/// </summary>
		public Image Image { get; set; }
		/// <summary>
		/// 默认值
		/// </summary>
		public object DefalutValue { get; set; }
		/// <summary>
		/// 组件参数
		/// </summary>
		public List<Parameter> Parameters { get; set; } = new List<Parameter>();
		/// <summary>
		/// 输出参数名称
		/// </summary>
		public string OutParameterName { get; set; }
		public Category Category { get; set; }
		public bool IsInit { get; set; } = false;
		public bool IsView { get; set; } = true;
	}
}

public static class CategoryHelper
{
	public static List<dynamic> Categorys = new();
	static CategoryHelper()
	{
		Categorys.Add(new
		{
			CategoryName = Category.流程控制.ToString(),
			CategoryId = (int)Category.流程控制,
			Image = string.Empty
		});
		Categorys.Add(new
		{
			CategoryName = Category.UI自动化.ToString(),
			CategoryId = (int)Category.UI自动化,
			Image = string.Empty
		});
		Categorys.Add(new
		{
			CategoryName = Category.基础组件.ToString(),
			CategoryId = (int)Category.基础组件,
			Image = string.Empty
		});
		Categorys.Add(new
		{
			CategoryName = Category.Excel.ToString(),
			CategoryId = (int)Category.Excel,
			Image = string.Empty
		});
	}

}
public enum Category
{
	流程控制=1,
	UI自动化,
	基础组件,
	Excel
}
