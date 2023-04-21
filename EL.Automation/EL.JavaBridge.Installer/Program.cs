using System.Reflection;

namespace EL.JavaBridge.Installer
{
    internal static class Program
    {
        // Token: 0x06000061 RID: 97 RVA: 0x000037F8 File Offset: 0x000019F8
        [STAThread]
        private static void Main()
        {
            Log.Info("in install");
            AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
            if (InstallHelper.HasInstallerProcess())
            {
                Log.Info("插入已完成");
                return;
            }
            InstallHelper.KillJuschedProcesses();
            string bridgeLibFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BridgeLib");
            InstallHelper.CopyToSystemRoot(bridgeLibFolder);
            string[] javaHomeFolders = InstallHelper.GetJreFolders();
         
            if (javaHomeFolders.Length == 0)
            {
                throw new JavaBridgeInstallException("jre没发现");
            }
            foreach (string jrefolder in javaHomeFolders)
            {
                Log.Info(jrefolder);
                InstallHelper.CopyToJre(bridgeLibFolder, jrefolder);
            }
            Log.Info("安装java环境已完成");
            Console.ReadLine();
        }


        // Token: 0x06000063 RID: 99 RVA: 0x00003964 File Offset: 0x00001B64
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(((Exception)e.ExceptionObject).StackTrace);
        }
    }
}
