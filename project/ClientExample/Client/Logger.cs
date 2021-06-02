using LSP.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientExample
{
	class Logger : LSP.Client.ILogger
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
}
