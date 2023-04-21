using AxMSTSCLib;
using RemoteDesktopManage.Model;
using RemoteDesktopManage.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.AxHost;

namespace RemoteDesktopManage
{

    public class AxMsRdpClient9NotSafeForScriptingEx : AxMsRdpClient9NotSafeForScripting
    {
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x0021) // WM_MOUSEACTIVATE
            {
                if (!this.ContainsFocus)
                {
                    this.Focus();
                }
            }
            base.WndProc(ref m);
        }
    }
    /// <summary>
    /// RDPView.xaml 的交互逻辑
    /// </summary>
    public partial class RDPView : Window
    {
        public AxMsRdpClient9NotSafeForScriptingEx RDPClient { get; private set; }
        public RDPKind RDPKind { get; private set; } = RDPKind.NoConn;
        public RDPConfig RDPConfig;
        public RDPView(RDPConfig rDPConfig)
        {
            InitializeComponent();
            this.SizeChanged += RDPView_SizeChanged;
            this.Unloaded += RDPView_Unloaded;
            this.RDPConfig = rDPConfig;
            RDPClient = new AxMsRdpClient9NotSafeForScriptingEx();
            RdpHost.Child = RDPClient;
            this.Title = this.RDPConfig.Title;
        }

        private void RDPView_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
        public void Dispose()
        {
            this.Start = false;
            try
            {
                Dispatcher.Invoke(() =>
                {
                    RDPClient?.Dispose();
                });
            }
            catch (Exception)
            {
            }
        }

        public bool Start { get; set; }
        public bool Connected { get { return RDPClient?.Connected == 1; } }
        public Action<RDPKind> ConnChange;
        public void ReConn()
        {
            try
            {
                RDPClient.Connect();
                Start = true;
            }
            catch (Exception ex)
            {

            }
        }
        public DateTime NowTime = DateTime.Now;
        public void Conn()
        {
            try
            {
                Init();
                RDPClient.Connect();
                Start = true;
                Task.Factory.StartNew(() =>
                {
                    while (Start)
                    {
                        try
                        {
                            if (RDPKind == RDPKind.Disconnected || RDPKind == RDPKind.Error || RDPKind == RDPKind.LogonError)
                            {
                                if ((DateTime.Now - NowTime).TotalSeconds > 5)
                                {
                                    Ping ping = new Ping();
                                    PingReply pingReply = ping.Send(this.RDPConfig.Server);
                                    if (pingReply.Status == IPStatus.Success)
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            NowTime = DateTime.Now;
                                            this.ReConn();
                                        });
                                    }
                                }
                            }
                            SpinWait.SpinUntil(() => false, 1000);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {

            }
        }
        private void Init()
        {
            //RDPClient.Dock = System.Windows.Forms.DockStyle.Fill;
            //RDPClient.Enabled = true;

            ((System.ComponentModel.ISupportInitialize)(RDPClient)).BeginInit();
            RDPClient.CreateControl();
            RDPClient.Server = this.RDPConfig.Server;
            RDPClient.UserName = this.RDPConfig.UserName;
            //if (this.RDPConfig.RDPPort != 3389)
            //{
            //    RDPClient.AdvancedSettings2.RDPPort = this.RDPConfig.RDPPort;
            //}
            RDPClient.AdvancedSettings7.ClearTextPassword = this.RDPConfig.Password;
            //RDPClient.FullScreenTitle = this.RDPConfig.Title;

            //一定要把EnableCredSspSupport属性置为ture 否则连接上去就是一片空白            
            RDPClient.AdvancedSettings7.EnableCredSspSupport = true;
            //自动尺寸
            RDPClient.AdvancedSettings7.SmartSizing = true;
            //显示连接栏
            RDPClient.AdvancedSettings7.DisplayConnectionBar = false;
            //重新定向
            RDPClient.AdvancedSettings7.RedirectSmartCards = true;


            //// enable CredSSP, will use CredSsp if the client supports.
            //RDPClient.AdvancedSettings7.EnableCredSspSupport = true;
            //RDPClient.AdvancedSettings2.EncryptionEnabled = 1;
            //RDPClient.AdvancedSettings5.AuthenticationLevel = 0;
            //RDPClient.AdvancedSettings5.EnableAutoReconnect = true;
            // setting PublicMode to false allows the saving of credentials, which prevents
            RDPClient.AdvancedSettings6.PublicMode = false;
            RDPClient.AdvancedSettings5.EnableWindowsKey = 1;
            RDPClient.AdvancedSettings5.GrabFocusOnConnect = true;
            RDPClient.AdvancedSettings2.keepAliveInterval = 1000 * 10; // 1000 = 1000 ms
            RDPClient.AdvancedSettings2.overallConnectionTimeout = 600; // The new time, in seconds. The maximum value is 600, which represents 10 minutes.


            //- 0: If server authentication fails, connect to the computer without warning (Connect and don't warn me)
            //- 1: If server authentication fails, don't establish a connection (Don't connect)
            //- 2: If server authentication fails, show a warning and allow me to connect or refuse the connection (Warn me)
            //- 3: No authentication requirement specified.
            RDPClient.AdvancedSettings9.AuthenticationLevel = 0;

            RDPClient.AdvancedSettings6.DisplayConnectionBar = true;
            RDPClient.AdvancedSettings6.ConnectionBarShowPinButton = true;
            RDPClient.AdvancedSettings6.PinConnectionBar = false;
            RDPClient.AdvancedSettings6.ConnectionBarShowMinimizeButton = true;
            RDPClient.AdvancedSettings6.ConnectionBarShowRestoreButton = true;
            RDPClient.AdvancedSettings6.BitmapVirtualCache32BppSize = 48;

            var Size = GetDeskTopSize();
            RDPClient.DesktopWidth = Size.Width;
            RDPClient.DesktopHeight = Size.Height;
            //to enhance user experience, i let the form handled full screen
            RDPClient.AdvancedSettings6.ContainerHandledFullScreen = 1;

            RDPClient.OnLogonError += RDPClient_OnLogonError;
            RDPClient.OnFatalError += RDPClient_OnFatalError;
            //远程桌面登录完成事件
            RDPClient.OnLoginComplete += RDPClient_OnLoginComplete;
            //正在连接远程桌面事件
            RDPClient.OnConnecting += RDPClient_OnConnecting;
            //连接成功后触发事件
            RDPClient.OnConnected += RDPClient_OnConnected;
            RDPClient.OnDisconnected += RDPClient_OnDisconnected;
            ((System.ComponentModel.ISupportInitialize)(RDPClient)).EndInit();
        }

        private void RDPClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            NowTime = DateTime.Now;
            RDPKind = RDPKind.Disconnected;
            ConnChange?.Invoke(RDPKind);
            ReConn();
        }

        private void RDPClient_OnConnected(object sender, EventArgs e)
        {
            RDPKind = RDPKind.Connected;
            ConnChange?.Invoke(RDPKind);
        }

        private void RDPClient_OnConnecting(object sender, EventArgs e)
        {
            RDPKind = RDPKind.Conning;
            ConnChange?.Invoke(RDPKind);
        }

        private void RDPClient_OnLoginComplete(object sender, EventArgs e)
        {
            RDPKind = RDPKind.LoginComplete;
            ConnChange?.Invoke(RDPKind);
        }

        private void RDPClient_OnFatalError(object sender, AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEvent e)
        {
            NowTime = DateTime.Now;
            RDPKind = RDPKind.Error;
            ConnChange?.Invoke(RDPKind);
            ReConn();
        }

        private void RDPClient_OnLogonError(object sender, AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEvent e)
        {
            NowTime = DateTime.Now;
            RDPKind = RDPKind.LogonError;
            ConnChange?.Invoke(RDPKind);
        }
        private void RDPView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Start && Connected)
            {
                var Size = GetDeskTopSize();
                SetRdpResolution((uint)Size.Width, (uint)Size.Height);
            }
        }
        private void SetRdpResolution(uint w, uint h)
        {
            Dispatcher.Invoke(() =>
            {
                RDPClient?.UpdateSessionDisplaySettings(w, h, w, h, 0, GetPrimaryScreenScaleFactor(), 100);
            });
        }
        private static uint GetPrimaryScreenScaleFactor()
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
        private System.Drawing.Rectangle GetDeskTopSize()
        {
            System.Drawing.Rectangle Size = new System.Drawing.Rectangle(0, 0, this.RDPConfig.DesktopWidth, this.RDPConfig.DesktopHeight);
            if (this.RDPConfig.DesktopWidth == 0 || this.RDPConfig.DesktopHeight == 0)
            {
                Size = System.Windows.Forms.Screen.AllScreens[0].Bounds;
            }
            return Size;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
