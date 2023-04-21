using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Model.P2P
{
    public class RoleType
    {
        public const string Editor = "Editor";
        public const string Mobile = "Mobile";
    }
    public class MessageType
    {
        public const string Join = "Join";
        public const string Ready = "Ready";
        public const string Start = "Start";
        public const string Stop = "Stop";
        public const string Close = "Close";
        public const string CheckOut = "CheckOut";
        public const string Offer = "Offer";
        public const string Answer = "Answer";
        public const string Candidate = "Candidate";
    }
}
