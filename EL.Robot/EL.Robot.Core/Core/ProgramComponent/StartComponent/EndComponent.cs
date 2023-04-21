using EL.Async;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text;

namespace EL.Robot.Component
{

    public class EndComponent : BaseComponent
    {
        public ELTask<bool> WaitEvaluation = ELTask<bool>.Create();
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            DisplayName = self.CurrentNode.Name;
            self.Value = true;
            return self;
        }
    }
}

