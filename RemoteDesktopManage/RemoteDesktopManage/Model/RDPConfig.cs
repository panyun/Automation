using AxMSTSCLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDesktopManage.Model
{
    public class RDPConfig
    {
        public RDPConfig(string Server, string UserName, string Password, int RDPPort = 3389, int DesktopWidth = 0, int DesktopHeight = 0, string title = "")
        {
            this.Server = Server;
            this.UserName = UserName;
            this.Password = Password;
            this.RDPPort = RDPPort;
            this.Title = string.IsNullOrEmpty(title) ? Server : title;
            this.DesktopHeight = DesktopHeight;
            this.DesktopWidth = DesktopWidth;
        }
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RDPPort { get; set; }
        public string Title { get; set; }
        public int DesktopWidth { get; set; }
        public int DesktopHeight { get; set; }
    }
}
