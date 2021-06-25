using LSP.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientExample
{
	class Logger : LSP.Client.Logger
	{
		static NLog.Logger logger = null;

		public Logger(string logFilename)
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
		public override bool IsFatalEnabled { get { return logger.IsFatalEnabled; } }
		public override bool IsErrorEnabled { get { return logger.IsErrorEnabled; } }
		public override bool IsWarnEnabled { get { return logger.IsWarnEnabled; } }
		public override bool IsInfoEnabled { get { return logger.IsInfoEnabled; } }
		public override bool IsDebugEnabled { get { return logger.IsDebugEnabled; } }
		public override bool IsTraceEnabled { get { return logger.IsTraceEnabled; } }

		public override void Debug(string message)
		{
			logger.Debug(message);
		}

		public override void Error(string message)
		{
			logger.Error(message);
		}

		public override void Fatal(string message)
		{
			logger.Fatal(message);
		}

		public override void Info(string message)
		{
			logger.Info(message);
		}

		public override void Trace(string message)
		{
			logger.Trace(message);
		}

		public override void Warn(string message)
		{
			logger.Warn(message);
		}
	}
}
