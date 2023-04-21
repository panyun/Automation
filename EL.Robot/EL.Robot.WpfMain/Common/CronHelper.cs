using EL.Robot.WpfMain.DispatchWindows.Model;
using Quartz;
using Quartz.Impl.Calendar;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.Common
{
    public static class CronHelper
    {
        public static bool IsValidExpression(string cron)
        {
            return CronExpression.IsValidExpression(cron);
        }
        public static List<DateTime> GetNextTime(string cron, int top = 1)
        {
            var list = TriggerUtils.ComputeFireTimes(new CronTriggerImpl("testName", "testGroup", cron), new BaseCalendar(TimeZoneInfo.Local), top);
            if (list != null)
            {
                return list.Select(t => t.LocalDateTime).ToList();
            }
            return null;
        }
        public static string GetCronExpression(this PlanDataModel planData)
        {
            var second = "*";
            var minute = "*";
            var hour = "*";
            var day = "*";
            var week = "?";
            switch (planData.SelectKind)
            {
                case DispatchWindows.ViewModel.SelectKind.Day:
                    second = planData.SelectSecondInfo;
                    minute = planData.SelectMinuteInfo;
                    hour = planData.SelectHoursInfo;
                    break;
                case DispatchWindows.ViewModel.SelectKind.Week:
                    second = planData.SelectSecondInfo;
                    minute = planData.SelectMinuteInfo;
                    hour = planData.SelectHoursInfo;
                    day = "?";
                    var result = planData.WeekInfos.GetInts();
                    if (result != null && result.Count > 0)
                    {
                        week = string.Join(",", result);
                    }
                    break;
                case DispatchWindows.ViewModel.SelectKind.Month:
                    second = planData.SelectSecondInfo;
                    minute = planData.SelectMinuteInfo;
                    hour = planData.SelectHoursInfo;
                    day = planData.SelectDaysInfo;
                    break;
            }
            return $"{second} {minute} {hour} {day} * {week}";
        }
    }
}
