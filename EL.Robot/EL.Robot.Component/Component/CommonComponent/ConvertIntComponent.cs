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
            Config.Parameters = new List<Parameter>()
            {
                new Parameter()
                {
                    Key = nameof(Node.OutParameterName),
                    DisplayName = "保留长度",
                    Value = "",
                    Title = "转换的小数保留长度",
                    Type = new List<Type>(){ typeof(int) },
                    IsInput = true,
                    Values = VariableSystem.OutParameterNameValues,
                },
            };
            return base.GetConfig();
        }
        public override ELTask<INodeContent> Main(INodeContent self)
        {
            if (string.IsNullOrEmpty(VariableSystem.UpNode.OutParameterName))
                throw new ELNodeHandlerException("上一条没有输出结果值");
            if (string.IsNullOrEmpty(self.CurrentNode.OutParameterName))
            {
                self.CurrentNode.OutParameterName = VariableSystem.UpNode.OutParameterName;
            }
            var upOut = self.CurrentFlow.GetFlowParamterValue(self.CurrentNode.OutParameterName);
            int.TryParse(upOut.ToString(), out int val);
            self.Out = val;
            self.Value = true;
            return base.Main(self);
        }
    }
}
