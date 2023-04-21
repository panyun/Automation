using EL.Async;
namespace EL
{
    public static class Boot
    {
        public static AppAbstract App;
        static Boot()
        {

        }

        public static T GetComponent<T>() where T : Entity => App.GetComponent<T>();
        public static Entity GetComponent(Type type)  => App.GetComponent(type);
        public static dynamic GetComponent(string name) => App.GetComponent(name);
        public static T AddComponent<T>(T t) where T : Entity => App.AddComponent(t);  
        public static Entity AddComponent(Type type, bool isFromPool = false) => App.AddComponent(type);
        public static T AddComponent<T>() where T : Entity, new() => App.AddComponent<T>();
        public static TimeInfo TimeInfo => TimeInfo.Instance;
        public static TimerComponent TimerComponent;
        public static IdGenerater IdGenerater => IdGenerater.Instance;
        public static ILogger Logger = new ConsoleLogger();
        public static void Update() => App.Update();
        public static void SetLog(ILogger logger)
        {
            Logger = logger;
            //logger.SetLevel(level);
            Log.Default = logger;
        }
        public static void CreateBoot()
        {
            App = new AppMananger();
            var log = new FileLogger();
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            _ = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1);
                        Update();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message, e);
                    }
                }
            });
            SetLog(log);
            log.SetLevel(LogLevel.Trace);
            Log.Trace("加载Boot对象-----");
            Log.Trace($"加载Log对象-----配置{JsonHelper.ToJson(Logger)}");
        }
    }
    public class AppMananger : AppAbstract
    {
        protected override void Init()
        {
            base.Init();
        }
    }
    public static class EntitySceneFactory
    {
        public static Scene CreateScene(long id, long instanceId, int zone, SceneType sceneType, string name, Entity parent = null)
        {
            Scene scene = new Scene(id, instanceId, zone, sceneType, name, parent);

            return scene;
        }

        public static Scene CreateScene(long instanceId, int zone, SceneType sceneType, string name, Entity parent = null)
        {
            Scene scene = new Scene(instanceId, zone, sceneType, name, parent);
            return scene;
        }

        public static Scene CreateScene(int zone, SceneType sceneType, string name, Entity parent = null)
        {
            long instanceId = (long)IdGenerater.Instance.GenerateInstanceId();
            Scene scene = new Scene(instanceId, zone, sceneType, name, parent);
            return scene;
        }
    }
    public interface IApp
    {
        T GetComponent<T>() where T : Entity;
        T AddComponent<T>(T t) where T : Entity;
        void Update();
    }

    public abstract class AppAbstract : IApp
    {

        private Scene scene;
        public Scene Scene
        {
            get
            {
                if (scene != null)
                {
                    return scene;
                }
                scene = EntitySceneFactory.CreateScene((long)IdGenerater.Instance.GenerateInstanceId(), 0, SceneType.Process, "Process");
                return scene;
            }
        }
        public AppAbstract()
        {
            Init();
        }
        protected virtual void Init()
        {
            Scene.AddComponent<TimerComponent>();
            EventSystem.Add(typeof(AppAbstract).Assembly); //添加当前应用程序集
        }
        public T GetComponent<T>() where T : Entity
        {
            return Scene.GetComponent<T>();
        }
        public dynamic GetComponent(string name)
        {
            return Scene.GetComponent(name);
        }
        public Entity GetComponent(Type type)
        {
            return Scene.GetComponent(type);
        }
        public T AddComponent<T>(T t) where T : Entity
        {
            return (T)Scene.AddComponent(component: t);
        }
        
        public T AddComponent<T>() where T : Entity, new()
        {
            return Scene.AddComponent<T>(false);
        }
        public Entity AddComponent(Type type, bool isFromPool = false)
        {
            return Scene.AddComponent(type, isFromPool);
        }
        public ThreadSynchronizationContext ThreadSynchronizationContext => ThreadSynchronizationContext.Instance;
        public EventSystem EventSystem => EventSystem.Instance;
        public virtual void Update()
        {
            ThreadSynchronizationContext.Update();
            TimeInfo.Instance.Update();
            if (GetComponent<TimerComponent>() != null)
                GetComponent<TimerComponent>().Update();
            EventSystem.Update();
        }
        public void Close()
        {
            scene?.Dispose();
            scene = null;
            ObjectPool.Instance.Dispose();
            EventSystem.Instance.Dispose();
            IdGenerater.Instance.Dispose();
        }

    }
}
