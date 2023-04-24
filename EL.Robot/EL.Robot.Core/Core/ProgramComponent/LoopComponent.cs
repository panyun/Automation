using Automation.Inspect;
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
    public class LoopComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var fortype = self.CurrentNode.GetParamterInt("fortype");
            var robot = Boot.GetComponent<RobotComponent>();
            switch (fortype)
            {
                case 1:
                    await LoopWhile(self, robot);
                    break;
                case 2:
                    await LoopFor(self, robot);
                    break;
                case 3:
                    await LoopSimilarElement(self, robot);
                    break;
                default:
                    break;
            }
            if (robot.ExecState == ExecState.IsBreak || robot.ExecState == ExecState.IsContinue)
                robot.ExecState = ExecState.None;
            self.Value = false; //不执行子流程
            return self;
        }
        /// <summary>
        /// 循环相似元素
        /// </summary>
        /// <param name="self"></param>
        /// <param name="robot"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task LoopSimilarElement(INodeContent self, dynamic robot)
        {
            //相似元素对象
            var fordom = self.CurrentNode.GetParamterValue<List<ElementUIA>>("fordom");
            List<object> elements = new List<object>();
            var domop = self.CurrentNode.GetParamterInt("domop");
            switch (domop)
            {
                case 1:
                    elements.AddRange(fordom);
                    break;
                case 2:
                    elements.AddRange(fordom.Select(el => el.Name));
                    break;
                case 3:
                    elements.AddRange(fordom.Select(el => el.Value));
                    break;
                case 4:
                    var propName = self.CurrentNode.GetParamterString("propname");
                    elements.AddRange(fordom.Select(el => UtilsComponent.GetPropertyVal(el, propName)));
                    break;
                default:
                    elements.AddRange(fordom);
                    break;
            }
            var flowComponent = robot.GetComponent("FlowComponent");
            var nodeComponent = flowComponent.GetComponent("NodeComponent");
            for (int i = 1; i <= elements.Count; i++)
            {
                //ElementActionComponent.Main(fordom[i - 1]);
                var msg = self.CurrentFlow.SetFlowParamBy(self.CurrentNode.OutParameterName, new ValueInfo(elements[i - 1]));
                flowComponent.WriteNodeLog(self.CurrentNode, msg);
                flowComponent.WriteNodeLog(self.CurrentNode, $"{i}次 循环开始");
                await nodeComponent.Exec(self.CurrentNode.Steps);
                flowComponent.WriteNodeLog(self.CurrentNode, $"{i}次 循环结束");
                if (robot.ExecState == ExecState.IsBreak || robot.ExecState == ExecState.IsStop)
                    break;
                if (robot.ExecState == ExecState.IsContinue)
                    continue;
            }
        }

        private async Task LoopWhile(INodeContent self, dynamic robot)
        {
            var exrp = self.CurrentNode.GetParamterValueExrp("ifexpression") + "";
            bool isIf = false;
            try
            {
                isIf = Evaluation(exrp);
            }
            catch (Exception)
            {
                throw new ELNodeHandlerException($"表达式不正确！{exrp}");
            }

            var flowComponent = robot.GetComponent("FlowComponent");
            var nodeComponent = flowComponent.GetComponent("NodeComponent");
            int i = 1;
            flowComponent.WriteNodeLog(self.CurrentNode, $"条件循环开始");
            while (isIf)
            {
                var msg = self.CurrentFlow.SetFlowParamBy(self.CurrentNode.OutParameterName, new ValueInfo(i));
                flowComponent.WriteNodeLog(self.CurrentNode, msg);
                flowComponent.WriteNodeLog(self.CurrentNode, $"{i}次 循环开始");
                await nodeComponent.Exec(self.CurrentNode.Steps);
                flowComponent.WriteNodeLog(self.CurrentNode, $"{i}次 循环结束");
                if (robot.ExecState == ExecState.IsBreak)
                    break;
                if (robot.ExecState == ExecState.IsContinue)
                    continue;
                i++;
            }
            flowComponent.WriteNodeLog(self.CurrentNode, $"条件循环结束");
        }

        private static async Task LoopFor(INodeContent self, dynamic robot)
        {
            var forNum = self.CurrentNode.GetParamterInt("forNum");
            var flowComponent = robot.GetComponent("FlowComponent");
            var nodeComponent = flowComponent.GetComponent("NodeComponent");
            flowComponent.WriteNodeLog(self.CurrentNode, $"次数循环开始");
            for (int i = 1; i <= forNum; i++)
            {
                var msg = self.CurrentFlow.SetFlowParamBy(self.CurrentNode.OutParameterName, new ValueInfo(i));
                flowComponent.WriteNodeLog(self.CurrentNode, msg);
                flowComponent.WriteNodeLog(self.CurrentNode, $"{i}次 循环开始");
                await nodeComponent.Exec(self.CurrentNode.Steps);
                flowComponent.WriteNodeLog(self.CurrentNode, $"{i}次 循环结束");
                if (robot.ExecState == ExecState.IsBreak)
                    break;
                if (robot.ExecState == ExecState.IsContinue)
                    continue;
            }
            flowComponent.WriteNodeLog(self.CurrentNode, $"次数循环结束");
        }

        public bool Evaluation(string expression)
        {
            ICodeCompiler comp = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
            CompilerParameters parms = new CompilerParameters();
            parms.GenerateInMemory = true;//是否在内存中生成space
            StringBuilder code = new StringBuilder();
            code.Append("using System; \n");
            code.Append("      public class Expression    {\n");
            code.Append("   public object Evaluation()        { ");
            code.AppendFormat($"  return (bool)({expression});");
            code.Append("}\n");
            code.Append("} ");
            System.CodeDom.Compiler.CompilerResults cr = comp.CompileAssemblyFromSource(parms, code.ToString());
            System.Reflection.Assembly a = cr.CompiledAssembly;
            object _Compiled = a.CreateInstance("Expression");
            System.Reflection.MethodInfo mi = _Compiled.GetType().GetMethod("Evaluation");
            return (bool)mi.Invoke(_Compiled, null);
        }
    }
}
