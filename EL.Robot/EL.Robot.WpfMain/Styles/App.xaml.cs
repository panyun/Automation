using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Overlay;
using EL.Sqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfInspect.ViewModels;

namespace WpfInspect
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Boot.App = new AppMananger();
            var log = new FileLogger();
            //var str = ConfigurationManager.AppSettings["IsLog"].ToString();
            //bool.TryParse(str, out var isLog);
            log.SetLevel(LogLevel.Trace);
            bool isLog = true;
            if (!isLog)
                log.SetLevel(LogLevel.Error);
            Boot.SetLog(log);
            RequestOptionComponent.RequestTime.Start();
            Log.Trace($"————Main Start！——--");
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            //加载程序集
            //Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            //Boot.App.EventSystem.Add(typeof(FormOverLayComponent).Assembly);
            //var inspect = Boot.AddComponent<InspectComponent>();
            //var parser = Boot.AddComponent<ParserComponent>();
            string param = "window";
            if (e.Args.Count() > 0)
                param = e.Args[0];
            Task.Run(() =>
            {
                while (true)
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
