using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.Component
{
    public class DateComponent : BaseComponent
    {
		public DateComponent()
		{
			Config.Category = Category.基础函数;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.DisplayName = "日期";
			return base.GetConfig();
		}
		public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var function = self.CurrentNode.GetParamterString("function").ToLower();
            string param = function;
            if (function == "now")
            {
                var type = self.CurrentNode.GetParamterInt("type");
                param = $"{function}_{type}";
            }
            switch (param)
            {
                case "now_1": // 当前时间转字符串
                    var format = self.CurrentNode.GetParamterString("format");
                    if (format == "wekDay")
                        self.Out = (int)DateTime.Now.DayOfWeek;
                    else if (format == "monthDay")
                        self.Out = DateTime.Now.Day;
                    else if (format == "monthLastDay")
                        self.Out = DateTime.Now.Date.AddMonths(1).AddDays(-DateTime.Now.Day).Day;
                    else if (format == "yearDay")
                        self.Out = DateTime.Now.DayOfYear;
                    else if (format == "quarter")
                    {
                        var str = DateTime.Now.AddMonths(0 - ((DateTime.Now.Month - 1) % 3)).ToString("yyyy-MM-01");
                        var start = DateTime.Parse(str);
                        self.Out = (DateTime.Now - start).Days;
                    }

                    else
                        self.Out = DateTime.Now.ToString(format);
                    break;
                case "now_2": //当前时间转为时间戳
                    self.Out = TimeHelper.ServerNow();
                    break;
                case "increaseordecrease":
                    var targettime = self.CurrentNode.GetParamterDateTime("targettime");
                    var way = self.CurrentNode.GetParamterInt("way");
                    var unit = self.CurrentNode.GetParamterString("unit").ToLower();
                    var length = self.CurrentNode.GetParamterInt("length");
                    length = way == 1 ? Math.Abs(length) : -Math.Abs(length);
                    if (unit == "year")
                        self.Out = targettime.AddYears(length);
                    else if (unit == "month")
                        self.Out = targettime.AddMonths(length);
                    else if (unit == "day")
                        self.Out = targettime.AddDays(length);
                    else if (unit == "hour")
                        self.Out = targettime.AddHours(length);
                    else if (unit == "minute")
                        self.Out = targettime.AddMinutes(length);
                    else if (unit == "second")
                        self.Out = targettime.AddSeconds(length);
                    break;
                case "interval":
                    var startDate = self.CurrentNode.GetParamterDateTime("starttime");
                    var endDate = self.CurrentNode.GetParamterDateTime("endtime");
                    var interval = (endDate - startDate);
                    unit = self.CurrentNode.GetParamterString("unit");
                    if (unit == "year")
                        self.Out = endDate.Year - startDate.Year;
                    else if (unit == "month")
                        self.Out = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);
                    else if (unit == "day")
                        self.Out = interval.TotalDays;
                    else if (unit == "hour")
                        self.Out = interval.TotalHours;
                    else if (unit == "minute")
                        self.Out = interval.TotalMinutes;
                    else if (unit == "second")
                        self.Out = interval.TotalSeconds;
                    break;
                case "formater":
                    format = self.CurrentNode.GetParamterString("format");
                    targettime = self.CurrentNode.GetParamterDateTime("targettime");
                    if (format == "timestamp")
                    {
                        self.Out = Boot.TimeInfo.Transition(targettime);
                        break;
                    }
                    self.Out = targettime.ToString(format);
                    break;
            }
            self.Value = true;
            return self;
        }

    }
}
