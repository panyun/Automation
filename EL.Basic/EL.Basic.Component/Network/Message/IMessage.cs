using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Basic.Network
{
    public static class OpcodeHelper
    {
        private static readonly HashSet<ushort> ignoreDebugLogMessageSet = new HashSet<ushort>();

        private static bool IsNeedLogMessage(ushort opcode)
        {
            if (ignoreDebugLogMessageSet.Contains(opcode))
            {
                return false;
            }

            return true;
        }

        public static void AddIgnoreDebugLogMessage(ushort opcode)
        {
            ignoreDebugLogMessageSet.Add(opcode);
        }

        public static bool IsOuterMessage(ushort opcode)
        {
            return opcode >= 20000;
        }

        public static bool IsInnerMessage(ushort opcode)
        {
            return opcode < 20000;
        }

        public static void LogMsg(int zone, ushort opcode, object message)
        {
            if (!IsNeedLogMessage(opcode))
            {
                return;
            }

            Log.Debug("zone: {0} {1}", zone, message);
        }

        public static void LogMsg(ushort opcode, long actorId, object message)
        {
            if (!IsNeedLogMessage(opcode))
            {
                return;
            }

            Log.Debug("actorId: {0} {1}", actorId, message);
        }
    }
    /// <summary>
    /// RPC异常,带ErrorCode
    /// </summary>
    [Serializable]
    public class RpcException : Exception
    {
        public int Error
        {
            get;
            private set;
        }

        public RpcException(int error, string message) : base($"Error: {error} Message: {message}")
        {
            this.Error = error;
        }

        public RpcException(int error, string message, Exception e) : base($"Error: {error} Message: {message}", e)
        {
            this.Error = error;
        }
    }
   

    public class ErrorResponse : IResponse
    {
        public int Error
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public int RpcId
        {
            get;
            set;
        }
        public string StackTrace { get; set; }
    }
}
