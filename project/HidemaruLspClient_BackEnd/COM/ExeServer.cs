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
    [ComVisible(true)]
    [Guid(LspContract.Constants.ServerClass)]
    [ProgId(LspContract.Constants.ProgId)]
    [ComDefaultInterface(typeof(IServer))]
    public sealed class ExeServer : IServer
    {
        public ExeServer()
        {
            COMRegistration.Ole32.CoAddRefServerProcess();
        }
        ~ExeServer()
        {
            if (COMRegistration.Ole32.CoReleaseServerProcess() == 0)
            {
                const int success = 0;
                Environment.Exit(success);
            }
        }
        int IServer.Add(int x, int y)
        {
            return x + y;
        }
        public bool Hoge(string x)
        {
            return true;
        }

        static LspClientLogger lspClientLogger = new LspClientLogger(Config.logFileName);
        /// <summary>
        /// コンストラクタ
        /// (Memo)アウトプロセスサーバなので createobject するたびに呼ばれる
        /// </summary>        
        bool IServer.Initialize()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                Holder.Initialized(lspClientLogger);
                //Hidemaru.Initialize();
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// LSPサーバを起動する
        /// </summary>
        /// <param name="serverConfigFilename"></param>
        /// <param name="currentSourceCodeDirectory"></param>
        /// <returns></returns>
        bool IServer.Start(string ExcutablePath,
                          string Arguments,
                          string RootUri,
                          string WorkspaceConfig,
                          string currentSourceCodeDirectory)
        {
            /*
            var logger = LogManager.GetCurrentClassLogger();

            logger.Trace("Start");
            try
            {
                var ret = Holder.Start(serverConfigFilename, currentSourceCodeDirectory);
                logger.Trace("Result={0}", ret);
                return ret;
            }
            catch (Exception e)
            {
                //pass
                logger.Error(e);
            }*/
            return false;
        }

        /// <summary>
        /// 補完を行う
        /// </summary>
        /// <param name="absFilename"></param>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns>成功時＝辞書ファル名、失敗時=空文字</returns>
        string IServer.Completion(string absFilename, long line, long column)
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
        void IServer.Finalizer(int reason)
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
