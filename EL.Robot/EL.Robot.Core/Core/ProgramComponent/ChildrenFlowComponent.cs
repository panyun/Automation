using EL.Async;
using EL.Robot.Core;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EL.Robot.Component
{
    public class ChildrenFlowComponent : BaseComponent
    {


        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var childrenflowfilepath = self.CurrentNode.GetParamterInt("childrenflowfilepath");
            var flow = self.CurrentFlow.ChildrenFlows.FirstOrDefault(x => x.Id == childrenflowfilepath);
            var robot = Boot.GetComponent<RobotComponent>();
            var flowComponent = robot.GetComponent<FlowComponent>();
            var nodeComponent = flowComponent.GetComponent<NodeComponent>();
            flowComponent.CurrentFlow = flow;
            //设置参数
            flow.InParams = new List<Parameter>();
            foreach (var item in self.CurrentNode.InParams)
            {
                if (item.Value == null) item.Value = default;
                var val = item.Value.ToString().Trim();
                if (val.StartsWith("@"))
                    item.Value = self.CurrentNode.Flow.GetFlowParamterValue(val);
                flow.InParams.Add(item);
            }
            if (!string.IsNullOrWhiteSpace(self.CurrentNode.OutParameterName))
            {
                var outparamter = self.CurrentNode.GetVariable(self.CurrentNode.OutParameterName);
                if (outparamter == null) outparamter = new Variable()
                {
                    Name = self.CurrentNode.OutParameterName,
                    Value = default
                };
                flow.OutParams = new List<Parameter>
                {
                    new Parameter() { Key = outparamter.Name, Value = outparamter.Value }
                };
            }
            //设置输入参数
            await nodeComponent.Exec(flow.Steps);
            self.Out = flowComponent.CurrentFlow.GetFlowParamterValue(self.CurrentNode.OutParameterName);
            flowComponent.CurrentFlow = flowComponent.MainFlow;
            self.Value = true;
            return self;

        }
    }
}
