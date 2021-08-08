using System;
using System.Runtime.InteropServices;
using HidemaruLspClient_BackEndContract;

namespace HidemaruLspClient
{
    /*[ComVisible(true)]
    [Guid("733283BC-3011-434F-A93C-B517DAD1B8B4")]
    [ComDefaultInterface(typeof(ILspClientLogger))]*/
    public sealed class ComClientLogger : ILspClientLogger
    {
		LSP.Client.ILogger logger_ = null;
		public ComClientLogger(LSP.Client.ILogger logger)
        {
			logger_ = logger;

		}
        sbyte ILspClientLogger.IsFatalEnabled => Convert.ToSByte(logger_.IsFatalEnabled);

        sbyte ILspClientLogger.IsErrorEnabled => Convert.ToSByte(logger_.IsErrorEnabled);

        sbyte ILspClientLogger.IsWarnEnabled => Convert.ToSByte(logger_.IsWarnEnabled);

        sbyte ILspClientLogger.IsInfoEnabled => Convert.ToSByte(logger_.IsInfoEnabled);

        sbyte ILspClientLogger.IsDebugEnabled => Convert.ToSByte(logger_.IsDebugEnabled);

        sbyte ILspClientLogger.IsTraceEnabled => Convert.ToSByte(logger_.IsTraceEnabled);

        void ILspClientLogger.Debug(string message)
        {
			logger_.Debug(message);
        }

        void ILspClientLogger.Error(string message)
        {
			logger_.Error(message);
        }

        void ILspClientLogger.Fatal(string message)
        {
			logger_.Fatal(message);
        }

        void ILspClientLogger.Info(string message)
        {
			logger_.Info(message);
        }

        void ILspClientLogger.Trace(string message)
        {
			logger_.Trace(message);
        }

        void ILspClientLogger.Warn(string message)
        {
			logger_.Warn(message);
        }
    }
}
