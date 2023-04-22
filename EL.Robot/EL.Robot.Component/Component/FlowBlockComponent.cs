using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.Component
{
    public class FlowBlockComponent: BaseComponent
    {
		public FlowBlockComponent()
		{
			Config.Category = Category.基础函数;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.DisplayName = "流程块";
			return base.GetConfig();
		}
		public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            self.Value = true;
            return self;
        }
    }
}
