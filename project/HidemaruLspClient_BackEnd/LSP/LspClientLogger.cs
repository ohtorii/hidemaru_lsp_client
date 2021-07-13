﻿using LSP.Client;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class LspClientLogger : LSP.Client.ILogger
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
		public bool IsFatalEnabled { get { return logger.IsFatalEnabled; } }
		public bool IsErrorEnabled { get { return logger.IsErrorEnabled; } }
		public bool IsWarnEnabled { get { return logger.IsWarnEnabled; } }
		public bool IsInfoEnabled { get { return logger.IsInfoEnabled; } }
		public bool IsDebugEnabled { get { return logger.IsDebugEnabled; } }
		public bool IsTraceEnabled { get { return logger.IsTraceEnabled; } }

		public void Debug(string message)
		{
			logger.Debug(message);
		}

		public void Error(string message)
		{
			logger.Error(message);
		}

		public void Fatal(string message)
		{
			logger.Fatal(message);
		}

		public void Info(string message)
		{
			logger.Info(message);
		}

		public void Trace(string message)
		{
			logger.Trace(message);
		}

		public void Warn(string message)
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
