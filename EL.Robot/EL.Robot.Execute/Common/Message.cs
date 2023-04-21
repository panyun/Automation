using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Execute.Common
{
    /// <summary>
    /// 协议
    /// </summary>
    public class Message<T>
    {
        public string room { get; set; }
        public string type { get; set; }
        public string role { get; set; }
        public T data { get; set; }
    }
    public class RoleType
    {
        public const string Client = "client";
        public const string Server = "server";
    }
    public class MessageType
    {
        public const string Join = "join";
        public const string Ready = "ready";
        public const string Stop = "stop";
        public const string Ping = "ping";
        public const string Data = "data";
    }
}
