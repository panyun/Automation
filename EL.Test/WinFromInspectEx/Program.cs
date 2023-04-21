using Automation.Cmd;
using Automation.Inspect;
using EL;
using EL.Async;
using EL.Overlay;

namespace WinFromInspect
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Log.Info("————开始进入程序Main！——--");
            Boot.App = new AppMananger();
            Boot.SetLog(new FileLogger());
            //加载程序集
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            Boot.App.EventSystem.Add(typeof(FormOverLayComponent).Assembly);
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            Log.Info("————开始进入程序End！——--");
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
            Application.Run(new Inspect_form());
        }
    }
}