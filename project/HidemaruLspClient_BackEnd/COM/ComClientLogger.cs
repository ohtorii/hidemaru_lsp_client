using System;
using System.Runtime.InteropServices;

namespace HidemaruLspClient
{
    [ComVisible(true)]
    [Guid("733283BC-3011-434F-A93C-B517DAD1B8B4")]
    [ComDefaultInterface(typeof(ILspClientLogger))]
    public sealed class ComClientLogger : ILspClientLogger
    {
		LSP.Client.ILogger logger_ = null;
		public ComClientLogger(LSP.Client.ILogger logger)
        {
			logger_ = logger;

		}
		public bool IsFatalEnabled { get { return logger_.IsFatalEnabled; } }

        public bool IsErrorEnabled { get { return logger_.IsErrorEnabled; } }

		public bool IsWarnEnabled { get { return logger_.IsWarnEnabled; } }

		public bool IsInfoEnabled { get { return logger_.IsInfoEnabled; } }

		public bool IsDebugEnabled { get { return logger_.IsDebugEnabled; } }

		public bool IsTraceEnabled { get { return logger_.IsTraceEnabled; } }

		public void Debug(string message)
        {
			logger_.Debug(message);
        }

        public void Error(string message)
        {
			logger_.Error(message);
        }

        public void Fatal(string message)
        {
			logger_.Fatal(message);
        }

        public void Info(string message)
        {
			logger_.Info(message);
        }

        public void Trace(string message)
        {
			logger_.Trace(message);
        }

        public void Warn(string message)
        {
			logger_.Warn(message);
        }
    }
}
