using EL.Async;
using EL.Robot.Core.Websocket;
using Newtonsoft.Json;
using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EL.Robot.Core.Request
{

    [Message(OuterOpcode.LoginRequest)]
    public class LoginRequest
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        [JsonIgnore]
        public IPAddress IPIPAddress { get { return IPAddress.Parse(Ip); } }
        public string SocketUrl => @"wss://msgbus.rpaii.com/auth/";
        public long AccountId { get; set; }
        public long ClientId { get; set; }
        public int ClientType { get; set; }
        [JsonIgnore]
        private IPEndPoint _ipEndPoint;
        [JsonIgnore]
        public IPEndPoint IPEndPoint
        {
            get
            {
                if (Ip == null) return default;
                if (_ipEndPoint == null)
                    _ipEndPoint = new IPEndPoint(IPIPAddress, Port);
                return _ipEndPoint;
            }
        }
        [JsonIgnore]
        private WChannel _channel;
        [JsonIgnore]
        public WChannel Channel
        {
            get
            {
                if (_channel != null) return _channel;
                if (IPEndPoint != default)
                    return new WChannel(IPEndPoint);
                else
                    return new WChannel(SocketUrl);
            }
        }
        /// <summary>
        /// 当前socket连接
        /// </summary>
        public WChannel CurrentTChannel { get; set; }
        /// <summary>
        /// websocket连接
        /// </summary>
        //public IWebSocketConnection WebSocket { get; set; }
    }
    public static class LoginRequestSystem
    {
        public static async ELTask<G2C_Enter> LoginAsync(this LoginRequest loginRequest)
        {
            try
            {
                var response = (R2C_Login)await loginRequest.Channel.Call(new C2R_Login()
                {
                    AccountId = loginRequest.AccountId,
                    ClientId = loginRequest.ClientId,
                    ClientType = loginRequest.ClientType
                });
                if (response.Error != 0) throw new Exception("登录失败！");
                MsgAgentComonpment.Instance.SelfClientId = loginRequest.ClientId;
                RoboatServerComponent.CurrentUser = response;
                try
                {
                    if (!string.IsNullOrWhiteSpace(loginRequest.Ip))
                    {
                        var ipAddress = IPAddress.Parse(loginRequest.Ip);
                        IPEndPoint ipep = new IPEndPoint(ipAddress, response.Port);
                        loginRequest.CurrentTChannel = new WChannel(ipep);
                    }
                    else
                        loginRequest.CurrentTChannel = new WChannel(@"wss://msgbus.rpaii.com/bus/");
                    RoboatServerComponent.CurrentTChannel = loginRequest.CurrentTChannel;
                }
                catch (Exception ex)
                {
                    return default;
                }
                var resEnter = (G2C_Enter)await loginRequest.CurrentTChannel.Call(new C2G_Enter() { Key = response.Key });
                loginRequest.Channel.Dispose();
                loginRequest.HeartBeat();
                return resEnter;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return default;
        }

        public static void HeartBeat(this LoginRequest loginRequest)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(10000);
                    try
                    {
                        var response = await loginRequest.CurrentTChannel.Call(new C2G_Ping());
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
            });


        }
    }
}
