namespace HidemaruLspClient
{
    class LspClientLogger : LSP.Implementation.ILogger
	{
		public NLog.Logger nlog_ {get; private set;}

		public LspClientLogger(string name)
		{
			nlog_ = NLog.LogManager.GetLogger(name);
		}
		public bool IsFatalEnabled { get { return nlog_.IsFatalEnabled; } }
		public bool IsErrorEnabled { get { return nlog_.IsErrorEnabled; } }
		public bool IsWarnEnabled { get { return nlog_.IsWarnEnabled; } }
		public bool IsInfoEnabled { get { return nlog_.IsInfoEnabled; } }
		public bool IsDebugEnabled { get { return nlog_.IsDebugEnabled; } }
		public bool IsTraceEnabled { get { return nlog_.IsTraceEnabled; } }
        public void Fatal(string message)
        {
            nlog_.Fatal(message);
        }
        public void Error(string message)
        {
            nlog_.Error(message);
        }
        public void Warn(string message)
        {
            nlog_.Warn(message);
        }
        public void Info(string message)
        {
            nlog_.Info(message);
        }
        public void Debug(string message)
		{
            nlog_.Debug(message);
        }
        public void Trace(string message)
		{
			nlog_.Trace(message);
		}

    }
}
