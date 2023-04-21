using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.InstallationService.Common
{
    public static class RunHelper
    {
        public static void SetRun(string path)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    key.SetValue("PIPServer", path);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
