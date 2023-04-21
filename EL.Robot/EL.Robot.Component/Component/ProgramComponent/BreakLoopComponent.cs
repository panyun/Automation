using EL.Async;
using EL.Robot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EL.Robot.Component
{
    /// <summary>
    /// 跳出循环组件
    /// </summary>
    public class BreakLoopComponent : BaseComponent
    {

        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var robot = Boot.GetComponent("RobotComponent");
            robot.ExecState = ExecState.IsBreak;
            self.Value = true;
            return self;

        }
    }
}
