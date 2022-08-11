using HidemaruLspClient.Native;
using HidemaruLspClient_BackEndContract;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HidemaruLspClient.ComContract
{
    public sealed class HidemaruLspBackEndServer : IHidemaruLspBackEndServer
    {
        static LspClient.Logger lspClientLogger_ = null;
        static ComClientLogger comClientLogger_ = null;

        class WorkerPair
        {
            Worker worker_ = null;
            int referenceCounter_ = 0;


            public WorkerPair(Worker w)
            {
                worker_ = w;
                Used();
            }
            public Worker GetWorker()
            {
                return worker_;
            }
            public void Used()
            {
                referenceCounter_ += 1;
            }
            public bool UnUsed()
            {
                referenceCounter_ -= 1;
                if (referenceCounter_ == 0)
                {
                    return true;
                }
                return false;
            }
            /// <summary>
            /// デバッグ用途
            /// </summary>
            public int referenceCounter { get { return referenceCounter_; } }
        }
        static Dictionary<LanguageServerIdentifier, WorkerPair> workerHolder_ = new Dictionary<LanguageServerIdentifier, WorkerPair>();




        public HidemaruLspBackEndServer()
        {
            Ole32.CoAddRefServerProcess();
        }
        ~HidemaruLspBackEndServer()
        {
            if (Ole32.CoReleaseServerProcess() == 0)
            {
                const int success = 0;
                Environment.Exit(success);
            }
        }

        int IHidemaruLspBackEndServer.Add(int x, int y)
        {
            return x + y;
        }

        /// <summary>
        /// コンストラクタ
        /// (Memo)アウトプロセスサーバなので createobject するたびに呼ばれる
        /// </summary>
        sbyte IHidemaruLspBackEndServer.Initialize()
        {
            if (lspClientLogger_ == null)
            {
                lspClientLogger_ = new LspClient.Logger(HidemaruLspClient.Constant.Logger.HeaderClient);
            }

            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                Worker.Initialize(lspClientLogger_);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Convert.ToSByte(false);
            }
            return Convert.ToSByte(true);
        }
        void IHidemaruLspBackEndServer.EnableSendCrashReport(sbyte value)
        {
            MicrosoftAppCenter.EnableSendCrashReport = Convert.ToBoolean(value);
        }

        ILspClientLogger IHidemaruLspBackEndServer.GetLogger(string name)
        {
            if (comClientLogger_ == null)
            {
                Debug.Assert(lspClientLogger_ != null);
                comClientLogger_ = new ComClientLogger(name);
            }
            return comClientLogger_;
        }

        /// <summary>
        /// LSPサーバを起動する
        /// </summary>
        /// <param name="serverConfigFilename"></param>
        /// <param name="currentSourceCodeDirectory"></param>
        /// <returns></returns>
        IWorker IHidemaruLspBackEndServer.CreateWorker(
            string ServerName,
            string ExcutablePath,
            string Arguments,
            string RootUri,
            string WorkspaceConfig,
            long HidemaruProcessId)
        {
            NLog.Logger logger = null;
            try
            {
                logger = LogManager.GetCurrentClassLogger();
                var holderKey = new LanguageServerIdentifier(ServerName, RootUri);
                if (workerHolder_.ContainsKey(holderKey))
                {
                    var w = workerHolder_[holderKey];
                    w.Used();
                    logger.Debug("w.referenceCounter={0}", w.referenceCounter);
                    return w.GetWorker();
                }

                var ins = new Worker(holderKey);
                var ret = ins.Start(ServerName,
                                    ExcutablePath,
                                    Arguments,
                                    RootUri,
                                    WorkspaceConfig,
                                    HidemaruProcessId);
                if (ret == false)
                {
                    return null;
                }
                var value = new WorkerPair(ins);
                logger.Debug("value.referenceCounter={0}", value.referenceCounter);
                workerHolder_[holderKey] = value;
                return ins;
            }
            catch (Exception e)
            {
                logger?.Error(e);
            }
            return null;
        }

        void IHidemaruLspBackEndServer.DestroyWorker(IWorker worker)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ins = worker as Worker;
                Debug.Assert(ins != null);

                var holderKey = ins.key;
                var value = workerHolder_[holderKey];

                logger.Debug("value.referenceCounter={0}", value.referenceCounter);
                if (value.UnUsed())
                {
                    var ret = workerHolder_.Remove(holderKey);
                    Debug.Assert(ret == true);

                    ins.Stop();
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }
    }
}
