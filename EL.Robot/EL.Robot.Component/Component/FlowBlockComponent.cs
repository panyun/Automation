using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component
{
    public class FlowBlockComponent: BaseComponent
    {
		public FlowBlockComponent()
		{
			//Config.Category = Category.基础组件;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.ButtonDisplayName = "流程块";
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
