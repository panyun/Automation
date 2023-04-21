using NLog;
using System.Diagnostics;

namespace RPCBus.Server
{
    public class DevLogger : ET.ILog
    {
        private readonly Logger logger;

        public DevLogger(string name)
        {
            this.logger = LogManager.GetLogger(name);
        }

        public void Trace(string message)
        {
            System.Console.WriteLine(message);
            this.logger.Trace(message);
        }

        public void Warning(string message)
        {
            System.Console.WriteLine(message);
            this.logger.Warn(message);
        }

        public void Info(string message)
        {
            System.Console.WriteLine(message);
            this.logger.Info(message);
        }

        public void Debug(string message)
        {
            System.Console.WriteLine(message);
            this.logger.Debug(message);
        }

        public void Error(string message)
        {
            //todo:error错误已经默认打印在控制台界面
            //System.Console.WriteLine(message);
            this.logger.Error(message);
        }

        public void Fatal(string message)
        {
            System.Console.WriteLine(message);
            this.logger.Fatal(message);
        }

        public void Trace(string message, params object[] args)
        {
            this.Trace(string.Format(message, args));
        }

        public void Warning(string message, params object[] args)
        {
            this.Warning(string.Format(message, args));
        }

        public void Info(string message, params object[] args)
        {
            this.Info(string.Format(message, args));
        }

        public void Debug(string message, params object[] args)
        {
            this.Debug(string.Format(message, args));
        }

        public void Error(string message, params object[] args)
        {
            StackTrace st = new StackTrace(1, true);
            message = $"{message}\n{st}";
            this.Error(string.Format(message, args));
        }

        public void Fatal(string message, params object[] args)
        {
            StackTrace st = new StackTrace(1, true);
            message = $"{message}\n{st}";
            this.Fatal(string.Format(message, args));
        }
    }
}
