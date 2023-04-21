namespace Automation.Parser
{
    public enum RequestType
    {
        /// <summary>
        /// 抓取节点
        /// </summary>
        CatchUIRequest = 10000,
        /// <summary>
        /// 目标点击
        /// </summary>
        MouseActionRequest = 10001,
        /// <summary>
        /// 目标输入
        /// </summary>
        InputActionRequest = 10002,
        /// <summary>
        /// 目标高亮
        /// </summary>
        ElementActionRequest = 10003,
        /// <summary>
        /// 目标验证
        /// </summary>
        ElementVerificationActionRequest = 10004,
        /// <summary>
        /// 聊天窗口探测
        /// </summary>
        CatchElementChatRequest = 10005,

        /// <summary>
        /// 聊天消息识别
        /// </summary>
        StartMsgActionRequest = 10006,
        /// <summary>
        /// 修改元素
        /// </summary>
        UpdateElement = 10007,
        /// <summary>
        /// 生成相似元素请求
        /// </summary>
        GenerateSimilarElementActionRequest = 10008,
        /// <summary>
        /// 节点属性获取
        /// </summary>
        ElementPropertyActionRequest = 10009,
        /// <summary>
        /// 相似元素请求
        /// </summary>
        SimilarElementActionRequest = 10010,
        /// <summary>
        /// 余弦相似度
        /// </summary>
        GenerateCosineSimilarActionRequest = 10011,
        /// <summary>
        /// 余弦相似度
        /// </summary>
        CosineSimilarElementActionRequest = 10012,
        /// <summary>
        /// 捕获表格数据
        /// </summary>
        GenerateTableActionRequest = 10013,
        /// <summary>
        /// 提取excel文件数据
        /// </summary>
        GenerateExcelDataActionRequest = 10014,
        /// <summary>
        /// 抓取窗口
        /// </summary>
        CatchWindowRequest = 10015,
        /// <summary>
        /// 获取Html对象
        /// </summary>
        GenerateHtmlActionRequest = 10016,
        /// <summary>
        /// PlayWright高亮
        /// </summary>
        HighlightActionRequest = 10017,
        /// <summary>
        /// Playwright打开浏览器
        /// </summary>
        OpenBrowserActionRequest = 10018,
        /// <summary>
        /// 获取子节点信息
        /// </summary>
        ChildsElementActionRequest = 10019,
        /// <summary>
        /// 获取父节点信息
        /// </summary>
        ParentElementActionRequest = 10020
    }
}
