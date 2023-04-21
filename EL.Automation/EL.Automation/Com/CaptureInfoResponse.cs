using EL;

namespace Automation.Com
{
    /// <summary>
    /// 截图信息返回结果
    /// </summary>
    public class CaptureInfoResponse : Response
    {
        /// <summary>
        /// 窗口标题
        /// </summary>
        public string TitleName { get; set; }
        /// <summary>
        /// 对应的x坐标
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// 对应的y坐标
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 截图路径
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 流程id
        /// </summary>
        public int ProcessId { get; set; }
        /// <summary>
        /// 节点对应的类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 节点对应的当前窗口
        /// </summary>
        public IntPtr CurrIntptr { get; set; }

    }
}
