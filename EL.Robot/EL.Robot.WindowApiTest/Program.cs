using EL.Robot.Component;
using EL.Robot.Core;
using EL.Robot.WindowApiTest.Code;

namespace EL.Robot.WindowApiTest
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			//StartComponent s = new StartComponent();
			RequestManager.CreateBoot();
			RequestManager.Init();
			Boot.App.EventSystem.Add(typeof(DesignViewComponent).Assembly);
			Boot.AddComponent<ViewLogComponent>();
			var com = Boot.AddComponent<DesignViewComponent>();
			var com1 = Boot.AddComponent<DesignFlowViewComponent>();
			Application.Run(new Index());
		}
	}
}