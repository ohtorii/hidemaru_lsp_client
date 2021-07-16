﻿using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    [ComVisible(true)]
    [Guid(LspContract.Constants.ServerClass)]
    [ProgId(LspContract.Constants.ProgId)]
    [ComDefaultInterface(typeof(IHidemaruLspBackEndServer))]
    public sealed partial class HidemaruLspBackEndServer : IHidemaruLspBackEndServer
    {
        static LspClientLogger lspClientLogger_ = null;
        static ComClientLogger comClientLogger_ = null;

        class WorkerPair
        {
            public WorkerPair(Worker w)
            {
                worker = w;
                Used();
            }            
            Worker worker =null;
            public Worker GetWorker()
            {
                return worker;
            }
            public void Used()
            {
                referenceCounter += 1;
            }
            public bool UnUsed()
            {
                referenceCounter -= 1;
                if (referenceCounter == 0)
                {
                    //worker.Destroy();
                    //worker = null;
                    return true;
                }
                return false;
            }
            int referenceCounter = 0;
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
        bool IHidemaruLspBackEndServer.Initialize(string logFileName)
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
                return false;
            }
            return true;
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
            string WorkspaceConfig)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var target = new LspKey(ServerName, RootUri);                
                if (workerHolder_.ContainsKey(target))
                {
                    var w = workerHolder_[target];
                    w.Used();
                    return w.GetWorker();
                }
                var ins = new Worker(target);
                var ret = ins.Start(  ServerName,
                                    ExcutablePath,
                                    Arguments,
                                    RootUri,
                                    WorkspaceConfig);
                if (ret == false)
                {
                    return null;
                }
                workerHolder_[target] = new WorkerPair(ins);
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
            var ins = worker as Worker;
            Debug.Assert(ins != null);
            
            var key = ins.key;
            var value = workerHolder_[key];
            //Debug.Assert(value.worker.key == key);

            if (value.UnUsed())
            {
                var ret = workerHolder_.Remove(key);
                Debug.Assert(ret == true);

                ins.Stop();
            }
        }
    }
}
