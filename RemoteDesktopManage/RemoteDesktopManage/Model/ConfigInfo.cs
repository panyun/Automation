using RemoteDesktopManage.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace RemoteDesktopManage.Model
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class ConfigInfo
    {
        static string Root = AppDomain.CurrentDomain.BaseDirectory;
        static ConfigInfo()
        {
            if (!Directory.Exists(Root))
            {
                Directory.CreateDirectory(Root);
            }
        }
        public List<RDPConfig> RDPConfigs { get; set; }

        private static object Lock = new object();
        public static List<RDPConfig> GetInfos()
        {
            lock (Lock)
            {
                var path = Path.Combine(Root, "ConfigInfo.json");
                var versions = new List<RDPConfig>();
                if (File.Exists(path))
                {
                    var file = File.ReadAllText(path);
                    try
                    {
                        if (file != null)
                        {
                            versions = file.ToObj<List<RDPConfig>>();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                return versions;
            }
        }
        public static void SaveInfos(List<RDPConfig> ConfigInfos)
        {
            lock (Lock)
            {
                try
                {
                    var path = Path.Combine(Root, "ConfigInfo.json");
                    var json = ConfigInfos.ToJson();
                    File.WriteAllText(path, json);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
