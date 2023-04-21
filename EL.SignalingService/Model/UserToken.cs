using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EL.SignalingService.Model
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// websocket对象
        /// </summary>
        public WebSocket WebSocket { get; set; }
        /// <summary>
        /// 连接的时间
        /// </summary>
        public DateTime ConnectTime { get; set; }
        /// <summary>
        /// 远程地址
        /// </summary>
        public EndPoint RemoteAddress { get; set; }
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public IPAddress IPAddress { get; set; }
        /// <summary>
        /// 一个自定义ID字段
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 一个自定义Role字段
        /// </summary>
        public string Role { get; set; }
        public bool IsLogin { get { return !string.IsNullOrEmpty(ID) && !string.IsNullOrEmpty(Role); } }
    }
}
