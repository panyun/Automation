using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
    public static class ChildsElementActionSystem
    {
        public static async ELTask<IEnumerable<Element>> Main(this ChildsElementActionRequest self)
        { 
            await ELTask.CompletedTask;
            if (self.ElementType == ElementType.UIAUI)
            {
                var element = self.AvigationElement().FirstOrDefault() as ElementUIA;
                var inspect = Boot.GetComponent<InspectComponent>();
                var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
                var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
                var eles = winfromInspect.GetChilds(element.NativeElement);
                self?.LightProperty.LightHighMany(default, eles.ToArray());
                return eles;
            }
            throw new ParserException("未实现的方法！");
        }
    }
}
