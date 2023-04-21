using EL.InstallationService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.InstallationService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TaskSchedulerHelper.AddTaskScheduler(@"E:\backenddev3\c-automation\EL.PIP\EL.Agent\bin\Debug\EL.Agent.exe");
            //TaskSchedulerHelper.RemoveTaskScheduler();
            //SystemServerHelper.Installation(@"E:\backenddev3\c-automation\EL.PIP\EL.PIPSystemServer\bin\Debug");
            //SystemServerHelper.UnInstallation(@"E:\backenddev3\c-automation\EL.PIP\EL.PIPSystemServer\bin\Debug");
            Console.WriteLine(SystemServerHelper.GetServerPort());
            Console.ReadLine();
        }
    }
}
