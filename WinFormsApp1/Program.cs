using Automation;

namespace WinFormsApp1
{
    internal static class Program
    {
        public static InspectRequestManager_NodeJs RequestInfo = new InspectRequestManager_NodeJs();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            RequestInfo.InitNodeJs(default);
            Application.Run(new Form1());

        }
    }
}