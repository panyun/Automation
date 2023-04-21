using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Overlay;
using EL.WindowsAPI;

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
            //PythonTest.RunPythonTest();
            InspectRequestManager.CreateBoot();
            InspectRequestManager.Init();
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            string param = string.Empty;
            if (args != null && args.Length > 0)
            {
                param = args[0];
            }
            if(param.ToLower() == "pipe")
            {
                InspectRequestManager.ReadResquest();
                var inspectForm = Boot.GetComponent<InspectComponent>();
                var fromMsg = inspectForm.GetComponent<FormOverLayComponent>();
                Application.Run(fromMsg.Form);
                return;
            }
            //param = "";
            if (param.ToLower() == "window")
            {
                //java界面控件
                //var javaInspect = inspect.AddComponent<JavaFormInspectComponent>();
                RequestOptionComponent.IsWindow = true;
                Application.Run(new Inspect_form());
                return;
            }
            if (param.ToLower() == "msg")
            {
                RequestOptionComponent.IsWindow = true;
                Application.Run(new InspectChatStart());
                Log.Trace($"————Main End！运行时间：{RequestOptionComponent.RequestTime.ElapsedMilliseconds}——--");
                return;
            }
            var inspect = Boot.GetComponent<InspectComponent>();
            if (string.IsNullOrEmpty(param))
            {
                param = "10000{\"$type\":\"Automation.Inspect.CatchElementRequest, EL.Automation\",\"RpcId\":1}";
            }
            RequestOptionComponent.IsWindow = false;
            RequestOptionComponent.Args = param;
            var val = InspectRequestManager.Start(param).GetResult();
            var from = inspect.GetComponent<FormOverLayComponent>();
       
            Application.Run(from.Form);
            RequestOptionComponent.RequestTime.Stop();
            Log.Trace($"————Main End！运行时间：{RequestOptionComponent.RequestTime.ElapsedMilliseconds}——--");

        }
    }
}