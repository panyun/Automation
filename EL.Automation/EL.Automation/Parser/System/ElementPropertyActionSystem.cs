using Automation.Inspect;
using EL;
using EL.Async;
using EL.Input;
using EL.Overlay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
    public static class ElementPropertyActionSystem
    {
        public static ElementPath Main(this ElementPropertyActionRequest self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winInspect.GetComponent<WinPathComponent>();
            var elements = self.AvigationElement();
            var element = elements.FirstOrDefault();
            return self.Main(element as ElementUIA);
        }
        public static ElementPath Main(this ElementPropertyActionRequest self, ElementUIA elementWin)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winInspect.GetComponent<WinPathComponent>();
            if (elementWin == null) throw new ParserException("未定位到目标元素！");
            return pathComponent.GetPathInfo(elementWin.NativeElement);
        }
    }
}
