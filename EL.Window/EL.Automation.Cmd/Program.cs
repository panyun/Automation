// See https://aka.ms/new-console-template for more information

using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Overlay;
using Microsoft.Win32;
using System.Management;

namespace Automation.Cmd
{
    internal class Program
    {
        static  void Main(string[] args)
        {
            //var mc = new ManagementClass("Win32_DesktopMonitor");
            //double height = 0;
            //double width = 0;
            //foreach (var a in mc.GetInstances())
            //{
            //    string path;
            //    Console.WriteLine(path = a.Properties["PNPDeviceID"].Value.ToString());
            //    var bytes = (byte[])Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\" + path + @"\Device Parameters").GetValue("EDID");
            //    height = bytes[22];
            //    width = bytes[21];
            //    Console.WriteLine("宽" + bytes[21].ToString());
            //    Console.WriteLine("高" + bytes[22].ToString());
            //}

            //Console.ReadKey();

            Boot.App = new AppMananger();
            Boot.SetLog(new FileLogger());
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            InspectManager.Start(args);
            Task.Run(() => {
                while (InspectManager.IsExist)
                {
                    try
                    {
                        Thread.Sleep(1);
                        Boot.Update();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message, e);
                    }
                }
            });
           
        }
    }

}