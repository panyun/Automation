namespace RPCBus.Server
{
    public class ETException : Exception
    {
        public int ErrorCode { get; set; } = default;
        public ETException(string message, int errorCode) : base(message)
        {
            this.ErrorCode = errorCode;
        }
    }
    
}
