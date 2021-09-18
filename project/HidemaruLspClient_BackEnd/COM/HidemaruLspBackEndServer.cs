using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HidemaruLspClient_BackEndContract;


namespace HidemaruLspClient
{
    /*[ComVisible(true)]
    [Guid(LspContract.Constants.ServerClass)]
    [ProgId(LspContract.Constants.ProgId)]
    [ComDefaultInterface(typeof(IHidemaruLspBackEndServer))]*/
    public sealed partial class HidemaruLspBackEndServer : IHidemaruLspBackEndServer
    {
        static LspClientLogger lspClientLogger_ = null;
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
        static Dictionary<LspKey, WorkerPair> workerHolder_ = new Dictionary<LspKey, WorkerPair>();




        public HidemaruLspBackEndServer()
        {
            COMRegistration.Ole32.CoAddRefServerProcess();
        }
        ~HidemaruLspBackEndServer()
        {
            if (COMRegistration.Ole32.CoReleaseServerProcess() == 0)
            {
                const int success = 0;
                Environment.Exit(success);
            }
        }

        [LogMethod]
        int IHidemaruLspBackEndServer.Add(int x, int y)
        {
            return x + y;
        }

        [LogMethod]
        /// <summary>
        /// コンストラクタ
        /// (Memo)アウトプロセスサーバなので createobject するたびに呼ばれる
        /// </summary>
        sbyte IHidemaruLspBackEndServer.Initialize(string logFileName)
        {
            if (lspClientLogger_ == null)
            {
                lspClientLogger_ = new LspClientLogger(logFileName);
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
        ILspClientLogger IHidemaruLspBackEndServer.GetLogger()
        {
            if (comClientLogger_ == null)
            {
                Debug.Assert(lspClientLogger_ != null);
                comClientLogger_ = new ComClientLogger(lspClientLogger_);
            }
            return comClientLogger_;
        }

        [LogMethod]
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
            long   HidemaruProcessId)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var holderKey = new LspKey(ServerName, RootUri);
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
            }catch(Exception e)
            {
                logger.Error(e);
            }
            return null;
        }

        [LogMethod]
        void IHidemaruLspBackEndServer.DestroyWorker(IWorker worker)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var ins = worker as Worker;
                Debug.Assert(ins != null);

                var holderKey = ins.key;
                var value = workerHolder_[holderKey];

                logger.Debug("value.referenceCounter={0}",value.referenceCounter);
                if (value.UnUsed())
                {
                    var ret = workerHolder_.Remove(holderKey);
                    Debug.Assert(ret == true);

                    ins.Stop();
                }
            }catch(Exception e)
            {
                logger.Error(e);
            }
        }
    }
}
