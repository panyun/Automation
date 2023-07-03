using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EL.BaseUI.Themes
{
    public static class CommonHelper
    {
        /// <summary>
        /// 根据字段自动生成属性
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static string CreatePropertyByField(string fields)
        {
            string result = "";
            string[] strs = fields.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            foreach (var item in strs)
            {
                if (item.StartsWith("//"))
                {
                    string str = item;
                    result += str + "\r\n";
                }
                else if (item.StartsWith("private"))
                {
                    string[] itemStrs = item.Split(new string[] { " ", ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    string propertyName = "";
                    if (itemStrs[2][0] == '_')
                    {
                        propertyName = itemStrs[2][1].ToString().ToUpper() + itemStrs[2].Substring(2);
                    }
                    else
                    {
                        propertyName = itemStrs[2][0].ToString().ToUpper() + itemStrs[2].Substring(1);
                    }
                    string str = @$"public {itemStrs[1]} {propertyName}
        {{
            get
            {{
                return {itemStrs[2]};
            }}

            set
            {{
                {itemStrs[2]} = value;
                OnPropertyChanged(nameof({propertyName}));
            }}
            }}
            ";
                    result += str + "\r\n";
                }

            }
            return result;
        }

        public static T DeepCopy<T>(this T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static Guid GetNewGuid()
        {
            return Guid.NewGuid();
        }

        public static string Guid32To36(string guid32)
        {
            string result = "";
            result = $"{guid32.Substring(0, 8)}-{guid32.Substring(8, 4)}-{guid32.Substring(12, 4)}-{guid32.Substring(16, 4)}-{guid32.Substring(20, 12)}";
            return result;
        }

        public static string Guid36To32(string guid36)
        {
            string result = "";
            result = guid36.Replace("-", "");
            return result;
        }

        public static string GetAppDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.Trim('\\');
        }

        public static object? GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj);
        }

        public static string BirthdayToAge(DateTime birthday)
        {
            string age = "";
            if (!birthday.Date.Equals(DateTime.Parse("0001-01-01 0:0:0")))
            {
                TimeSpan ts = DateTime.Now.Date - birthday.Date;
                // 当前月的天数
                var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                // 相差月份数
                var months = ((DateTime.Now.Year - birthday.Year) * 12) + DateTime.Now.Month - birthday.Month;
                // 年龄少于一个月，按照"天"数显示
                if ((ts.TotalDays + 1) < daysInMonth)
                {
                    age = (ts.TotalDays + 1) + "天";
                }
                // 年龄大于一个月，少于一周岁，记录到"月"
                else if (months >= 1 && months < 12)
                {
                    age = months + "个月";
                }
                // 年龄大于一周岁，少于三周岁，记录"年"、"月"
                else if (months >= 12 && months < 36)
                {
                    int year = months / 12;
                    age = $"{year}岁{months - year * 12}个月";
                }
                // 年龄大于三周岁，记录"年"
                else if (months >= 36)
                {
                    age = $"{months / 12}岁";
                }
            }
            return age;
        }

        public async static Task<string> GetConfigValue(string key)
        {
            var config = new ConfigurationBuilder()
                 .SetBasePath(GetAppDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
            return await Task.FromResult(config.GetSection(key).Value);
        }

        public static List<string> GetConfigValues(string key)
        {
            
            var config = new ConfigurationBuilder()
                 .SetBasePath(GetAppDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
            return config.AsEnumerable().Where(x => x.Key.Contains(key)).Select(x => x.Key + ":" + x.Value).ToList();
        }

        public static void KillCurrentProcess()
        {
            var process = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(process.ProcessName);
            if (processes != null)
            {
                foreach (var item in processes)
                {
                    if (item.Id == process.Id) continue;
                    item.Kill();
                }
            }
        }

        public static string GetIPFromUrl(string url)
        {
            var strs = url.Split(new string[] { "/", ":" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strs.Length; i++)
            {
                if (IPAddress.TryParse(strs[i], out IPAddress? ip))
                {
                    return strs[i];
                }
            }
            return "";
        }

        public static int GetPortFromUrl(string url)
        {
            var strs = url.Split(new string[] { "/", ":" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strs.Length; i++)
            {
                if (IPAddress.TryParse(strs[i], out IPAddress? ip))
                {
                    if (i < strs.Length - 1)
                    {
                        return Convert.ToInt32(strs[i + 1]);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }


    }
}
