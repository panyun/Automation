using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.InstallationService.Common
{
    public static class InstallServer
    {
        /// <summary>
        /// 提示信息,作为参考
        /// </summary>
        public static void Install()    
        {
            SystemServerHelper.Installation(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EL.PIPSystemServer"));
            RunHelper.SetRun(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EL.Robot.Execute.exe"));
            TaskSchedulerHelper.AddTaskScheduler(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EL.Robot.Execute.exe"));
        }
    }
}
