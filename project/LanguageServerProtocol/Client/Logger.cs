using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Client
{
	/// <summary>
	/// 
	/// </summary>
	class Logger
	{
		public virtual bool IsFatalEnabled { get; }
		public virtual bool IsErrorEnabled { get; }
		public virtual bool IsWarnEnabled { get; }
		public virtual bool IsInfoEnabled { get; }
		public virtual bool IsDebugEnabled { get; }
		public virtual bool IsTraceEnabled { get; }
		public virtual void Trace(string message)	{ }
		public virtual void Debug(string message)	{ }
		public virtual void Info(string message)	{ }
		public virtual void Warn(string message)	{ }
		public virtual void Error(string message)	{ }
		public virtual void Fatal(string message)	{ }
	}

	/// <summary>
	/// 
	/// </summary>
	class NullLogger : Logger
	{
	}
}
