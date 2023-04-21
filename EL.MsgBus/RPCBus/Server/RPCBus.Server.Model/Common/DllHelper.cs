using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server
{
    public static class DllHelper
    {
        public static Assembly[] ListAssemblyInterests()
        {
            List<Assembly> list = new List<Assembly>();

            // 必须先载入 ET 框架的 Hotfix 层
            if (true)
            {
                byte[] dllBytes = File.ReadAllBytes("./Server.Hotfix.dll");
                byte[] pdbBytes = File.ReadAllBytes("./Server.Hotfix.pdb");

                list.Add(Assembly.Load(dllBytes, pdbBytes));
            }

            // 当前程序集， 即 <Example.Server.Model.dll>
            list.Add(Assembly.GetExecutingAssembly());

            if (true)
            {
                byte[] dllBytes = File.ReadAllBytes("./RPCBus.Server.Hotfix.dll");
                byte[] pdbBytes = File.ReadAllBytes("./RPCBus.Server.Hotfix.pdb");

                list.Add(Assembly.Load(dllBytes, pdbBytes));
            }

            // TODO: 添加你需要载入的程序集 DLL

            return list.ToArray();
        }
    }
}
