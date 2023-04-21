using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
    public static class ParentElementActionSystem
    {
        public static async ELTask<ElementPath> Main(this ParentElementActionRequest self)
        {
            await ELTask.CompletedTask;
            if (self.ElementType == ElementType.UIAUI)
            {
                var element = self.AvigationElement().FirstOrDefault() as ElementUIA;
                var inspect = Boot.GetComponent<InspectComponent>();
                var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
                var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
                var ele = winfromInspect.GetParent(element.NativeElement).Convert();
                self?.LightProperty.LightHighMany(ele);
                var elementPath = pathComponent.GetPathInfo_ByRuntime(ele.NativeElement, self.ElementPath);
                if (self?.ElementPath?.PathNode != default)
                    elementPath.PathNode = self.ElementPath.PathNode;
                elementPath.PathNode.CurrentElementWin= ele;
                return elementPath;
            }
            throw new ParserException("未实现的方法！");
        }
    }
}
