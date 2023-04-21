using EL.Basic;
using EL.Basic.Network.Message;

namespace EL.Net.Network.Message
{
    public interface ISessionStreamDispatcher
    {
        void Dispatch(Session session, MemoryStream stream);
    }
    public class SessionStreamDispatcherAttribute : BaseAttribute
    {
        public int Type;

        public SessionStreamDispatcherAttribute(int type)
        {
            this.Type = type;
        }
    }
    public class SessionStreamDispatcher : Entity
    {
        public static SessionStreamDispatcher Instance;
        public ISessionStreamDispatcher[] Dispatchers;
        public void Load()
        {
            this.Dispatchers = new ISessionStreamDispatcher[100];

            HashSet<Type> types = Boot.App.EventSystem.GetTypes(typeof(SessionStreamDispatcherAttribute));

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(SessionStreamDispatcherAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                SessionStreamDispatcherAttribute sessionStreamDispatcherAttribute = attrs[0] as SessionStreamDispatcherAttribute;
                if (sessionStreamDispatcherAttribute == null)
                {
                    continue;
                }

                if (sessionStreamDispatcherAttribute.Type >= 100)
                {
                    Log.Error("session dispatcher type must < 100");
                    continue;
                }

                ISessionStreamDispatcher iSessionStreamDispatcher = Activator.CreateInstance(type) as ISessionStreamDispatcher;
                if (iSessionStreamDispatcher == null)
                {
                    Log.Error($"sessionDispatcher {type.Name} 需要继承 ISessionDispatcher");
                    continue;
                }

                this.Dispatchers[sessionStreamDispatcherAttribute.Type] = iSessionStreamDispatcher;
            }
        }

        public void Dispatch(int type, Session session, MemoryStream memoryStream)
        {
            ISessionStreamDispatcher sessionStreamDispatcher = this.Dispatchers[type];
            if (sessionStreamDispatcher == null)
            {
                throw new Exception("maybe your NetInnerComponent or NetOuterComponent not set SessionStreamDispatcherType");
            }
            sessionStreamDispatcher.Dispatch(session, memoryStream);
        }
    }
}
