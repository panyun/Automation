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
using System.Windows.Forms;
using Interop.UIAutomationClient;

namespace Automation.Parser
{
    public static class SimilarElementActionSystem
    {
        public static IEnumerable<Element> Main(this SimilarElementActionRequest self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winInspect.GetComponent<WinPathComponent>();
            var elements = self.AvigationElement();
            ((ElementUIA)elements.FirstOrDefault()).SetForeground();
            self.LightProperty.LightHighMany(elements.ToArray());
            return elements;
        }
    }
}
