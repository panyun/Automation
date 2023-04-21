using EL.Robot.Core.SqliteEntity;
using EL.Robot.WpfMain.Command;
using EL.Robot.WpfMain.Utils;
using Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EL.Robot.WpfMain.DispatchWindows.ViewModel
{
    /// <summary>
    /// 新计划模板
    /// </summary>
    public class NewPlanViewModel
    {
        public NewPlanViewModel()
        {
            var result = RobotDataManagerService.GetPlanDataFlowId();
            if (result != null)
            {
                JobCount = result.Count;
            }
        }
        public long JobCount { get; set; } = 0;

        private MvvmCommand _CreateCommand;
        public ICommand CreateCommand => _CreateCommand ?? new MvvmCommand(Create);
        public void Create()
        {
            WindowManager.Show<Plan>(new PlanViewModel());
        }
    }
}
