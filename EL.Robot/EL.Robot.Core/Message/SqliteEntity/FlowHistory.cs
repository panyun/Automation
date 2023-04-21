using EL.Async;
using EL.Robot.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EL.Robot.Core.SqliteEntity
{
    public class FlowHistory
    {
        public FlowHistory()
        {

        }

        public long Id { get; set; }
        public long FlowId { get; set; }
        public long StartTime { get; set; }
        public DateTime StartDate { get { return TimeHelper.ToDateTime(StartTime); } }
        public long TakeTime { get; set; }
        public DateTime FinishDate { get { return TimeHelper.ToDateTime(StartTime + TakeTime); } }
        public string Name { get; set; }
        // 0 正常 1异常 2手动停止
        public short State { get; set; }
        public string StateStr
        {
            get
            {
                if (State == 0) return "正常";
                if (State == 1) return "异常";
                if (State == 2) return "手动停止";
                return "正常";
            }
        }
        public Brush Color
        {
            get
            {
                if (State == 0) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D9F7BE"));
                if (State == 1) return new SolidColorBrush(Colors.Red);
                if (State == 2) return new SolidColorBrush(Colors.Yellow);
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D9F7BE"));
            }
        }
        public string ExNodeName { get; set; }
        public string ExMsg { get; set; }
        public string LogFilePath
        {
            get; set;
        }
        public string VideoFilePath
        {
            get; set;
        }
        public bool VideoFileExsit { get { return !string.IsNullOrEmpty(VideoFilePath); } }
        public string FileDir
        {
            get
            {
                return GetFileDir();
            }
        }
        private string GetFileDir()
        {
            var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
            return Path.Combine(ConfigItemsHelper.Log_BackupDirectory + "", MsgAgentComonpment.Instance.SelfClientId + "",
                             DateTime.Now.ToString("yyyy-MM-dd") + "",
                             flowComponent.MainFlow.Name + "_" + flowComponent.MainFlow.Id
                             );
        }
        public static FlowHistory CreateFlowHistory()
        {
            var startTime = TimeHelper.ServerNow();
            var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
            FlowHistory flowHistory = new()
            {
                Id = IdGenerater.Instance.GenerateId(),
                FlowId = flowComponent.MainFlow.Id,
                Name = flowComponent.MainFlow.Name,
                StartTime = startTime
            };
            flowHistory.LogFilePath = Path.Combine(flowHistory.FileDir,
                $"{flowHistory.Name + "_" + flowHistory.StartDate.ToString("yyyyMMdd_HHmmssFFF")}.txt");
            if (ConfigItemsHelper.Log_IsScreenRecording)
                flowHistory.VideoFilePath = Path.Combine(flowHistory.FileDir, $"{flowHistory.Name + "_" + flowHistory.StartDate.ToString("yyyyMMdd_HHmmssFFF")}.avi");
            return flowHistory;
        }
    }

}
