namespace LSP.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    class NullLogger : ILogger
    {
        public bool IsFatalEnabled { get { return false; } }

        public bool IsErrorEnabled { get { return false; } }

        public bool IsWarnEnabled { get { return false; } }

        public bool IsInfoEnabled { get { return false; } }

        public bool IsDebugEnabled { get { return false; } }

        public bool IsTraceEnabled { get { return false; } }

        public void Debug(string message)
        {
        }

        public void Error(string message)
        {
        }

        public void Fatal(string message)
        {         
        }

        public void Info(string message)
        {         
        }

        public void Trace(string message)
        {         
        }

        public void Warn(string message)
        {         
        }
    }
}
