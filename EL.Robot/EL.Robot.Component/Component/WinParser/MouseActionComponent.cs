using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Component.DTO;
using System.Diagnostics;

namespace EL.Robot.Component
{
	/// <summary>
	/// 节点高亮
	/// </summary>
	public class MouseActionComponent : BaseComponent
	{
		public MouseActionComponent()
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
							Key = "actiontype",
							DisplayName = "点击方式",
							Value = "Mouse",
							Type = new List<Type>(){ typeof(string) },
							IsInput = true,
							Title = "actiontype",
							Values = new List < ValueInfo > {
								new ValueInfo() {DisplayName = "模拟点击",
								Value = ActionType.Mouse.ToString(),
								AcationType = ValueActionType.Value,
								},
								new ValueInfo() {DisplayName = "事件点击",
								Value = ActionType.ElementEvent.ToString(),
								AcationType = ValueActionType.Value,
								},
							},
							 Parameters= new List<Parameter>()
							 {
								 new Parameter()
								 {
									Key = "clicktype",
									DisplayName = "点击类型",
									Value = "clicktype",
									Type = new List<Type>(){ typeof(string) },
									IsInput = true,
									Title = "点击类型",
									Values = new List<ValueInfo>
									 {
										new ValueInfo{
											 AcationType = ValueActionType.Value,
											 DisplayName = "左键点击",
											  Value=ClickType.LeftClick,
										},
										new ValueInfo{
											 AcationType = ValueActionType.Value,
											  DisplayName = "右键点击",
											  Value=ClickType.RightClick,
										},
										new ValueInfo{
											 AcationType = ValueActionType.Value,
											  DisplayName = "双击",
											  Value=ClickType.LeftDoubleClick,
										}
									 }
								 }
							 }
						},

					}
				},
			};
			Config.DisplayName = "元素点击";
			return base.GetConfig();
		}
		public override async ELTask<INodeContent> Main(INodeContent self)
		{
			await base.Main(self);
			DisplayName = self.CurrentNode.Name;
			var element = self.CurrentNode.GetParamterValue("ElementPath");
			var actiontypeStr = self.CurrentNode.GetParamterValue("actiontype") + "";
			var clicktypeStr = self.CurrentNode.GetParamterValue("clicktype") + "";
			Enum.TryParse(actiontypeStr, true, out ActionType actionType);
			Enum.TryParse(clicktypeStr, true, out ClickType clickType);
			MouseActionRequest request = new()
			{
				ActionType = actionType,
				ClickType = clickType,
				LocationType = LocationType.Center,
				OffsetX = 0,
				OffsetY = 0,
				TimeOut = 10000
			};
			if (element is ElementPath)
			{
				var elementPath = element as ElementPath;
				if (elementPath == null)
					throw new ELNodeHandlerException("elementPath is null!");
				request.ElementPath = elementPath;
				await UtilsComponent.Exec(request);
			}
			else if (element is ElementUIA ele)
			{
				MouseActionSystem.UIAMain(request, new List<ElementUIA>() { ele });
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

