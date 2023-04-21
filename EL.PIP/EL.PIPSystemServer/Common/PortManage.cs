using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EL.PIPSystemServer.Common
{
    /// <summary>
    /// 获取端口
    /// </summary>
    public static class PortManage
    {
        private static string PortReleaseGuid = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 获取一个没有被占用的端口
        /// </summary>
        public static int GetPort(int port, int newPort)
        {
            var listport = new List<int>() { port };
            if (newPort > 0)
            {
                listport.Add(newPort);
            }

            return FindNextAvailableTCPPort(listport);
        }
        /// <summary> 
        /// Check if startPort is available, incrementing and 
        /// checking again if it's in use until a free port is found 
        /// </summary> 
        /// <param name="startPort">The first port to check</param> 
        private static int FindNextAvailableTCPPort(List<int> ports, int startPort = 1000)
        {
            var mutex = new Mutex(false, string.Concat("Global/", PortReleaseGuid));
            mutex.WaitOne();
            try
            {
                IPEndPoint[] endPoints = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
                var endPorts = endPoints.Select(t => t.Port).Distinct().OrderBy(t => t).ToList();
                foreach (var item in ports)
                {
                    if (!endPorts.Contains(item))
                    {
                        return item;
                    }
                }

                for (int i = startPort; i < IPEndPoint.MaxPort; i++)
                {
                    if (!endPorts.Contains(i))
                    {
                        return i;
                    }
                }
                throw new ApplicationException("Not able to find a free TCP port.");
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
