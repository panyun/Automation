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
        public ElementPath ElementPath { get; set; }
        public string InputTxt { get; set; }
        public InputType InputType { get; set; }
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
                            Key = nameof(InputTxt),
                            DisplayName = "输入目标",
                            Types = new List<Type>(){ typeof(ElementPath) },
                            IsInput = true,
                            Title = "匹配插入",
                            Value = VariableSystem.InputVariable("测试字符串"),
                            Values = VariableSystem.InputOrSelectOrUpValues(),
                             Parameters= new List<Parameter>()
                             {
                                 new Parameter()
                                 {
                                    Key = nameof(InputType),
                                    DisplayName = "输入类型",
                                    Types = new List<Type>(){ typeof(string),typeof(InputType) },
                                    IsInput = true,
                                    Value = new ValueInfo{
                                             ActionType = ValueActionType.Value,
                                             DisplayName = "键盘输入",
                                              Value=InputType.Keyboard.ToString(),
                                        },
                                    Title = "输入类型",
                                    Values = new List<ValueInfo>
                                     {
                                        new ValueInfo{
                                             ActionType = ValueActionType.Value,
                                             DisplayName = "键盘输入",
                                              Value=InputType.Keyboard.ToString(),
                                        },
                                        new ValueInfo{
                                             ActionType = ValueActionType.Value,
                                              DisplayName = "事件输入",
                                              Value=InputType.ElementInput.ToString(),
                                        },
                                        new ValueInfo{
                                             ActionType = ValueActionType.Value,
                                              DisplayName = "复制粘贴",
                                              Value=InputType.Paste.ToString(),
                                        }
                                     }
                                 }
                             }
                        },

                    }
                },
            };
            Config.ButtonDisplayName = "元素输入";
            return base.GetConfig();
        }
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            DisplayName = self.CurrentNode.Name;

            ElementPath = self.CurrentNode.GetParamterValue<ElementPath>(nameof(ElementPath));
            InputTxt = self.CurrentNode.GetParamterValue<string>(nameof(InputTxt)) + "";
            InputType = self.CurrentNode.GetParamterValue<InputType>(nameof(InputType));
            InputActionRequest request = new()
            {
                InputTxt = InputTxt,
                ElementPath = ElementPath,
                InputType = InputType,
                IsClear = true,
                TimeOut = VariableSystem.TimeOut,
            };
            self.Out = new ValueInfo(await UtilsComponent.Exec(request));
            self.Value = true;
            return self;
        }
    }

}

