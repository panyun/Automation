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
        public string VariableName { get; set; }
        public string VariableValue { get; set; }
        public SetVariableComponent()
        {
            Config.Category = Category.基础组件;
        }
        public override Config GetConfig()
        {
            if (Config.IsInit) return Config;
            Config.Parameters = new List<Parameter>()
            {
                new Parameter()
                {
                    Key = nameof(VariableName),
                    DisplayName = "变量名称",
                    Value = "",
                    Title = "请给变量取一个名字",
                    Type = new List<Type>(){ typeof(string) },
                    IsInput = true,
                    Values =VariableSystem.InputOrSelectOrUpValues,
                    Parameters = new List<Parameter>()
                    {
                        new Parameter()
                        {
                            Key =nameof(VariableValue),
                            DisplayName = "赋值",
                            Value = "",
                            Type = new List<Type>(){ typeof(string),typeof(int) },
                            IsInput = true,
                            Title = "请给变量赋一个初始值",
                            Values = new List <ValueInfo>() {
                                VariableSystem.InputVariable,
                                VariableSystem.SelectVariable
                            },
                        },

                    }
                },
            };
            Config.ButtonDisplayName = "设置变量";
            return base.GetConfig();
        }
        public override ELTask<INodeContent> Main(INodeContent self)
        {
            VariableName = self.CurrentNode.GetParamterString(nameof(VariableName));
            VariableValue = self.CurrentNode.GetParamterString(nameof(VariableValue));
            self.CurrentFlow.SetFlowParamBy(VariableName, VariableValue);
            self.Out = true;
            self.Value = true;
            return base.Main(self);
        }
    }
}
