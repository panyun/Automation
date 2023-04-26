using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component
{

    public class CommentComponent : BaseComponent
    {
        public string Comment { get; set; }
        public CommentComponent()
        {
            Config.Category = Category.基础组件;
        }
        public override Config GetConfig()
        {
            if (Config.IsInit) return Config;
            Config.ButtonDisplayName = "注释";
            Config.CmdDisplayName = "给组件添加注释";
            Config.Parameters = new List<Parameter>()
            {
                new Parameter()
                {
                    Key = nameof(Comment),
                    DisplayName = "注释内容",
                    Title = "注释内容",
                    Value =VariableSystem.InputVariable("这是对组件的说明和备注！"),
                    Types = new List<Type>(){ typeof(int) },
                    IsInput = true,
                    Values = new List<ValueInfo>(){ VariableSystem.InputVariable("这是对下一个组件的说明和备注！解析组件完成的工作和特殊说明。")},
                },
            };
            return base.GetConfig();
        }
        public override ELTask<INodeContent> Main(INodeContent self)
        {

            self.Value = true;
            return base.Main(self);
        }
    }
}
