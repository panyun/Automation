using Cassia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.InstallationService
{
    public static class PIPHelper
    {
        public static bool IsPIP()
        {
            ITerminalServicesManager manager = new TerminalServicesManager();
            var newSession = manager.GetLocalServer().GetSessions().Where(t => t.UserAccount == manager.CurrentSession.UserAccount).ToList();
            return newSession.Count > 1;
        }
        public static void StartPIP(string path)
        {
            Process.Start(path);
        }
    }
}
