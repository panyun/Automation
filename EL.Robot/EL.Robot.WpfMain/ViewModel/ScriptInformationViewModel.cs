using EL.Robot.Core.SqliteEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.ViewModel
{
    public class ScriptInformationViewModel
    {

        public ScriptInformationViewModel()
        {
            Count = RobotDataManagerService.GetFlowHistorysLength();
            FlowHistories= RobotDataManagerService.GetFlowHistorys(0, Count);
            if (FlowHistories == default) FlowHistories =  new List<FlowHistory>();
        }

        public List<FlowHistory> FlowHistories { get; set; }
        public int Count { get; set; }




    }
}
