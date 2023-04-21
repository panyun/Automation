using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Overlay;
using NLog;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;

namespace Automation
{
    public class RequestOptionComponent : Entity
    {
        public static Stopwatch RequestTime = new Stopwatch();
        public static string Args { get; set; }
        public Process Process { get; set; }
        public static bool IsWindow { get; set; }
        public static bool IsCloseApp { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public static bool IsJVM { get; set; } = false;
        public static ElementType ProgramType { get; set; }
        public static object Obj { get; set; } = new object();
        public static int Index { get; set; } = 0;
        public static RequestOptionComponent Instance { get; set; }
        public long ElementPathId { get; set; }
        public RequestType RequestType { get; set; }
        public IRequest Request { get; set; }
        public int TimeOut { get; set; }
        public static RequestOptionComponent CreateRequestLog(dynamic request, int timeOut, RequestType requestType)
        {
            var questLog = new RequestOptionComponent();
            questLog.Request = request;
            questLog.TimeOut = timeOut;
            questLog.RequestType = requestType;
            Instance = questLog;
            return questLog;
        }
    }
    public class InspectRequestManager_NodeJs  
    {
        public async Task<object> InitNodeJs(string param)
        {
            InspectRequestManager.CreateBoot();
            InspectRequestManager.Init();
            return "Init Suceess";
        }
        public async Task<object> StartNodeJs(string args)
        {
            var response = await InspectRequestManager.StartAsync(args);
            return JsonHelper.ToJson(response);
        }
        public async Task<object> StartNodeJs_Async1(string args)
        {
            return await InspectRequestManager.StartAsync(args);
        }
        public async Task<object> StartNodeJs_Async2(IRequest obj)
        {
            return await InspectRequestManager.StartAsync(obj);
        }
    }
    public static class InspectRequestManager
    {

        public static NamedPipeServerStream PipeServer { get; set; }

        static InspectRequestManager()
        {
            PipeServer = new NamedPipeServerStream("inspectPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            ThreadPool.QueueUserWorkItem(delegate
            {
                PipeServer.BeginWaitForConnection((o) =>
                {
                    NamedPipeServerStream pServer = (NamedPipeServerStream)o.AsyncState;
                    pServer.EndWaitForConnection(o);
                    StreamReader sr = new StreamReader(pServer);
                }, PipeServer);

            });
        }

        public static void SendRespose(string text)
        {
            try
            {
                var sw = new StreamWriter(PipeServer)
                {
                    AutoFlush = true
                };
                sw.Write(text);
            }
            catch (Exception)
            {
            }
        }
        public static async Task ReadResquest()
        {
            Boot.GetComponent<TimerComponent>().NewRepeatedTimer(100, async () =>
            {
                if (!PipeServer.IsConnected) return;
                var count = PipeServer.ReadByte();
                if (count == 0) return;
                byte[] buffer = new byte[count];
                await PipeServer.ReadAsync(buffer, 0, count);
                var resquest = Encoding.UTF8.GetString(buffer);
                var respose = await Start(resquest);
                SendRespose(respose);
            });
            //Thread.Sleep(1000);
            //try
            //{
            //    var pipeClient = new NamedPipeClientStream("localhost", "inspectPipe", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
            //    pipeClient.Connect();
            //    CatchElementRequest catchElementRequest = new();
            //    var resquest = BsonHelper.ToJson(catchElementRequest);
            //    var bytes = Encoding.UTF8.GetBytes(resquest);
            //    await pipeClient.WriteAsync(bytes, 0, bytes.Length);
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

        }
        public static ELTask<IResponse> FinishAction
        { get; set; }

        #region init
        public static void CreateBoot()
        {
            Boot.CreateBoot();
        }
        public static void Init()
        {
            Log.Trace($"————Main Start！——--");
            ConfigSystem.LazyLoad();
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            //加载程序集
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            Boot.App.EventSystem.Add(typeof(FormOverLayComponent).Assembly);
            var inspect = Boot.AddComponent<InspectComponent>();
            var parser = Boot.AddComponent<ParserComponent>();
            RequestOptionComponent.IsWindow = true;
            GetComponentInfo(Boot.App.Scene, "Boot->Scene");

        }
        public static void GetComponentInfo(Entity entity, string path)
        {
            Log.Trace($"加载Componet对象-----配置{path}");
            foreach (var item in entity.Components)
                GetComponentInfo(item.Value, path + "->" + item.Key.Name);
        }
        #endregion

        #region Start Main
        /// <summary>
        /// 协议入口
        /// </summary>
        /// <param name="args"></param>

        public static async ELTask<string> Start(string args)
        {
            return await StartToJsonAsync(args);
        }
        /// <summary>
        /// 协议异步回调
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>

        public static async ELTask<IResponse> StartAsync(string args)
        {
            Log.Trace(args);
            var inspect = Boot.GetComponent<InspectComponent>();
            try
            {
                var request = GetRequest(args);
                return await StartAsync(request);
            }
            catch (Exception ex)
            {
                var res = new ResponseBase().GetFail(ex);
                inspect.ExitApp(res);
                return res;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IRequest GetRequest(string args)
        {
            RequestType request = (RequestType)int.Parse(args.Substring(0, 5));
            var json = args.Substring(5);
            Type type = default;
            switch (request)
            {
                case RequestType.CatchUIRequest:
                    type = typeof(CatchUIRequest);
                    break;
                case RequestType.MouseActionRequest:
                    type = typeof(MouseActionRequest);
                    break;
                case RequestType.InputActionRequest:
                    type = typeof(InputActionRequest);
                    break;
                case RequestType.ElementActionRequest:
                    type = typeof(ElementActionRequest);
                    break;
                case RequestType.ElementVerificationActionRequest:
                    type = typeof(ElementVerificationActionRequest);
                    break;
                case RequestType.StartMsgActionRequest:
                    type = typeof(StartMsgActionRequest);
                    break;
                case RequestType.UpdateElement:
                    break;
                case RequestType.GenerateSimilarElementActionRequest:
                    type = typeof(GenerateSimilarElementActionRequest);
                    break;
                case RequestType.ElementPropertyActionRequest:
                    type = typeof(ElementPropertyActionRequest);
                    break;
                case RequestType.SimilarElementActionRequest:
                    type = typeof(SimilarElementActionRequest);
                    break;
                case RequestType.GenerateCosineSimilarActionRequest:
                    type = typeof(GenerateCosineSimilarActionRequest);
                    break;
                case RequestType.CosineSimilarElementActionRequest:
                    type = typeof(CosineSimilarElementActionRequest);
                    break;
                case RequestType.OpenBrowserActionRequest:
                    type = typeof(OpenBrowserActionRequest);
                    break;
                case RequestType.ChildsElementActionRequest:
                    type = typeof(ChildsElementActionRequest);
                    break;
                case RequestType.ParentElementActionRequest:
                    type = typeof(ParentElementActionRequest);
                    break;
                default:
                    break;
            }

            return (IRequest)JsonHelper.FromJson(type, json);


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<string> StartToJsonAsync(string args)
        {
            var rtn = await StartAsync(args);
            return JsonHelper.ToJson(rtn);
        }

        /// <summary>
        /// SDK异步回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<IResponse> StartAsync(IRequest request)
        {
            FinishAction = ELTask<IResponse>.Create();
            var inspect = Boot.GetComponent<InspectComponent>();
            var parser = Boot.GetComponent<ParserComponent>();
            _ = Enum.TryParse(request.GetType().Name, out RequestType requestType);
            var requestLog = RequestOptionComponent.CreateRequestLog(request, 0, RequestType.CatchUIRequest);
            inspect.RemoveComponent<RequestOptionComponent>();
            inspect.AddComponent(requestLog);
            parser.RemoveComponent<RequestOptionComponent>();
            parser.AddComponent(requestLog);
            RequestOptionComponent.RequestTime.Start();
            //更新配置文件
            switch (requestType)
            {
                case RequestType.CatchUIRequest:
                    inspect.CatchElement((CatchUIRequest)request);
                    break;
                case RequestType.MouseActionRequest:
                    await parser.MouseAction((MouseActionRequest)request);
                    break;
                case RequestType.InputActionRequest:
                    await parser.InputAction((InputActionRequest)request);
                    break;
                case RequestType.ElementActionRequest:
                    parser.ElementAction((ElementActionRequest)request);
                    break;
                case RequestType.ElementPropertyActionRequest:
                    parser.ElementPropertyAction((ElementPropertyActionRequest)request);
                    break;
                case RequestType.ElementVerificationActionRequest:
                    parser.ElementVerificationAction((ElementVerificationActionRequest)request);
                    break;
                case RequestType.StartMsgActionRequest:
                    parser.StartMsgAction((StartMsgActionRequest)request);
                    break;
                case RequestType.GenerateSimilarElementActionRequest:
                    parser.GenerateSimilarElementAction((GenerateSimilarElementActionRequest)request);
                    break;
                case RequestType.SimilarElementActionRequest:
                    parser.SimilarElementAction((SimilarElementActionRequest)request);
                    break;
                case RequestType.GenerateCosineSimilarActionRequest:
                    parser.GenerateCosineSimilarAction((GenerateCosineSimilarActionRequest)request);
                    break;
                case RequestType.CosineSimilarElementActionRequest:
                    parser.CosineSimilarElementAction((CosineSimilarElementActionRequest)request);
                    break;
                case RequestType.GenerateTableActionRequest:
                    parser.GenerateTableAction((GenerateTableActionRequest)request);
                    break;
                case RequestType.GenerateExcelDataActionRequest:
                    parser.GenerateExcelDataAction((GenerateExcelDataActionRequest)request);
                    break;
                case RequestType.GenerateHtmlActionRequest:
                    await parser.GenerateHtmlAction((GenerateHtmlActionRequest)request);
                    break;
                case RequestType.HighlightActionRequest:
                    await parser.HighlightAction((HighlightActionRequest)request);
                    break;
                case RequestType.OpenBrowserActionRequest:
                    await parser.OpenBrowserAction((OpenBrowserActionRequest)request);
                    break;
                case RequestType.ChildsElementActionRequest:
                    await parser.ChildsElementAction((ChildsElementActionRequest)request);
                    break;
                case RequestType.ParentElementActionRequest:
                    await parser.ParentElementAction((ParentElementActionRequest)request);
                    break;
                default:
                    inspect.CatchElement((CatchUIRequest)request);
                    break;
            }
            var rtnValue = await FinishAction;
            rtnValue.RpcId = request.RpcId;
            inspect.ExitApp(rtnValue);
            return rtnValue;
        }
        #endregion
    }

    #region Edge.js 测试方法
    //public class Message
    //{
    //    static Queue<string> _queue = new Queue<string>();

    //    public async Task<object> Init(string parameter)
    //    {
    //        if (_queue != default(Queue<string>))
    //        {
    //            for (int i = 0; i < 10; i++)
    //            {
    //                _queue.Enqueue(i.ToString());
    //            }
    //            return $".Net success :" + parameter;
    //        }
    //        else
    //        {
    //            return ".Net fail :" + parameter;
    //        }
    //    }

    //    public async Task<object> Get(string parameter)
    //    {
    //        if (_queue.Count() > 0)
    //        {
    //            return _queue.Dequeue() + ":Net:" + parameter;
    //        }
    //        else
    //        {
    //            return ".Net:" + parameter;
    //        }
    //    }
    //}
    #endregion

}