using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL
{
    public interface IMessage
    {
    }

    public interface IRequest : IMessage
    {
        int RpcId
        {
            get;
            set;
        }
    }

    public interface IResponse : IMessage
    {
        int Error
        {
            get;
            set;
        }

        string Message
        {
            get;
            set;
        }

        int RpcId
        {
            get;
            set;
        }
        string StackTrace
        {
            get; set;
        }
    }
    public class Response : IResponse
    {
        public Response()
        {
            Error = 0;
            Message = string.Empty;
            RpcId = 0;
        }
        public int Error { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int RpcId { get; set; }
        public void SetError(string msg, int error)
        {
            Error = error;
            Message = msg;
        }
    }
}
