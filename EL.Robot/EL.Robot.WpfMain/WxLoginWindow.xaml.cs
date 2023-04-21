using CefSharp.DevTools.Target;
using EL;
using EL.Async;
using EL.InstallationService;
using EL.Robot;
using EL.Robot.Core;
using EL.Robot.Core.Request;
using EL.Robot.Core.SqliteEntity;
using EL.Robot.WpfMain.Common;
using EL.Robot.WpfMain.DispatchWindows.Model;
using EL.Robot.WpfMain.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Protos;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Cache;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Robot
{
    /// <summary>
    /// WxLoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WxLoginWindow : Window
    {

        private const string getQrCodeUrl = @"https://gateway.rpaii.com/api-auth/mp/getQrCode";
        private const string isSubscribeUrl = @"https://gateway.rpaii.com/api-auth/mp/isSubscribe";
        private const string setRobotName = @"https://gateway.rpaii.com/api-auth/userReboot/setRobotName";
        private const string WebUrl = @"https://designer.rpaii.com/";
        public bool IsShow = true;
        public WxLoginWindow()
        {
            InitializeComponent();
            this.Loaded += WxLoginWindow_Loaded;
            this.Closed += WxLoginWindow_Closed;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void WxLoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadQr();
        }

        private void WxLoginWindow_Closed(object sender, EventArgs e)
        {
            IsShow = false;
        }
        /// <summary>
        /// 退出
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private static WxQrCode GetWxQrCode()
        {
            return HttpJsonPost<WxQrCode>(getQrCodeUrl);
        }
        private static WxSubScribe IsSubScribe(string token, string macAddr)
        {
            return HttpJsonPost<WxSubScribe>(isSubscribeUrl, new Dictionary<string, object>() { { "token", token }, { "macAddr", macAddr } });
        }
        /// <summary>
        /// robotType 0 代表PC机器人 1代表移动机器人
        /// </summary>
        private static WxSetRobortName SetRobotName(long robortId, string robortName, string token)
        {
            return HttpJsonPost<WxSetRobortName>(setRobotName, new Dictionary<string, object>() { { "robotId", robortId }, { "robotName", robortName }, { "robotType", 0 } }, new Dictionary<string, object>() { { "authorization", token } });
        }
        private static T HttpJsonPost<T>(string url, Dictionary<string, object> args = null, Dictionary<string, object> header = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 20000;
            request.Method = "post";
            request.ContentType = "application/json";
            request.Proxy = null;
            if (header?.Any() == true)
            {
                foreach (var item in header)
                {
                    request.Headers.Add(item.Key, item.Value.ToString());
                }
            }
            if (args?.Any() == true)
            {
                var body = JsonConvert.SerializeObject(args);
                byte[] data = Encoding.UTF8.GetBytes(body);
                using var stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string postContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return JsonConvert.DeserializeObject<T>(postContent);
            }
            catch { }
            return default;
        }
        private void LoadQr()
        {
            _ = Task.Run(() =>
            {
                while (IsShow)
                {
                    var macAddr = GetNetMacAddress();
                    var result = GetWxQrCode();
                    if (result?.IsSuccess == true)
                    {
                        _ = Dispatcher.BeginInvoke(new Action(delegate
                        {
                            qrImg.Source = new BitmapImage(new Uri(result.data.url));
                        }));
                        _ = Task.Run(() =>
                        {
                            while (IsShow)
                            {
                                var issubscribe = IsSubScribe(result.data.token, macAddr);
                                if (issubscribe.IsSuccess)
                                {
                                    if (!localinfo.IsDebug())
                                    {
                                        string url = $"{WebUrl}login?token={issubscribe.data.token}";
                                        Process.Start(url);
                                    }
                                    localinfo.Set(new localinfo() { AccountId = issubscribe.data.userId, clientId = issubscribe.data.robotId, clientType = 1, nickname = issubscribe.data.nickname, headImgUrl = issubscribe.data.headImgUrl, token = issubscribe.data.token });

                                    Login(localinfo.Instance);
                                    return;
                                }
                                Thread.Sleep(1000);
                            }
                        });
                        break;
                    }
                    Thread.Sleep(5 * 1000);
                    System.Windows.Forms.MessageBox.Show($"网络不稳定，获取不到二维码，请检查网络！", "错误");
                }
            });
        }
        public static void Logout()
        {
            var rtn = System.Windows.Forms.MessageBox.Show($"您确定要注销吗？", "注销", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
            if (rtn == System.Windows.Forms.DialogResult.OK)
            {

            }
            else
            {
                return;
            }
            localinfo.Instance = null;
            WindowManager.Show<WxLoginWindow>();
            void SetValue(ContextMenu contextMenu)
            {
                if (contextMenu == null)
                {
                    return;
                }
                foreach (MenuItem item in contextMenu.Items)
                {
                    if (item.Header.ToString().StartsWith("注销"))
                    {
                        item.Header = "登录";
                        item.Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/yc.png"))
                        };
                    }
                    else if (item.Header.ToString() == "设置...")
                    {
                        item.Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/yc.png"))
                        };
                    }
                    else if (item.Header.ToString() == "打开画中画")
                    {
                        item.Icon = new Image()
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/yc.png"))
                        };
                    }
                }
            }
            var bigRobot = WindowManager.GetWindow<BigRobot>();
            var taskMenu = bigRobot.FindResource("taskMenu") as ContextMenu;
            var taskMenu1 = bigRobot.FindResource("taskMenu1") as ContextMenu;
            SetValue(taskMenu);
            SetValue(taskMenu1);
            WindowManager.Close<BlueWindow>();
            WindowManager.Hide<BigRobot>();
        }
        public static void Login(localinfo localinfo)
        {
            if (localinfo != null && localinfo.AccountId > 0)
            {
                _ = Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    WindowManager.Show<BigRobot>();
                    void SetValue(ContextMenu contextMenu, string name)
                    {
                        if (contextMenu == null)
                        {
                            return;
                        }
                        var newName = name;
                        var length = 5;
                        if (name.Length > 5)
                        {
                            newName = name.Substring(0, name.Length - 5);
                            newName += "...";
                        }
                        foreach (MenuItem item in contextMenu.Items)
                        {
                            if (item.Header.ToString() == "登录")
                            {
                                item.Header = $"注销 “{newName}”";
                                item.Icon = new Image()
                                {
                                    //Source = new BitmapImage(new Uri(localinfo.headImgUrl))
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/zc.png"))
                                };
                            }
                            else if (item.Header.ToString() == "设置...")
                            {
                                item.Icon = new Image()
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/zc.png"))
                                };
                            }
                            else if (item.Header.ToString() == "打开画中画")
                            {
                                item.Icon = new Image()
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/zc.png"))
                                };
                            }
                        }
                    }
                    var bigRobot = WindowManager.GetWindow<BigRobot>();
                    var taskMenu = bigRobot.FindResource("taskMenu") as ContextMenu;
                    var taskMenu1 = bigRobot.FindResource("taskMenu1") as ContextMenu;
                    SetValue(taskMenu, localinfo.nickname);
                    SetValue(taskMenu1, localinfo.nickname);
                    WindowManager.Close<WxLoginWindow>();
                    WindowManager.Show<BlueWindow>();
                }));
                _ = Task.Run(() =>
                {
                    string robortName = Environment.MachineName;
                    var result = SetRobotName(localinfo.clientId, robortName, localinfo.token);
                });
                _ = Task.Factory.StartNew(async () =>
                {
                    await MsgBusLogin(localinfo.AccountId, localinfo.clientId);
                }, TaskCreationOptions.LongRunning);
                _ = Task.Run(() =>
                {
                    DispatchTaskManage.AddJob();
                });
            }
        }
        /// <summary>
        /// 返回是否登录成功！
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="clientId"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        private static async ELTask<bool> MsgBusLogin(long accountId, long clientId, int clientType = 1)
        {
            var info = await Task.Run(async () =>
               {
                   var rtn = await LoginMsg(accountId, clientId, clientType);
                   if (rtn != null)
                       Boot.AddComponent<WebSocketGateComponent>();
                   return rtn;
               });
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    long timeNow = TimeHelper.ServerNow();
                    if (timeNow - RoboatServerComponent.LastRecvTime <= 4 * 10000)
                    {
                        Thread.Sleep(20 * 1000);
                        continue;
                    }
                    var rtn = System.Windows.Forms.MessageBox.Show($"通信失去连接，是否重连！", "错误", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
                    if (rtn == System.Windows.Forms.DialogResult.OK)
                    {
                        _ = await LoginMsg(accountId, clientId, clientType);
                        continue;
                    }
                    Application.Current.Shutdown();
                }
            });
            //Boot.GetComponent<TimerComponent>().NewRepeatedTimer(10000 * 4, async () =>
            //{

            //});
            return info != null;
        }

        private static async ELTask<G2C_Enter> LoginMsg(long accountId, long clientId, int clientType = 1)
        {
            var ip = ConfigurationManager.AppSettings["MsgBusIp"];
            LoginRequest request = new();
            if (!string.IsNullOrWhiteSpace(ip))
            {
                request.Ip = ip;
            }
            request.Port = 10001;
            request.AccountId = accountId;
            request.ClientId = clientId;
            request.ClientType = clientType;
            return await request.LoginAsync();
        }
        /// <summary>
        /// 获取某个控件特定类型的事件列表
        /// </summary>
        /// <param name="element"></param>
        /// <param name="routedEvent"></param>
        /// <returns></returns>
        public static RoutedEventHandlerInfo[] GetRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
        {
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(
                eventHandlersStore, new object[] { routedEvent });

            return routedEventHandlers;
        }
        private string GetNetMacAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            var localIP = ipEntry.AddressList.Where(t => t.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties property = nic.GetIPProperties();
                foreach (UnicastIPAddressInformation ip in property.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (ip.Address.ToString() == localIP.ToString())
                        {
                            return string.Join("-", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
                        }
                    }
                }
            }
            return GetMacAddressNew();
        }
        /// <summary>
        /// 获取网卡地址
        /// </summary>
        /// <returns>网卡地址</returns>
        private string GetMacAddressNew()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                var mac = string.Join("-", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed &&
                    !string.IsNullOrEmpty(tempMac) &&
                    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    maxSpeed = nic.Speed;
                    macAddress = mac;
                }
            }

            return macAddress;
        }
    }
}
