using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Component.DTO;

namespace EL.Robot.Component
{
	/// <summary>
	/// 节点高亮
	/// </summary>
	public class InputActionComponent : BaseComponent
	{
		public InputActionComponent()
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
					Key = "ElementPath",
					DisplayName = "捕获目标",
					Value = "ElementPath",
					Title = "捕获目标",
					Type = new List<Type>(){ typeof(string) },
					IsInput = true,
					Values = new List < ValueInfo > {
						new ValueInfo()
						{
							DisplayName = "捕获目标",
							Value = "",
							AcationType =  ValueActionType.RequestValue,
							Action = new CommponetRequest()
							{
								ComponentName = nameof(CatchElementComponent)
							}
						}
					},
					Parameters = new List<Parameter>()
					{
						new Parameter()
						{
							Key = "InputTxt",
							DisplayName = "输入目标",
							Value = "InputTxt",
							Type = new List<Type>(){ typeof(string) },
							IsInput = true,
							Title = "匹配插入",
							Values = new List < ValueInfo > {
								new ValueInfo() {DisplayName = "选择变量",
								Action = new SelectVariableRequest()
								{
									 ComponentName= "SelectVariable",
									  Types = new List<Type>()
									   {
										   typeof(string)
									   },
								},
								AcationType = ValueActionType.RequestList,
								},
								new ValueInfo()
								{
									 DisplayName="输入变量",
									  AcationType = ValueActionType.Input,
									   Types = new List<Type>(){ typeof(string) }
								}
							},
							 Parameters= new List<Parameter>()
							 {
								 new Parameter()
								 {
									Key = "inputType",
									DisplayName = "输入类型",
									Value = "inputType",
									Type = new List<Type>(){ typeof(string) },
									IsInput = true,
									Title = "输入类型",
									Values = new List<ValueInfo>
									 {
										new ValueInfo{
											 AcationType = ValueActionType.Value,
											 DisplayName = "键盘输入",
											  Value=InputType.Keyboard.ToString(),
										},
										new ValueInfo{
											 AcationType = ValueActionType.Value,
											  DisplayName = "事件输入",
											  Value=InputType.ElementInput,
										},
										new ValueInfo{
											 AcationType = ValueActionType.Value,
											  DisplayName = "复制粘贴",
											  Value=InputType.Paste,
										}
									 }
								 }
							 }
						},

					}
				},
			};
			Config.DisplayName = "元素输入";
			return base.GetConfig();
		}
		public override async ELTask<INodeContent> Main(INodeContent self)
		{
			await base.Main(self);
			DisplayName = self.CurrentNode.Name;
			object element = default;
			try
			{
				element = self.CurrentNode.GetParamterValue("ElementPath");
			}
			catch (Exception ex)
			{
				throw new ELNodeHandlerException($"获取属性[ElementPath]为空！");
			}
			var inputTxt = self.CurrentNode.GetParamterValue("InputTxt") + "";
			var inputtypeStr = self.CurrentNode.GetParamterValue("inputtype") + "";
			Enum.TryParse(inputtypeStr, out InputType inputtype);
			bool.TryParse(self.CurrentNode.GetParamterValue("isclear") + "", out var isclear);
			int.TryParse(self.CurrentNode.GetParamterValue("TimeOut") + "", out var timeOut);
			InputActionRequest request = new()
			{
				InputTxt = inputTxt,
				InputType = inputtype,
				IsClear = isclear,
				TimeOut = timeOut
			};
			if (element is ElementPath elementPath)
			{
				request.ElementPath = elementPath;
				await UtilsComponent.Exec(request);
			}
			else if (element is ElementUIA ele)
			{
				InputActionSystem.UIAMain(request, new List<ElementUIA>() { ele });
			}
			else
			{
				throw new ELNodeHandlerException("引用参数的类型不匹配！");
			}

			self.Value = true;
			return self;
		}
	}

}

