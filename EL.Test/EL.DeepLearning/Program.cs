using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Overlay;
using System.Configuration;

namespace EL.DeepLearning
{

    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InspectRequestManager.CreateBoot();
            InspectRequestManager.Init();
            Application.Run(new Form1());
        }
    }
}