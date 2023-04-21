using EL;
using EL.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.UIAutomationClient;

namespace Automation.Inspect
{
    public class IEInspectComponent : Entity
    {
        public int WM_HTML_GETOBJECT => User32.RegisterWindowMessage("WM_HTML_GETOBJECT");
        public Guid IID_IHTMLDocumentGuid = new Guid("626fc520-a41e-11cf-a731-00a0c9082637");
        public static IEInspectComponent Instance;
        public static object NotSupportedValue => new CUIAutomation().ReservedNotSupportedValue;
    }
}
