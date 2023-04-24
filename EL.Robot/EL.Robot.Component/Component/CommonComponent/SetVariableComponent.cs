using Automation.Inspect;
using EL.Async;
using EL.Robot.Component.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace EL.Robot.Component
{

	public class SetVariableComponent : BaseComponent
	{
		public ValueInfo VariableName { get; set; }
		public ValueInfo VariableValue { get; set; }
		public SetVariableComponent()
		{
			Config.Category = Category.基础组件;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.ButtonDisplayName = "设置变量";
			Config.CmdDisplayName = "创建变量并赋值";
			Config.Parameters = new List<Parameter>()
			{
				new Parameter()
				{
					Key = nameof(Node.OutParameterName),
					DisplayName = "变量名称",
					CmdDisplayName="变量的名称",
					Value = VariableSystem.OutParameterNameValue("var"),
					Title = "请给变量取一个名字",
					Type = new List<Type>(){ typeof(string) },
					IsInput = true,
					Values = VariableSystem.OutParameterNameValues("var"),
					Parameters = new List<Parameter>()
					{
						new Parameter()
						{
							Key =nameof(VariableValue),
							DisplayName = "赋值",
							CmdDisplayName="初始值",
							Value = VariableSystem.InputVariable("val"),
							Type = new List<Type>(){ typeof(string),typeof(int) },
							IsInput = true,
							Title = "请给变量赋一个初始值",
							Values = new List <ValueInfo>() {
								VariableSystem.InputVariable("val"),
								VariableSystem.SelectVariable()
							},
						},

					}
				},
			};

			return base.GetConfig();
		}
		public override ELTask<INodeContent> Main(INodeContent self)
		{
			VariableValue = self.CurrentNode.GetParameterValueInfo(nameof(VariableValue));
			self.Out = VariableValue;
			self.Value = true;
			return base.Main(self);
		}
	}
}
