using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Core;
using System.Diagnostics;
using System.Reflection;

namespace EL.Robot.Component
{
    /// <summary>
    /// 节点高亮
    /// </summary>
    public class ElementPropertyActionComponent : BaseComponent
    {
      
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            object element = default;
            try
            {
                element = self.CurrentNode.GetParamterValue("in_var");
            }
            catch (Exception ex)
            {
                throw new ELNodeHandlerException($"获取属性[ElementPath]为空！");
            }
            ElementPath Path = default;
            ElementPropertyActionRequest request = new ElementPropertyActionRequest();
            if (element is ElementPath elementPath)
            {
                request.ElementPath = elementPath;
                var res = (ElementActionResponse)await UtilsComponent.Exec(request);
                self.Value = res.Error == 0;
                Path = res.ElementPath;
            }
            else if (element is ElementUIA elementWin)
            {
                Path = request.Main(elementWin);
                self.Value = true;
            }
            else
            {
                throw new ELNodeHandlerException("引用参数的类型不匹配！");
            }
            if (Path == default) throw new ELNodeHandlerException("未找到节点");
            var domop = self.CurrentNode.GetParamterInt("domop");
            string val = string.Empty;
            if (domop == 1) val = nameof(Path.Name);
            else if (domop == 2) val = nameof(Path.Value);
            else if (domop == 3) val = self.CurrentNode.GetParamterString("propname");
            val = UtilsComponent.GetPropertyVal(Path, val);
            self.Out = val;
            return self;
        }
    }
}

