using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class TargetServer : ITargetServer
    {
        public string ServerName { get; set; }
        public string RootUri { get; set; }

        public void Initialize()
        {
            ServerName = "";
            RootUri = "";
        }
    }
    [ComVisible(true)]
    [Guid(LspContract.Constants.ServerClass)]
    [ProgId(LspContract.Constants.ProgId)]
    [ComDefaultInterface(typeof(IHidemaruLspBackEndServer))]
    public sealed class HidemaruLspBackEndServer : IHidemaruLspBackEndServer
    {
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
        int IHidemaruLspBackEndServer.Add(int x, int y)
        {
            return x + y;
        }        

        static LspClientLogger lspClientLogger = null;
        static ComClientLogger comClientLogger = null;

        ITargetServer IHidemaruLspBackEndServer.CreateTargetServer()
        {
            return new TargetServer();
        }

        /// <summary>
        /// コンストラクタ
        /// (Memo)アウトプロセスサーバなので createobject するたびに呼ばれる
        /// </summary>        
        bool IHidemaruLspBackEndServer.Initialize(string logFileName)
        {
            if (lspClientLogger == null)
            {
                lspClientLogger = new LspClientLogger(logFileName);
            }

            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                Holder.Initialized(lspClientLogger);
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
            if (comClientLogger == null)
            {
                Debug.Assert(lspClientLogger != null);
                comClientLogger = new ComClientLogger(lspClientLogger);
            }
            return comClientLogger;
        }

        /*あとで作る
        Dictionary<int,Holder>WorkerHolder_=new Dictionary<int,Holder>();
        */

        /// <summary>
        /// LSPサーバを起動する
        /// </summary>
        /// <param name="serverConfigFilename"></param>
        /// <param name="currentSourceCodeDirectory"></param>
        /// <returns></returns>
        bool IHidemaruLspBackEndServer.Start(
            ITargetServer TargetServer,
            string ExcutablePath,
            string Arguments,
            string WorkspaceConfig,
            string currentSourceCodeDirectory)
        {

          /*  {
                var hash = TargetServer.GetHashCode();
                if (WorkerHolder_.ContainsKey(hash))
                {
                    return WorkerHolder_[hash];
                }
                var h = new Holder();
                var ret = h.Start(ExcutablePath,
                                    Arguments,
                                    WorkspaceConfig,
                                    currentSourceCodeDirectory);
                if (ret == false)
                {
                    return null;
                }
                WorkerHolder_[hash] = h;
                return h;

            }*/

            var logger = LogManager.GetCurrentClassLogger();

            logger.Trace("Start");
            try
            {
                var ret = Holder.Start(
                    TargetServer,
                    ExcutablePath,
                    Arguments,
                    WorkspaceConfig,
                    currentSourceCodeDirectory);
                logger.Trace("Result={0}", ret);
                return ret;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return false;
        }
        void IHidemaruLspBackEndServer.DigOpen(ITargetServer TargetServer, string filename, string text, int contentsVersion)
        {
            Holder.DigOpen(filename, text, contentsVersion);
        }
        void IHidemaruLspBackEndServer.DigChange(ITargetServer TargetServer, string filename, string text, int contentsVersion)
        {
            Holder.DigChange(filename, text,contentsVersion);
        }
        /// <summary>
        /// 補完を行う
        /// </summary>
        /// <param name="absFilename"></param>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns>成功時＝辞書ファル名、失敗時=空文字</returns>
        string IHidemaruLspBackEndServer.Completion(ITargetServer TargetServer, string absFilename, long line, long column)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Trace("Completion");
            try
            {
                if (line < 0)
                {
                    return "";
                }
                if (column < 0)
                {
                    return "";
                }
                var fileName = Holder.Completion(absFilename, (uint)line, (uint)column);
                return fileName;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return "";
        }

        /// <summary>
        /// 秀丸マクロ終了時に呼び出されるメソッド
        /// </summary>
        /// <param name="reason"></param>
        void IHidemaruLspBackEndServer.Finalizer(ITargetServer TargetServer, int reason)
        {
            /* reason
             *  1　releaseobjectで解放
             *  3　プロセス終了時
             *  4　マクロ終了時(keepobject #obj,0;のとき)
             */
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                logger.Trace("Finalizer (n={0})", reason);
                Holder.Destroy();
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        
    }
}
