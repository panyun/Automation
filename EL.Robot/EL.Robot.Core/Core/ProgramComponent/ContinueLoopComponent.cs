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
    /// 继续循环组件
    /// </summary>
    public class ContinueLoopComponent : BaseComponent
    {
  

        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            dynamic robot = Boot.GetComponent<RobotComponent>();
            robot.ExecState = ExecState.IsContinue;
            self.Value = true;
            return self;

        }
    }
}
