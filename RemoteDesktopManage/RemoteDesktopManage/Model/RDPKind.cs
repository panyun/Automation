using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDesktopManage.Model
{
    /// <summary>
    /// 远程桌面的状态
    /// </summary>
    public enum RDPKind
    {
        [Description("未连接")]
        NoConn,
        [Description("连接中")]
        Conning,
        [Description("登录完成")]
        LoginComplete,
        [Description("登录异常")]
        LogonError,
        [Description("已连接")]
        Connected,
        [Description("断开连接")]
        Disconnected,
        [Description("异常")]
        Error,
    }
}
