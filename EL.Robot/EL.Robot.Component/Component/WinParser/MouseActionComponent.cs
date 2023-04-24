using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Component.DTO;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using System.Drawing;
using WpfInspect.Models;

namespace EL.Robot.Component
{
	/// <summary>
	/// 节点高亮
	/// </summary>
	public class MouseActionComponent : BaseComponent
	{
		public ElementPath ElementPath { get; set; }
		public ActionType ActionType { get; set; }
		public ClickType ClickType { get; set; }
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
					Key =nameof(ElementPath),
					DisplayName = "捕获目标",
					Title = "捕获目标",
					Type = new List<Type>(){ typeof(string) },
					IsInput = true,
					Values = new List < ValueInfo > {
						new ValueInfo()
						{
							DisplayName = "捕获目标",
							Value = "",
							ActionType =  ValueActionType.RequestValue,
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
							Key = nameof(ActionType),
							DisplayName = "点击方式",
							Type = new List<Type>(){ typeof(ActionType) },
							IsInput = true,
							Value=new ValueInfo() {DisplayName = "模拟点击",
								Value = ActionType.Mouse.ToString(),
								ActionType = ValueActionType.Value,
								},
							Title = "actiontype",
							Values = new List < ValueInfo > {
								new ValueInfo() {DisplayName = "模拟点击",
								Value = ActionType.Mouse.ToString(),
								ActionType = ValueActionType.Value,
								},
								new ValueInfo() {DisplayName = "事件点击",
								Value = ActionType.ElementEvent.ToString(),
								ActionType = ValueActionType.Value,
								},
							},
							 Parameters= new List<Parameter>()
							 {
								 new Parameter()
								 {
									Key = nameof(ClickType),
									DisplayName = "点击类型",
									Type = new List<Type>(){ typeof(string) },
									Value = new ValueInfo{
											 ActionType = ValueActionType.Value,
											 DisplayName = "左键点击",
											  Value=ClickType.LeftClick,
										},
									IsInput = true,
									Title = "点击类型",
									Values = new List<ValueInfo>
									 {
										new ValueInfo{
											 ActionType = ValueActionType.Value,
											 DisplayName = "左键点击",
											  Value=ClickType.LeftClick,
										},
										new ValueInfo{
											 ActionType = ValueActionType.Value,
											  DisplayName = "右键点击",
											  Value=ClickType.RightClick,
										},
										new ValueInfo{
											 ActionType = ValueActionType.Value,
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
			Config.ButtonDisplayName = "元素点击";
			return base.GetConfig();
		}
		public override async ELTask<INodeContent> Main(INodeContent self)
		{
			await base.Main(self);
			ElementPath = self.CurrentNode.GetParamterValue<ElementPath>(nameof(ElementPath));
			Light(ElementPath, 10000);
			ActionType = self.CurrentNode.GetParamterValue<ActionType>(nameof(ActionType));
			ClickType = self.CurrentNode.GetParamterValue<ClickType>(nameof(ClickType));
			MouseActionRequest request = new()
			{
				ActionType = ActionType,
				ClickType = ClickType,
				LocationType = LocationType.Center,
				OffsetX = 0,
				OffsetY = 0,
				TimeOut = VariableSystem.TimeOut,
				ElementPath = ElementPath,
			};
			self.Out = new ValueInfo(await UtilsComponent.Exec(request));
			self.Value = true;
			return self;
		}
		public static async void Light(ElementPath element, int timeOut)
		{
			if (element is ElementPath elementPath)
			{
				ElementActionRequest requestlight = new();
				requestlight.LightProperty = new LightProperty()
				{
					ColorName = nameof(Color.Red),
					Count = 1,
					Time = 100
				};
				requestlight.TimeOut = timeOut;
				requestlight.ElementPath = elementPath;
				await UtilsComponent.Exec(requestlight);
			}
		}
	}
}

