using Automation.Com;
using EL;
using EL.WindowsAPI;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;

namespace Inspect.Test
{

    public class ComTest
    {
        [Test]
        public void Test()
        {
            InspectCaptureServer InspectServer = new InspectCaptureServer();
            InspectCaptureExServer InspectServer1 = new InspectCaptureExServer();
            for (int i = 0; i < 50000; i++)
            {
                
                var val = InspectServer.GetCaptureInfo();
                var val1 = InspectServer.GetCaptureInfo(val.TitleName, val.ClassName);
                var va2 = InspectServer1.GetCaptureInfo();
                var val3 = InspectServer1.GetCaptureInfo(va2.TitleName, va2.ClassName);
                Thread.Sleep(100);
                Trace.Write(i);
            }
           
        }
        [Test]
        public void Select()
        {
            ElementServer elementServer = new ElementServer();
            elementServer.SelectIndex(1);
        }
        [Test]
        public void LogTest()
        {
            InspectCaptureServer InspectServer = new InspectCaptureServer();
            Log.Error("msg");
            Log.Info("msg");
            Log.Debug("msg");
        }
        [Test]
        public void OpenInternetExplorer()
        {
            SHDocVw.InternetExplorer IE = null;
            IE = new SHDocVw.InternetExplorer();
            var app = IE.Application;
            IE.Visible = true;
            object nil = new object();
            IE.Navigate("www.bing.com", ref nil, ref nil, ref nil, ref nil);
            var isTrue = true;
            User32.ShowWindow(IE.HWND, 3);
            Process process = new Process();
            process.StartInfo.FileName = @"C:\\Program Files\\Internet Explorer\\iexplore.exe";//IEFilePath();   //IE浏览器，可以更换
            process.StartInfo.Arguments = "www.bing.com";
            process.Start();
            Interop.UIAutomationClient.IUIAutomationElement IEEL = null;
        }
    }

}
