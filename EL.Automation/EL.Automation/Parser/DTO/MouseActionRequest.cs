using Automation.Inspect;
using EL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
   
    public class MouseActionRequest : RequestBase
    {
        /// <summary>
        /// 事件点击、 鼠标模拟点击
        /// </summary>
        public ActionType ActionType { get; set; }
        /// <summary>
        /// 中心、左上、左下、右上、右下
        /// </summary>
        public LocationType LocationType { get; set; }
        /// <summary>
        /// 左键单击 中键单击 右键单击 左键双击
        /// </summary>
        public ClickType ClickType { get; set; }
        /// <summary>
        /// x坐标偏差值
        /// </summary>
        public int OffsetX { get; set; }
        /// <summary>
        /// Y坐标偏差值
        /// </summary>
        public int OffsetY { get; set; }


    }
    public class MouseActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }

    }
}
