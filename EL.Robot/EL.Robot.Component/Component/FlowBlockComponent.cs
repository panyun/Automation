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

        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            self.Value = true;
            return self;
        }
    }
}
