using LSP.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
	class LspClientLogger : LSP.Client.Logger
	{
		NLog.Logger logger = null;
		public NLog.Logger GetLogger()
        {
			return logger;
        }

		public LspClientLogger(string logFilename)
		{
			{
				var config = new NLog.Config.LoggingConfiguration();

				// Targets where to log to: File and Console
				var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logFilename };
				//var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

				// Rules for mapping loggers to targets            
				//config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
				config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);

				// Apply config
				NLog.LogManager.Configuration = config;
			}
			logger = NLog.LogManager.GetCurrentClassLogger();
		}
		override public bool IsFatalEnabled { get { return logger.IsFatalEnabled; } }
		override public bool IsErrorEnabled { get { return logger.IsErrorEnabled; } }
		override public bool IsWarnEnabled { get { return logger.IsWarnEnabled; } }
		override public bool IsInfoEnabled { get { return logger.IsInfoEnabled; } }
		override public bool IsDebugEnabled { get { return logger.IsDebugEnabled; } }
		override public bool IsTraceEnabled { get { return logger.IsTraceEnabled; } }

		override public void Debug(string message)
		{
			logger.Debug(message);
		}

		override public void Error(string message)
		{
			logger.Error(message);
		}

		override public void Fatal(string message)
		{
			logger.Fatal(message);
		}

		override public void Info(string message)
		{
			logger.Info(message);
		}

		override public void Trace(string message)
		{
			logger.Trace(message);
		}

		override public void Warn(string message)
		{
			logger.Warn(message);
		}
	}

	class EnterLeaveLogger
    {
        NLog.Logger logger;
		string methodName;
		public EnterLeaveLogger(string methodName, NLog.Logger logger)
        {
			this.logger = logger;
			this.methodName = methodName;
			this.logger.Trace("{0} Enter.", this.methodName);
		}
        ~EnterLeaveLogger()
        {
			this.logger.Trace("{0} Leave.", this.methodName);
		}
	}
}
