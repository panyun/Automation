using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.PIP
{
    public static class Log
    {
        private static object Lock = new object();
        private static string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pipinfo.log");
        public static void Write(string data)
        {
            lock (Lock)
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} {data}");
                }
            }
        }
        public static void Clear()
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
