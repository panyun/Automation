using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.PIPSystemServer
{
    /// <summary>
    /// 端口帮助 
    /// </summary>
    public static class PortHelper
    {
        private static object Lock = new object();
        private static string RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "port.txt");
        public static void Set(int port)
        {
            lock (Lock)
            {
                File.WriteAllText(RootPath, port.ToString());
            }
        }
        public static int Read()
        {
            lock (Lock)
            {
                if (File.Exists(RootPath))
                {
                    var txt = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "port.txt"));
                    if (int.TryParse(txt, out var port))
                    {
                        return port;
                    }
                }
            }
            return -1;
        }
    }
}
