namespace LSP.Implementation
{
    /// <summary>
    /// ログのインターフェース
    /// </summary>
    public interface ILogger
	{
        bool IsFatalEnabled { get; }
		bool IsErrorEnabled { get; }
		bool IsWarnEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsDebugEnabled { get; }
		bool IsTraceEnabled { get; }
		void Trace(string message);
		void Debug(string message);
		void Info(string message);
		void Warn(string message);
		void Error(string message);
		void Fatal(string message);
	}
}
