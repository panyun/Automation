using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component
{

    public class ELNodeHandlerException : Exception
    {
        public int ErrorCode { get; set; } = default;
        public ELNodeHandlerException(string message, int errorCode = 600) : base(message)
        {
            this.ErrorCode = errorCode;
        }
        public ELNodeHandlerException(string message, Exception exception, int errorCode = 600) : base(message, exception)
        {
            this.ErrorCode = errorCode;
        }
    }
	public class DesignException : Exception
	{
		public int ErrorCode { get; set; } = default;
		public DesignException(string message, int errorCode = 600) : base(message)
		{
			this.ErrorCode = errorCode;
		}
		public DesignException(string message, Exception exception, int errorCode = 600) : base(message, exception)
		{
			this.ErrorCode = errorCode;
		}
	}
}
