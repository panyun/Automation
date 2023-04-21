using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Overlay;
using EL.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Cmd
{
    public static class InspectManager
    {
        public static bool IsExist = true;
        public static void Start(string[] args)
        {
            CatchElementRequest request1 = new CatchElementRequest();
            request1.RpcId = 1;
            var requestS = JsonHelper.ToJson(request1);

            Boot.App = new AppMananger();
            Boot.SetLog(new FileLogger());
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            RequestType request = RequestType.CatchElement;
            var param = "";
            if (args == null || args.Length == 0)
                param = "10000{\"$type\":\"Automation.Inspect.CatchElementRequest, EL.Automation\",\"RpcId\":1}";
            else
                param = args[0];
            request = (RequestType)int.Parse(param.Substring(0, 5));
            //加载程序集
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            Boot.App.EventSystem.Add(typeof(FormOverLayComponent).Assembly);
            var inspect = Boot.AddComponent<InspectComponent>();

            switch (request)
            {
                case RequestType.CatchElement:
                    CatchElement(param);
                    break;
                case RequestType.MouseActionRequest:
                    MouseActionRequest(param);
                    break;
                case RequestType.InputActionRequest:
                    InputActionRequest(param);
                    break;
                default:
                    CatchElement(param);
                    break;
            }
            inspect.Action = (x, y) =>
            {
                var json = JsonHelper.ToJson(y);
                CatchElementResponse response = new CatchElementResponse();
                response.Content = json;
                var responseStr = JsonHelper.ToJson(response);
                inspect.GetComponent<ClipboardComponent>().CopyToClipboard(responseStr);
                inspect.ExitApp();
            };
        }
        public static void CatchElement(string param)
        {
            var request = JsonHelper.FromJson<CatchElementRequest>(param.Substring(5));
            var inspect = Boot.GetComponent<InspectComponent>();
            inspect.MouseHookInit();
            inspect.CatchElement();
        }
        public static void MouseActionRequest(string param)
        {
            var request = JsonHelper.FromJson<CatchElementRequest>(param.Substring(5));
            var inspect = Boot.GetComponent<InspectComponent>();
            var parser = Boot.GetComponent<ParserComponent>();
        }
        public static void InputActionRequest(string param)
        {
            var request = JsonHelper.FromJson<CatchElementRequest>(param.Substring(5));
            var inspect = Boot.GetComponent<InspectComponent>();
            var parser = Boot.GetComponent<ParserComponent>();
        }
    }
}
