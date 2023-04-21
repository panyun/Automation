using NPOI.POIFS.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Common
{
    public static class ProcessHelper
    {

        private static ProcessInfo GetDescendants(int parentProcessId, ProcessInfo processInfo = null)
        {
            // 创建查询字符串，根据父进程ID过滤
            string query = $"Select * From Win32_Process Where ParentProcessId={parentProcessId}";

            // 创建查询对象
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            if (processInfo == null)
            {
                processInfo = new ProcessInfo();
                var process = Process.GetProcessById(parentProcessId);
                processInfo.Id = process.Id;
                processInfo.Name = process.ProcessName;
            }
            var list = searcher.Get();
            if (list?.Count > 0)
            {
                processInfo.ChildProcesses = new List<ProcessInfo>();
                // 遍历查询结果，添加到列表中，并继续递归调用
                foreach (ManagementObject obj in list)
                {
                    // 获取子进程的ID和名称
                    int childProcessId = Convert.ToInt32(obj["ProcessId"]);
                    string childProcessName = obj["Name"].ToString();

                    // 创建一个新的 Process 对象，并设置其 ID 和名称属性
                    ProcessInfo childProcess = new ProcessInfo();
                    childProcess.Id = childProcessId;
                    childProcess.Name = childProcessName;

                    // 递归调用该方法，获取该子进程的所有后代进程
                    processInfo.ChildProcesses.Add(GetDescendants(childProcessId, childProcess));
                }
            }
            return processInfo;
        }
        public static int GetWebProcessID()
        {
            var result = GetDescendants(Process.GetCurrentProcess().Id);
            return GetTargetProcessID(result, "chrome.exe");
        }
        private static int GetTargetProcessID(ProcessInfo ProcessInfo, string procesName)
        {
            if (ProcessInfo.Name == procesName)
            {
                return ProcessInfo.Id; ;
            }
            else
            {
                var child = ProcessInfo.ChildProcesses;
                if (child?.Any() == true)
                {
                    foreach (var process in child)
                    {
                        var id = GetTargetProcessID(process, procesName);
                        if (id > 0)
                        {
                            return id;
                        }
                    }
                }
            }
            return 0;
        }
    }
    public class ProcessInfo
    {
        public int Id;
        public string Name;
        public List<ProcessInfo> ChildProcesses;
        public override string ToString()
        {
            return $"{Name} {Id}";
        }
    }
}
