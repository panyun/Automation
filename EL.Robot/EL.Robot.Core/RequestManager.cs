using EL.Async;
using EL.Robot.Component;
using System.IO.Pipes;
using System.Security.Principal;
using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Overlay;
using System.Diagnostics;
using EL.Robot.Component.DTO;

namespace EL.Robot.Core
{
    public static class RequestManager
    {
        public static NamedPipeClientStream PipeClient;
        public static NodeComponent NodeComponentRoot
        {
            get
            {
                return Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().GetComponent<NodeComponent>();
            }
        }
        static RequestManager()
        {
            PipeClient = new NamedPipeClientStream("localhost", "inspectPipe", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
        }

        public static void CreateBoot()
        {
            Boot.CreateBoot();
        }
        public static void Init()
        {
            Log.Trace($"————Main Start！——--");
            ConfigSystem.LazyLoad();
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            //RequestManager.Init();
            RequestOptionComponent.IsWindow = true;
            Boot.App.EventSystem.Add(typeof(RobotComponent).Assembly);
            Boot.App.EventSystem.Add(typeof(InspectComponent).Assembly);
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            Boot.App.EventSystem.Add(typeof(FormOverLayComponent).Assembly);
            var inspect = Boot.AddComponent<InspectComponent>();
            var parser = Boot.AddComponent<ParserComponent>();
            var robot = Boot.AddComponent<RobotComponent>();
            robot.AddComponent<DesignComponent>();
            NodeComponentRoot.AddComponent<CatchElementComponent>();
            //加载程序集
            GetComponentInfo(Boot.App.Scene, "Boot->Scene");
        }
        private static void GetComponentInfo(Entity entity, string path)
        {
            Log.Trace($"加载Componet对象-----配置{path}");
            foreach (var item in entity.Components)
                GetComponentInfo(item.Value, path + "->" + item.Key.Name);
        }
        public static async ELTask<ComponentResponse> StartAsync(string json)
        {
            return await StartAsync(BsonHelper.FromJson<CommponetRequest>(json));
        }
        public static async ELTask<ComponentResponse> StartAsync(CommponetRequest request)
        {
            var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
            if (request.ComponentName != null && request.ComponentName == nameof(CatchElementComponent))
                return await NodeComponentRoot.GetComponent<CatchElementComponent>().Main(request);
            if (request.ComponentName != null && request.ComponentName.ToLower() == nameof(ExecFlow).ToLower())
                return await ExecFlow(request);
            if (request.ComponentName != null && request.ComponentName.ToLower() == nameof(DesignComponentSystem.CreateRobot).ToLower())
                return designComponent.CreateRobot(request);
            if (request is SelectVariableRequest selectVariableRequest)
                return designComponent.SelectVariable(selectVariableRequest);
            if (request.ComponentName != null && request.ComponentName.ToLower() == nameof(ComponentSystem).ToLower())
                return ComponentSystem.Main(request);
            if (request.ComponentName != null && request.ComponentName.ToLower() == nameof(DesignComponentSystem.PreviewNodes).ToLower())
                return await designComponent.PreviewNodes(request);
            return await request.Main();
        }

        public static async ELTask<ComponentResponse> ExecFlow(this CommponetRequest request)
        {
            var flow = (request.Data as Flow);
            var robot = Boot.GetComponent<RobotComponent>();
            await robot.LocalMain(flow, false);
            var logs = robot.GetComponent<FlowComponent>().LogMsgs;
            return new ComponentResponse()
            {
                Data = logs
            };
        }
        public static async ELTask<ComponentResponse> Main(this CommponetRequest request)
        {
            var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
            try
            {
                if (request.FlowId != default)
                {
                    designComponent.DesignFlowDic.TryGetValue(request.FlowId, out var flow);
                }
                var content = NodeContent.Create(designComponent.CurrentDesignFlow, request.Data);
                var flowComponent = NodeComponentRoot.GetParent<FlowComponent>();
                flowComponent.CurrentFlow = designComponent.CurrentDesignFlow;
                var component = NodeComponentRoot.FindComponent((string)request.Data.ComponentName);
                var rtn = await component.Main(content);
                return new ComponentResponse()
                {
                    Error = rtn.Value ? 0 : 600,
                    Data = rtn.Out
                };
            }
            catch (Exception ex)
            {
                return new ComponentResponse()
                {
                    Error = 700,
                    Message = ex.Message,
                };
            }
        }

    }
    public static class ComponentSystem
    {
        public class ComponentSystemReuqest
        {
            public string Action { get; set; }
            public Dictionary<string, object> Paramters { get; set; }
        }
        public static ComponentResponse Main(CommponetRequest request)
        {
            if (request.Data.Action.ToLower() == nameof(GetCategorys).ToLower())
                return GetCategorys();
            if (request.Data.Action.ToLower() == nameof(GetComponentsById).ToLower())
                return GetComponentsById(request);
            if (request.Data.Action.ToLower() == nameof(GetExpression).ToLower())
                return GetExpression(request);
            return new ComponentResponse();
        }
        public static ComponentResponse GetExpression(this CommponetRequest request)
        {
            var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
            var componentName = request.Data.Paramters["ComponentName"];
            RequestManager.NodeComponentRoot.ChildComponent.TryGetValue(componentName, out Entity entity);
            return new ComponentResponse()
            {
                Data =
                (entity as BaseComponent).GetExpression(request, designComponent.CurrentDesignFlow.ParamsManager)
            };
        }


        public static ComponentResponse GetComponentsById(this CommponetRequest request)
        {
            var id = request.Data.Paramters["Id"];
            if (id == default(int)) return new ComponentResponse();
            var components = RequestManager.NodeComponentRoot.Components;
            var objs = components.Values.Where(x => x is BaseComponent && (int)((BaseComponent)x).Config.Category == id && ((BaseComponent)x).Config.IsView).
               Select(x => ((BaseComponent)x).GetConfig()).ToList();
            if (objs.Any())
            {
                return new ComponentResponse()
                {
                    Data = objs
                };
            }
            return new ComponentResponse();
        }
        public static ComponentResponse GetCategorys()
        {
            return new ComponentResponse()
            {
                Data = CategoryHelper.Categorys
            };
        }
    }
}
