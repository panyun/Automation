using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.PIP.Model
{
    /// <summary>
    /// PIPState，获取PIPState的状态
    /// </summary>
    public enum PIPState
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,
        /// <summary>
        /// 服务安装
        /// </summary>
        ServerInstall,
        /// <summary>
        /// 服务运行中
        /// </summary>
        ServerRun,
        /// <summary>
        /// 服务关闭
        /// </summary>
        ServerClose,
        /// <summary>
        /// 已连接
        /// </summary>
        Connected,
        /// <summary>
        /// 已关闭
        /// </summary>
        Close
    }
}
