using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.Component.WinParser
{
	public class ElementPropertyComponent : BaseComponent
	{
		public ElementPath ElementPath { get; set; }
		public string PropertyName { get; set; }
		public ElementPropertyComponent()
		{
			Config.Category = Category.UI自动化;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.Parameters = new List<Parameter>()
			{
				new Parameter()
				{
					Key = nameof(ElementPath),
					DisplayName = "捕获目标",
					Title = "捕获目标",
					Value = VariableSystem.CatchVariable(),
					Types = new List<Type>(){ typeof(ElementPath) },
					IsInput = true,
					Values = new List<ValueInfo>()
					{
						VariableSystem.CatchVariable(),
						VariableSystem.SelectVariable(typeof(ElementPath)),
					},
					Parameters = new List<Parameter>()
					{
						new Parameter()
						{
							Key = nameof(PropertyName),
							DisplayName = "属性名称",
							Types = new List<Type>(){ typeof(ElementPath) },
							IsInput = true,
							Title = "属性的名称",
							Value = new ValueInfo{
											 ActionType = ValueActionType.Value,
											 DisplayName = "元素名称",
											  Value=nameof(ElementPath.Name),
										},
							Values = new List<ValueInfo>
									 {
										new ValueInfo{
											 ActionType = ValueActionType.Value,
											 DisplayName = "元素名称",
											  Value=nameof(ElementPath.Name),
										},
										new ValueInfo{
											 ActionType = ValueActionType.Value,
											  DisplayName = "元素值",
											  Value=nameof(ElementPath.Value),
										},
										new ValueInfo{
											 ActionType = ValueActionType.Value,
											  DisplayName = "元素坐标",
											  Value=nameof(Point),
										}
									 },
							Parameters = new List<Parameter>()
							{
								new Parameter()
									{
										Key = nameof(Node.OutParameterName),
										DisplayName = "保存到变量",
										Types = new List<Type>(){ typeof(ElementPath) },
										IsInput = true,
										Title = "保存到变量",
										Value = VariableSystem.OutParameterNameValue("elementInfo"),
										Values = new List<ValueInfo>
												 {
													VariableSystem.OutParameterNameValue("elementInfo")
												 }
									}
							}

						},

					}
				},
			};
			Config.ButtonDisplayName = "获取信息";
			return base.GetConfig();
		}
		public override async ELTask<INodeContent> Main(INodeContent self)
		{
			await base.Main(self);
			ElementPath = self.CurrentNode.GetParamterValue<ElementPath>(nameof(ElementPath));
			//Light(ElementPath, 10000);
			PropertyName = self.CurrentNode.GetParamterValue<string>(nameof(PropertyName));

			ElementPropertyActionRequest request = new()
			{
				ElementPath = ElementPath,
			};
			self.Out = new ValueInfo(await UtilsComponent.Exec(request));
			self.Value = true;
			return self;
		}
	}
}
