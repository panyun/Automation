using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BaseAttribute : Attribute
    {
        public Type AttributeType { get; }

        public BaseAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObjectSystemAttribute : BaseAttribute
    {
    }
    public interface ISystemType
    {
        Type Type();
        Type SystemType();
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventAttribute : BaseAttribute
    {
    }
    public interface IEvent
    {
        Type GetEventType();
    }

    [Event]
    public abstract class AEvent<A> : IEvent where A : struct
    {
        public Type GetEventType()
        {
            return typeof(A);
        }

        protected abstract ELTask Run(A a);

        public async ELTask Handle(A a)
        {
            try
            {
                await Run(a);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
