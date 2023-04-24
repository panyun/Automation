using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component
{
	public class ConvertIntComponent : BaseComponent
	{

		public ConvertIntComponent()
		{
			Config.Category = Category.基础组件;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.ButtonDisplayName = "转整数";
			Config.CmdDisplayName = "将上一步组结果转整数";
			Config.Parameters = new List<Parameter>()
			{
				new Parameter()
				{
					Key = nameof(Node.OutParameterName),
					DisplayName = "保存到变量",
					Value = VariableSystem.OutParameterNameValue("varInt"),
					CmdDisplayName ="输出到变量",
					Title = "转换的小数保留长度",
					Type = new List<Type>(){ typeof(int) },
					IsInput = true,
					Values = VariableSystem.OutParameterNameValues("varInt"),
				},
			};
			return base.GetConfig();
		}
		public override ELTask<INodeContent> Main(INodeContent self)
		{
			if (string.IsNullOrEmpty(VariableSystem.UpNode.OutParameterName))
				throw new ELNodeHandlerException("上一条没有输出结果值");
			if (string.IsNullOrEmpty(self.CurrentNode.OutParameterName))
				self.CurrentNode.OutParameterName = VariableSystem.UpNode.OutParameterName;
			var upOut = self.CurrentFlow.GetFlowParamterValue(self.CurrentNode.OutParameterName);
			self.Out = upOut;
			self.Value = true;
			return base.Main(self);
		}
	}
}
