using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [ConsoleHandler(ConsoleMode.ReloadDll)]
    public class ReloadDllConsoleHandler : IConsoleHandler
    {
        #region hotfix
        //public async ETTask Run(ModeContex contex, string content)
        //{
        //    byte[] dllBytes = File.ReadAllBytes("./Example.Server.Hotfix.dll");
        //    byte[] pdbBytes = File.ReadAllBytes("./Example.Server.Hotfix.pdb");
        //    Assembly assembly = Assembly.Load(dllBytes, pdbBytes);
        //    Game.EventSystem.Add(assembly);
        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    Log.Info($"[CONSOLE]: reload {assembly.GetName().Name}.dll finish!");
        //    await ETTask.CompletedTask;
        //}
        #endregion

        public async ETTask Run(ModeContex contex, string content)
        {
            switch (content)
            {
                case ConsoleMode.ReloadDll:
                    contex.Parent.RemoveComponent<ModeContex>();
                    var list = GetHotfixAssembly();
                    list.ForEach(assembly =>
                    {
                        Game.EventSystem.Add(assembly);
                        Log.Info($"[CONSOLE]: reload {assembly.GetName().Name}.dll finish!");
                    });
                    //Game.EventSystem.Load();
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                   
                    break;
            }
            await ETTask.CompletedTask;
        }
        public List<Assembly> GetHotfixAssembly()
        {
            var assemblyList = new List<Assembly>();
            byte[] dllBytes = File.ReadAllBytes("./Server.Hotfix.dll");
            byte[] pdbBytes = File.ReadAllBytes("./Server.Hotfix.pdb");
            assemblyList.Add(Assembly.Load(dllBytes, pdbBytes));
            dllBytes = File.ReadAllBytes("./Example.Server.Hotfix.dll");
            pdbBytes = File.ReadAllBytes("./Example.Server.Hotfix.pdb");
            assemblyList.Add(Assembly.Load(dllBytes, pdbBytes));
            return assemblyList;
        }
    }
}
