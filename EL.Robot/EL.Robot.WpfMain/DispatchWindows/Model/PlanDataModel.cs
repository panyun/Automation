using EL.Robot.WpfMain.DispatchWindows.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.DispatchWindows.Model
{
    public class PlanDataModel
    {
        public SelectKind SelectKind { get; set; }
        public string SelectDaysInfo { get; set; }
        public string SelectHoursInfo { get; set; }
        public string SelectMinuteInfo { get; set; }
        public string SelectSecondInfo { get; set; }
        public WeekInfos WeekInfos { get; set; }
    }
}
