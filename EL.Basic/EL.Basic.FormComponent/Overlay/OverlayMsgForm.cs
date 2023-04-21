#if NETFRAMEWORK || NETCOREAPP

using EL.WindowsAPI;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace EL.Overlay
{
    public class OverlayMsgForm : Form
    {


        public Label label1;
        int SH = Screen.PrimaryScreen.Bounds.Height; //1080
        int SW = Screen.PrimaryScreen.Bounds.Width; //1920
        int lableH = 149;
        int lableW = 270;
        int lableFont = 15;

        public OverlayMsgForm()
        {
            this.MouseClick += OverlayMsgForm_MouseClick;
            this.KeyDown += OverlayMsgForm_KeyDown;
            GetPhScreen();
            Height = Screen.PrimaryScreen.Bounds.Height;
            Width = Screen.PrimaryScreen.Bounds.Width;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            Left = 0;
            Top = 0;
            Visible = false;
            TransparencyKey = Color.Gray;
            BackColor = Color.Gray;
            Opacity = 0.75;
            Cursor = Cursors.Arrow;

            label1 = new Label();
            label1.BackColor = Color.FromArgb(100, Color.Black);
            label1.Height = 50;
            label1.Width = 270;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.ForeColor = Color.White;
            label1.Text = "";
            label1.Font = new Font("黑体", lableFont, FontStyle.Regular);
            label1.Left = (SW / 2) - 130;
            label1.Top = SH / 2;
            label1.Visible = true;
            this.Controls.Add(label1);
        }

        private void OverlayMsgForm_KeyDown(object? sender, KeyEventArgs e)
        {
            
        }

        private void OverlayMsgForm_MouseClick(object? sender, MouseEventArgs e)
        {

        }

        void GetPhScreen()
        {
            try
            {
                var mc = new ManagementClass("Win32_DesktopMonitor");
                double height = 0;
                double width = 0;
                foreach (var a in mc.GetInstances())
                {
                    var pnpId = a.Properties["PNPDeviceID"];
                    string path = pnpId.Value?.ToString();
                    if (path != null)
                    {
                        var bytes = (byte[])Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\" + path + @"\Device Parameters").GetValue("EDID");
                        height = bytes[22];
                        width = bytes[21];
                    }
                }

                lableH = (int)(149 * SH / 1080 * (height / 29));
                lableW = (int)(270 * SW / 1920 * (width / 52));
				if (lableH == default)
				{
					lableH = 180;
					lableW = 300;
				}
				lableFont = (int)(15 * (height / 29));
				if (lableFont == 0)
				{
					lableFont = 15;
				}
			}
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void SetWindow(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                
            });
        }


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            var hotkey = FormOverLayComponent.Instance.GetComponent<HotkeyComponent>();
            hotkey.ProcessHotKey(m);
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var result = base.CreateParams;
                result.ExStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
                result.ExStyle |= (int)WindowStyles.WS_EX_TRANSPARENT;
                result.ExStyle |= (int)WindowStyles.WS_EX_NOACTIVATE;
                result.ExStyle |= (int)WindowStyles.WS_EX_LAYERED;
                result.ExStyle |= (int)WindowStyles.WS_EX_TOPMOST;
                return result;
            }
        }
        
    }
}
#endif