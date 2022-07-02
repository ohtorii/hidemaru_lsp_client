using System;
using HidemaruLspClient_BackEndContract;

namespace HidemaruLspClient.ComContract
{
    public sealed class ComClientLogger : ILspClientLogger
    {
        NLog.Logger logger_ = null;
        public ComClientLogger(string name)
        {
            logger_ = NLog.LogManager.GetLogger(name);
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
