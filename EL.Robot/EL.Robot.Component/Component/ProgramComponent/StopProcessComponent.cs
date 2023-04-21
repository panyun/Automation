using EL.Async;
using EL.Robot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.Component
{
    public class StopFlowComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var robot = Boot.GetComponent("RobotComponent");
            robot.ExecState = ExecState.IsStop;
            DisplayName = self.CurrentNode.Name;
            self.Value = true;
            return self;
        }
    }
}
