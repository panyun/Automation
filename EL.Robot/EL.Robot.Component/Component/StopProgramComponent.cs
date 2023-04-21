using EL.Async;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace EL.Robot.Component
{

    public class StopProgramComponent : BaseComponent
    {

        public ELTask<bool> WaitEvaluation = ELTask<bool>.Create();
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var paramName = self.CurrentNode.GetParamterValue("targetApp") + "";
            if (!string.IsNullOrEmpty(paramName))
            {
                var obj = self.CurrentNode.GetParamterValue(paramName);
                int.TryParse(obj + "", out int processid);
                if (processid != default)
                {
                    var process = Process.GetProcessById(processid);
                    process.Kill();
                    self.Value = true;
                    return self;
                }
            }
            throw new ELNodeHandlerException("未找到命令行参数和执行路径！");
        }
    }
}

