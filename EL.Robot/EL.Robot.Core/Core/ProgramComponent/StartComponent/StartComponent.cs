using EL.Async;
using EL.Robot.Core;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text;

namespace EL.Robot.Component
{
    public class StartRequest
    {

    }

    public class StartComponent : BaseComponent
    {
        public StartComponent()
        {
            DisplayName = "开始";
        }
        public ELTask<bool> WaitEvaluation = ELTask<bool>.Create();
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var robot = Boot.GetComponent<RobotComponent>();
            var flowComponent = robot.GetComponent<FlowComponent>();
            flowComponent.InitParam(self.CurrentNode);
            flowComponent.GetSteps();
            self.Value = true;
            return self;
        }


    }
}

