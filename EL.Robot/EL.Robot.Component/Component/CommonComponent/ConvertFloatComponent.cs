using EL.Async;

namespace EL.Robot.Component
{
    public class ConvertFloatComponent : BaseComponent
    {
        public int Length { get; set; }
        public ConvertFloatComponent()
        {
            Config.Category = Category.基础组件;
        }
        public override Config GetConfig()
        {
            if (Config.IsInit) return Config;
            Config.ButtonDisplayName = "转小数";
            Config.Parameters = new List<Parameter>()
            {
                new Parameter()
                {
                    Key = nameof(Length),
                    DisplayName = "保留长度",
                    Value = "",
                    Title = "转换的小数保留长度",
                    Type = new List<Type>(){ typeof(int) },
                    IsInput = true,
                    Values = VariableSystem.InputOrSelectOrUpValues,
                    Parameters = new List<Parameter>()
                    {
                        new Parameter()
                        {
                            Key =nameof(Node.OutParameterName),
                            DisplayName = "输出变量的名称是",
                            Value = "",
                            Type = new List<Type>(){ typeof(string),typeof(int) },
                            IsInput = true,
                            Title = "请给变量命名",
                            Values = VariableSystem.OutParameterNameValues
                        },

                    }
                },
            };
            return base.GetConfig();
        }
        public override ELTask<INodeContent> Main(INodeContent self)
        {
            if (string.IsNullOrEmpty(VariableSystem.UpNode.OutParameterName))
                throw new ELNodeHandlerException("上一条没有输出结果值");
            Length = self.CurrentNode.GetParamterInt(nameof(Length));
            self.CurrentNode.OutParameterName = VariableSystem.UpNode.OutParameterName;
            var upOut = self.CurrentFlow.GetFlowParamterValue(self.CurrentNode.OutParameterName);
            double.TryParse(upOut.ToString(), out double val);
            val = Math.Round(val, Length);
            self.Out = val;
            self.Value = true;
            return base.Main(self);
        }
    }
}
