using Automation;
using EL;
using EL.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInspect
{
    public static class RequestWpfManager
    {
        public  static void BootMain()
        {
            InspectRequestManager.CreateBoot();
            InspectRequestManager.Init();
            var elSqlComponent = Boot.AddComponent<ElSqliteComponent>();
            new MainWindow().Show();
        }
    }
}
