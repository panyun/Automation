using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.SignalingService.Model
{
    /// <summary>
    /// 协议
    /// </summary>
    public class Message<T>
    {
        public string type { get; set; }
        public string role { get; set; }
        public string room { get; set; }
        public T data { get; set; }
    }
    public class RoleType
    {
        public const string Editor = "editor";
        public const string Mobile = "mobile";
    }
    public class MessageType
    {
        public const string Join = "join";
        public const string Ping = "ping";
        public const string Ready = "ready";
        public const string Start = "start";
        public const string Stop = "stop";
        public const string Close = "close";
        public const string Offer = "offer";
        public const string Answer = "answer";
        public const string Candidate = "candidate";
    }
    public class ReadyInfo
    {
        public bool ready { get; set; }
    }
}
