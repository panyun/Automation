using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace EL.Exceptions
{
    [Serializable]
    public class ELException : Exception
    {
        public ELException()
        {
        }

        public ELException(string message)
            : base(message)
        {
        }

        public ELException(Exception innerException)
            : base(String.Empty, innerException)
        {
        }

        public ELException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ELException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
