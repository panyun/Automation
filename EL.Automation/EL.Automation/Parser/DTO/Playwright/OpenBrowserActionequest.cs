namespace Automation.Parser
{
    public enum BrowserType
    {
        Chromium,
        Msedge,
        Firefox,
        Webkit
    }
    public class OpenBrowserActionRequest : EL.IRequest
    {
        public int RpcId { get; set; }
        public BrowserType BrowserType { get; set; }
        public string Url { get; set; }
    }
    public class OpenBrowserActionResponse : EL.IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }
    }
}
