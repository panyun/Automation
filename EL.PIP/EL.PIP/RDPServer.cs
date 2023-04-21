using AxMSTSCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.PIP
{
    public class RDPServer : AxMsRdpClient9NotSafeForScripting
    {
        public bool Control { get; set; }
        public RDPServer() : base()
        {
            Control = true;
        }
        protected override void WndProc(ref Message m)
        {
            if (Control)
            {
                base.WndProc(ref m);
            }
        }
    }
}
