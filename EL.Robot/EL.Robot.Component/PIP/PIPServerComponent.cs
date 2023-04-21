using EL.InstallationService;
using EL.InstallationService.Common;
using EL.Robot.Component.PIP.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.PIP
{
    /// <summary>
    /// PIP 服务组件 (会挂载到Robot上)
    /// 1. 启动PIP客户端
    /// 2. 开启PIP 接口服务
    /// 3. 与PIP 进行通讯
    /// </summary>
    public class PIPServerComponent : Entity
    {
        private PIPWebServer PIPWebServer { get; set; }
        public int PingTime { get; set; } = 20 * 1000;
        /// <summary>
        /// 当前服务的状态
        /// </summary>
        public PIPState State { get { return CheckState(); } }
        public bool IsRuning { get { return PIPWebServer?.Connected == true; } }
        public bool Wating { get; set; }
        public bool Ready { get; set; }
        public string RoomName { get { return TaskSchedulerHelper.GetTaskSchedulerPath(); } }
        public int Port { get { return SystemServerHelper.GetServerPort(); } }
        public Action<string> Receive { get; set; }

        public async Task<bool> Conn()
        {
            if (!IsRuning)
            {
                var WebApiState = WebApiServerState();
                if (!WebApiState)
                {
                    Install();
                    Thread.Sleep(500);
                    if (CheckServerState() != PIPState.ServerRun)
                    {
                        ServerStart();
                        Thread.Sleep(500);
                    }
                }
                WebApiState = WebApiServerState();
                if (WebApiState)
                {
                    var pip = await WebApiConn();
                    if (pip == PIPState.Connected)
                    {
                        return true;
                    }
                }
            }
            return (true);
        }

        public bool Send(string msg)
        {
            if (Ready)
            {
                PIPWebServer.Send(ToJson(new Message<string>() { room = RoomName, role = RoleType.Server, type = MessageType.Data, data = msg }));
                return true;
            }
            return false;
        }
        public bool Send(FlowScript msg)
        {
            if (Ready)
            {
                PIPWebServer.Send(ToJson(new Message<string>() { room = RoomName, role = RoleType.Server, type = MessageType.Data, data = ToJson(msg) }));
                return true;
            }
            return false;
        }
        private PIPState CheckState()
        {
            var pipState = PIPState.None;
            pipState = CheckServerState();
            if (pipState != PIPState.ServerRun)
            {
                return pipState;
            }
            pipState = PIPState.ServerRun;
            if (!CheckWebApi())
            {
                pipState = PIPState.Close;
                return pipState;
            }
            pipState = PIPState.Connected;
            return pipState;
        }
        private PIPState CheckServerState()
        {
            PIPState pIPState = PIPState.ServerInstall;
            if (SystemServerHelper.IsInstallation())
            {
                if (SystemServerHelper.IsRuning())
                {
                    pIPState = PIPState.ServerRun;
                }
                else
                {
                    pIPState = PIPState.ServerClose;
                }
            }
            return pIPState;
        }
        private bool CheckWebApi()
        {
            return false;
        }
        private void Install()
        {
            SystemServerHelper.Installation(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EL.PIPSystemServer"));
            //启动两个是为了保险，因为有时候有一个会启动不起来,可能需要系统重启后才能起来。
            RunHelper.SetRun(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EL.Robot.Execute.exe"));
            TaskSchedulerHelper.AddTaskScheduler(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EL.Robot.Execute.exe"));
        }
        private bool ServerStart()
        {
            return SystemServerHelper.Start();
        }
        private async Task<PIPState> WebApiConn()
        {
            try
            {
                if (PIPWebServer != null)
                {
                    PIPWebServer?.Close();
                }
            }
            catch { }

            PIPWebServer = new($"ws://127.0.0.1:{Port}");
            Ready = false;
            PIPWebServer.OnOpen = () =>
            {
                Wating = true;
                Console.WriteLine("已连接服务");
                PIPWebServer.Send(ToJson(new Message<string>() { room = RoomName, role = RoleType.Server, type = MessageType.Join, data = "服务端登录" }));
                _ = Task.Run(() =>
                {
                    while (Wating)
                    {
                        PIPWebServer.Send(ToJson(new Message<dynamic>() { type = MessageType.Ping }));
                        Thread.Sleep(PingTime);
                    }
                });
            };
            PIPWebServer.OnReceive = (data) =>
            {
                var oldData = Encoding.UTF8.GetString(data);
                try
                {
                    var message = ToObj<Message<dynamic>>(oldData);
                    switch (message.type)
                    {
                        case MessageType.Ready:
                            {
                                Ready = true;
                            }
                            break;
                        case MessageType.Stop:
                            {
                                Ready = false;
                            }
                            break;
                        case MessageType.Data:
                            {
                                Receive?.Invoke(oldData);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };

            PIPWebServer.OnClose = () =>
            {
                Ready = false;
                Wating = false;
                Console.WriteLine("已断开服务");
            };
            await PIPWebServer.Start();
            return PIPState.Connected;
        }
        private bool WebApiServerState()
        {
            if (SystemServerHelper.IsRuning())
            {
                var port = Port;
                if (port > 0)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://127.0.0.1:{port}");
                    request.Timeout = 20000;
                    request.Method = "get";
                    request.ContentType = "application/json";
                    request.Proxy = null;
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        string postContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        return response.StatusCode == HttpStatusCode.OK;
                    }
                    catch { }
                }
            }
            return false;
        }
        public bool StartPIP()
        {
            if (!Process.GetProcessesByName("EL.PIP").Any())
            {
                PIPHelper.StartPIP(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EL.PIP.exe"));
            }
            return Process.GetProcessesByName("EL.PIP").Length > 0;
        }
        public async Task<(bool, string)> StartAsync()
        {
            var reslut = await Conn();
            SpinWait.SpinUntil(() => IsRuning, 5 * 1000);
            if (reslut && IsRuning)
            {
                if (StartPIP())
                {
                    SpinWait.SpinUntil(() => Ready, 5 * 1000);
                    return (true, null);
                }
                else
                {
                    return (false, "启动画中画失败!");
                }
            }
            else
            {
                return (false, "连接画中画服务失败!");
            }
        }
        private static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
        private static T ToObj<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
        public override void Dispose()
        {
            base.Dispose();
            PIPWebServer?.Close();
        }
    }
}
