using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.PIPSystemServer.Common
{
    public static class Log
    {
        private static object Lock = new object();
        public static void Write(string data)
        {
            lock (Lock)
            {
                using (StreamWriter sw = new StreamWriter($"log_{DateTime.Now.ToString("yyyyMMdd")}.txt", true))
                {
                    sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} {data}");
                }
            }
        }
    }
}
