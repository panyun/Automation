using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
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
            RequestType request = RequestType.CatchUIRequest;
            var param = "";
            if (args != null && args.Length > 0)
            {
                param = args[0];
                request = (RequestType)int.Parse(param.Substring(0, 4));
            }
            //加载程序集
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            Boot.App.EventSystem.Add(typeof(FormOverLayComponent).Assembly);
          
            var inspect = Boot.AddComponent<InspectComponent>();
    
            var parser = Boot.GetComponent<ParserComponent>();
            switch (request)
            {
                case RequestType.CatchUIRequest:
                    CatchElement(param);
                    break;
                default:
                    CatchElement(param);
                    break;
            }
            //inspect.KeyboardHookInit();
            inspect.Action = (x, y) =>
            {

            };
        }
        public static void CatchElement(string param)
        {
            // var request = JsonHelper.FromJson<CatchElementRequest>(param);
            //var inspect = Boot.GetComponent<InspectComponent>();
            //inspect.MouseHookInit();
            //inspect.CatchElement();
        }
    }
}
