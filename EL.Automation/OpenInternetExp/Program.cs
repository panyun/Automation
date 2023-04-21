using EL.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenInternetExp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                InternetExplorer IE = null;
                IE = new InternetExplorer();
                var app = IE.Application;
                IE.Visible = true;
                object nil = new object();
                IE.Navigate("www.bing.com", ref nil, ref nil, ref nil, ref nil);
                User32.ShowWindow(IE.HWND, 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
