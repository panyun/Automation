using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Inspect.Helper
{
    public static class InspectHelper
    {
        public static void OpenIE()
        {
            var process= Process.Start(@"C:\Program Files (x86)\Internet Explorer\IEXPLORE.EXE");
            process.Start();
        }
    }
}
