using EL.Robot.Component;
using EL.Robot.Core;

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
			Application.Run(new IndexForm());
		}
	}
}