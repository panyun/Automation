using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Client
{
    public interface ICreateSystem : ISystemType
    {
        void Run(object self, Registration a);
    }

    [PlayerSystem]
    public abstract class CreateSystem<T> : ICreateSystem
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }
        Type ISystemType.SystemType()
        {
            return typeof(ICreateSystem);
        }

        public void Run(object self, Registration a)
        {
            this.Create((T)self, a);
        }

        public abstract void Create(T self, Registration a);
    }

}
