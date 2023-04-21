using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EL.InstallationService.Common
{
    public static class SystemServerHelper
    {
        public static void Installation(string path, string serviceName = "PIPSystemServer")
        {
            if (!IsInstallation(serviceName))
            {
                RunBatFile(Path.Combine(GetDirectory(path), "install.bat"), false);
            }
        }
        public static void UnInstallation(string path, string serviceName = "PIPSystemServer")
        {
            if (IsInstallation(serviceName))
            {
                RunBatFile(Path.Combine(GetDirectory(path), "uninstall.bat"), false);
            }
        }
        private static string GetDirectory(string path)
        {
            var file = new FileInfo(path);
            if (file.Attributes == FileAttributes.Directory)
            {
                return file.FullName;
            }
            else
            {
                return file.DirectoryName;
            }
        }
        public static bool IsInstallation(string serviceName = "PIPSystemServer")
        {
            var server = ServiceController.GetServices().Where(t => t.ServiceName == serviceName).FirstOrDefault();
            return server != null && server.ServiceName == serviceName;
        }
        public static bool IsRuning(string serviceName = "PIPSystemServer")
        {
            var server = ServiceController.GetServices().Where(t => t.ServiceName == serviceName).FirstOrDefault();
            return server != null && server.ServiceName == serviceName && server.Status == ServiceControllerStatus.Running;
        }
        public static bool Start(string serviceName = "PIPSystemServer")
        {
            var server = ServiceController.GetServices().Where(t => t.ServiceName == serviceName).FirstOrDefault();
            if (server != null && server.ServiceName == serviceName)
            {
                if (server.Status != ServiceControllerStatus.Running)
                {
                    server.Start();
                    SpinWait.SpinUntil(() => IsRuning(), 5 * 1000);
                }
                return true;
            }
            return false;
        }
        public static int GetServerPort(string serviceName = "PIPSystemServer")
        {
            if (IsRuning(serviceName))
            {
                if (int.TryParse(EnvironmentVarialbeHelper.Get(serviceName), out var port))
                {
                    return port;
                }
            }
            return -1;
        }
        /// <summary>
        /// 执行bat(批处理)文件
        /// </summary>
        /// <param name="filePathAndName">指定应用程序的完整路径(比如：D:\GetHostInfo.bat)</param>
        /// <param name="isShowCMDWindow">是否显示执行的命令行窗体(注意：false时是隐式执行，需要在bat文件中添加退出命令exit)</param>
        /// <returns>返回执行结果（true:表示成功）</returns>
        public static bool RunBatFile(string filePathAndName, bool isShowCMDWindow = false)
        {
            bool success = false;
            try
            {
                Process pro = new Process();
                FileInfo file = new FileInfo(filePathAndName);
                pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                pro.StartInfo.FileName = filePathAndName;
                if (isShowCMDWindow)
                {
                    pro.StartInfo.UseShellExecute = true;
                    pro.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    pro.StartInfo.UseShellExecute = false;
                    pro.StartInfo.CreateNoWindow = true;
                }

                pro.Start();
                pro.WaitForExit();
                success = true;

            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
    }
}
