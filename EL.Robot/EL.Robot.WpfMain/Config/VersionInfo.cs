using EL.Robot.WpfMain.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.Config
{
    public class VersionInfo
    {
        public bool IsCurrentVersion { get; set; }
        public string Version { get; set; }
        public string Time { get; set; }
        public List<string> Updates { get; set; }

        public static List<VersionInfo> GetVersionInfos()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "VersionInfo.json");
            var versions = new List<VersionInfo>();
            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);
                try
                {
                    if (file != null)
                    {
                        versions = file.ToObj<List<VersionInfo>>();
                        versions.First().IsCurrentVersion = true;
                    }
                }
                catch (Exception)
                {
                }
            }
            return versions;
        }
    }
}
