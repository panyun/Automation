using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.Model
{
    /// <summary>
    /// 本地信息
    /// </summary>
    public class localinfo
    {
        public static localinfo Instance = new localinfo();
        private static string configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        private static object _lock = new object();
        public long AccountId { get; set; }
        public long clientId { get; set; }
        public int clientType { get; set; } = 1;
        public string nickname { get; set; }
        public string headImgUrl { get; set; }
        public string token { get; set; }
        public static void Set(localinfo localinfo)
        {
            lock (_lock)
            {
                try
                {
                    Instance = localinfo;
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(localinfo));
                }
                catch (Exception)
                {
                }
            }
        }
        public static localinfo Get()
        {
            lock (_lock)
            {
                try
                {
                    var result = File.ReadAllText(configPath);
                    var localinfo = JsonConvert.DeserializeObject<localinfo>(result);
                    Instance = localinfo;
                    return localinfo;
                }
                catch (Exception)
                {
                }
                return null;
            }
        }
        public static bool IsDebug()
        {
            var Debug = ConfigurationManager.AppSettings["Debug"];
            if (bool.TryParse(Debug, out var debug))
            {
                return debug;
            }
            return false;
        }
    }
}
