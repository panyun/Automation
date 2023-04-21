using System;
using System.Runtime.Serialization;

namespace EL.JavaBridge.Installer
{
    // Token: 0x0200000F RID: 15
    public class JavaBridgeInstallException : Exception
    {
        // Token: 0x06000035 RID: 53 RVA: 0x00002D10 File Offset: 0x00000F10
        public JavaBridgeInstallException()
        {
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00002D18 File Offset: 0x00000F18
        public JavaBridgeInstallException(string message) : base(message)
        {
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00002D21 File Offset: 0x00000F21
        public JavaBridgeInstallException(string message, Exception innerException) : base(message, innerException)
        {
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00002D2B File Offset: 0x00000F2B
        protected JavaBridgeInstallException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
