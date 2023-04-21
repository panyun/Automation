using AxMSTSCLib;
using Cassia;
using MSTSCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EL.PIP
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 打开子会话功能
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSEnableChildSessions(bool enable);
        /// <summary>
        /// 判断用户是否打开了子会话功能
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSIsChildSessionsEnabled(out bool isEnabled);
        /// <summary>
        /// 获取子会话的SessionID
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSGetChildSessionId(out uint SessionId);
        /// <summary>
        /// 注销子会话
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        [DllImport("Wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSLogoffSession(IntPtr hServer, uint SessionId, bool bWait);
        private RDPServer rdp;

        public MainWindow()
        {
            InitializeComponent();
            rdp = new RDPServer();
            formhost.Child = rdp;
            this.SizeChanged += MainWindow_SizeChanged;
            this.Closing += MainWindow_Closing;
        }
        /// <summary>
        /// 屏蔽无效的操作
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SYSCOMMAND = 0x112;
            int SC_MOVE = 0xF012;
            int SC_MAXIMIZE = 0xF030;
            int SC_MINIMIZE = 0xF020;
            int SC_RESTORE = 0xF120;
            int SC_SIZE = 0xF000;
            int SC_CLOSE = 0xF060;
            var check = ControlName.IsChecked == true ? true : false;
            if (!check)
            {
                if (msg == 0x00A3 || msg == 0x0003)
                {
                    handled = true;
                    wParam = IntPtr.Zero;
                }
                else if (msg == WM_SYSCOMMAND)
                {
                    if (!((wParam == (IntPtr)0xF012) || (wParam == (IntPtr)SC_MINIMIZE) || (wParam == (IntPtr)SC_RESTORE) || (wParam == (IntPtr)SC_CLOSE)))
                    {
                        handled = true;
                        wParam = IntPtr.Zero;
                    }
                }
            }
            return IntPtr.Zero;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(WndProc));
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var rtn = System.Windows.Forms.MessageBox.Show($"您确定要关闭画中画吗？", "关闭", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
            if (rtn == System.Windows.Forms.DialogResult.OK)
            {
                if (WTSGetChildSessionId(out var sessionid))
                {
                    ITerminalServicesManager manager = new TerminalServicesManager();
                    var session = manager.GetLocalServer().GetSessions()?.Where(t => t.SessionId == sessionid).FirstOrDefault();
                    if (session != null)
                    {
                        session.Logoff(true);
                    }
                }
            }
            else
            {
                e.Cancel = true;
                return;
            }
        }

        public bool Connected { get { return rdp.Connected == 1; } }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                Log.Write($"Window_Loaded: {ex.Message}");
            }
        }
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Connected)
            {
                try
                {
                    SetRdpResolution((uint)shell.ActualWidth, (uint)shell.ActualHeight);
                }
                catch (Exception ex)
                {
                    Log.Write($"Window_Loaded: {ex.Message}");
                }
            }
        }
        private void SetRdpResolution(uint w, uint h)
        {
            rdp?.UpdateSessionDisplaySettings(w, h, w, h, 0, GetPrimaryScreenScaleFactor(), 100);
        }
        public static uint GetPrimaryScreenScaleFactor()
        {
            uint sf = 100;
            try
            {
                // !must use PrimaryScreen scale factor
                sf = (uint)(100 * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth);
            }
            catch (Exception)
            {
                sf = 100;
            }
            finally
            {
                if (sf < 100)
                    sf = 100;
            }
            return sf;
        }
        public void Connect()
        {
            // 打开画中画功能
            if (!WTSEnableChildSessions(true))
            {
                Log.Write($"Connect: 启用画中画功能失败");
                throw new InvalidOperationException("启用画中画功能失败");
            }
            bool enabled = false;
            WTSIsChildSessionsEnabled(out enabled);
            if (!enabled)
            {
                Log.Write($"Connect: 启用画中画功能失败2");
                throw new InvalidOperationException("启用画中画功能失败2");
            }
            //连接本机,不会把自己挤掉线
            rdp.Server = "127.0.0.1";
            var settings = (rdp.GetOcx() as IMsRdpExtendedSettings);
            object otrue = true;
            //设置连接自己，必须要加这一行,加了这一行后，会自动定位到LocalHost
            settings.set_Property("ConnectToChildSession", ref otrue);

            //设置账号密码，我这里因为不知道用户的账号密码，暂时屏蔽掉
            //var name = ConfigurationManager.AppSettings["name"];
            //var password = ConfigurationManager.AppSettings["password"];
            //rdp.UserName = name;
            //rdp.AdvancedSettings7.ClearTextPassword = password;

            //启用CredSSupSupport
            //一定要把EnableCredSspSupport属性置为ture 否则连接上去就是一片空白            
            //rdp.Connected
            rdp.AdvancedSettings7.EnableCredSspSupport = true;
            //自动尺寸
            rdp.AdvancedSettings2.SmartSizing = true;
            //显示连接栏
            rdp.AdvancedSettings7.DisplayConnectionBar = false;
            ////重新定向
            //rdp.AdvancedSettings7.RedirectSmartCards = true;
            //远程桌面登录完成事件
            rdp.OnLoginComplete += (_, __) =>
            {
                //如有需要，编写代码        
            };
            //远程桌面警告事件
            rdp.OnWarning += (_, e) =>
            {
                //如有需要，编写代码        
            };
            //正在连接远程桌面事件
            rdp.OnConnecting += (_, e) =>
            {
                //如有需要，编写代码        
            };
            //连接成功后触发事件
            rdp.OnConnected += (_, e) =>
            {
                //如有需要，编写代码        
            };
            //断开连接回调事件
            rdp.OnDisconnected += OnDisconnected;
            //登录远程桌面事件
            rdp.OnLogonError += delegate (object _, IMsTscAxEvents_OnLogonErrorEvent e)
            {
                if (e.lError != -2)
                {
                    Log.Write($"Connect: OnLogonError");
                    //如有需要               
                    this.Close();
                }
            };
            //远程桌面发生致命错误事件
            rdp.OnFatalError += delegate (object _, IMsTscAxEvents_OnFatalErrorEvent e)
            {
                Log.Write($"Connect: OnFatalError");
                //如有需要
                this.Close();
            };
            //连接
            rdp.Connect();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (rdp == null)
            {
                return;
            }
            //减去事件
            rdp.OnDisconnected -= OnDisconnected;
            //查看连接数，断开连接
            if (rdp.Connected != 0)
            {
                rdp.Disconnect();
            }
        }
        private void OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Log.Write($"Connect: OnDisconnected");
            this.Close();
        }


        private void ControlName_Click(object sender, RoutedEventArgs e)
        {
            var check = ControlName.IsChecked == true ? true : false;
            rdp.Control = check;
        }

        private void TopMostName_Click(object sender, RoutedEventArgs e)
        {
            var check = TopMostName.IsChecked == true ? true : false;
            Topmost = check;
        }
    }
}
