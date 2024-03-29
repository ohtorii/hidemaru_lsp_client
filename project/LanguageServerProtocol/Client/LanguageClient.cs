﻿using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace LSP.Client
{
    class LanguageClient
    {
        ServerProcess server_;
        Mediator mediator_;
        CancellationTokenSource source_ = new CancellationTokenSource();

        public enum Mode
        {
            Init,
            ServerInitializeStart,
            ServerInitializeFinish,
            ClientInitializeFinish,
            ServerShutdownStart,
            ServerShutdownFinish,
            ServerExit,
        }
        public Mode Status { get; private set; }
        Mode Status_Get() => Status;
        void Status_Set(Mode m) => Status = m;

        public class LspParameter
        {
            public ILogger logger;
            /// <summary>
            /// 実行ファイル名
            /// </summary>
            public string exeFileName;
            /// <summary>
            /// 実行ファイルの引数
            /// </summary>
            public string exeArguments;
            /// <summary>
            /// 実行ファイルのワーキングディレクトリ
            /// </summary>
            public string exeWorkingDirectory;
            /// <summary>
            /// サーバからの問い合わせ（method: ‘workspace/configuration’）に対する応答。
            /// (memo)vim-lsp-settingのLspRegisterServer.workspace_configに対応する。
            /// (See)https://github.com/mattn/vim-lsp-settings/blob/master/settings/sumneko-lua-language-server.vim
            /// </summary>
            public JObject jsonWorkspaceConfiguration;
        }
        LspParameter param_;
        ClientEvents clientEvents_;

        /// <summary>
        /// 関連付けられているプロセスが終了したかどうかを示す値を取得します。
        /// </summary>
        public bool HasExited { get { return hasExited_; } }
        bool hasExited_ = false;

        /// <summary>
        /// 関連付けられたプロセスが終了したときにプロセスによって指定された値を取得します。
        /// </summary>
        public int ExitCode { get { return exitCode_; } }
        int exitCode_ = 0;

        public Sender Send { get; private set; }

        public LanguageClient()
        {
            Status = Mode.Init;
        }
        public void Start(LspParameter param)
        {
            Debug.Assert(Status == Mode.Init);
            if (param.logger == null)
            {
                param.logger = new NullLogger();
            }
            param_ = param;
            clientEvents_ = new ClientEvents(param_, this.EventResponseProxy);
            mediator_ = new Mediator(source_.Token, param_.logger);

            {
                var Protocol = mediator_.Protocol;
                Protocol.OnWindowLogMessage += this.clientEvents_.OnWindowLogMessage;
                Protocol.OnWindowShowMessage += this.clientEvents_.OnWindowShowMessage;
                Protocol.OnWorkspaceConfiguration += this.clientEvents_.OnWorkspaceConfiguration;
                Protocol.OnWorkspaceSemanticTokensRefresh += this.clientEvents_.OnWorkspaceSemanticTokensRefresh;
                Protocol.OnClientRegisterCapability += this.clientEvents_.OnClientRegisterCapability;
                Protocol.OnWindowWorkDoneProgressCreate += this.clientEvents_.OnWindowWorkDoneProgressCreate;
                Protocol.OnProgress += this.clientEvents_.OnProgress;
                Protocol.OnTextDocumentPublishDiagnostics += this.clientEvents_.OnTextDocumentPublishDiagnostics;
            }
            server_ = new ServerProcess(param.exeFileName, param.exeArguments, param.exeWorkingDirectory);
            Send = new Sender(param, mediator_.StoreResponseCallback, Status_Get, Status_Set, server_.WriteStandardInput);

            server_.standardErrorReceived += Client_standardErrorReceived;
            server_.standardOutputReceived += Client_standardOutputReceived;
            server_.Exited += Server_Exited;
            server_.StartProcess();
        }
        #region LSP_Server
        void Client_standardOutputReceived(object sender, byte[] e)
        {
            if (e.Length == 0)
            {
                return;
            }
            if (param_.logger.IsDebugEnabled)
            {
                var jsonDataUnicode = Encoding.UTF8.GetString(e);
                param_.logger.Debug("<--- " + jsonDataUnicode.Replace("\n", "\\n").Replace("\r", "\\r"));
            }
            mediator_.StoreBuffer(e);
        }
        void Client_standardErrorReceived(object sender, byte[] e)
        {
            if (param_.logger.IsErrorEnabled)
            {
                var unicodeString = Encoding.UTF8.GetString(e.ToArray());
                param_.logger.Error(string.Format("[StandardError]{0}", unicodeString));
            }
        }
        void Server_Exited(object sender, EventArgs e)
        {
            hasExited_ = true;
            exitCode_ = server_.GetExitCode();
            source_.Cancel();

            const int success = 0;
            var message = $"Server Exited. exitcode={exitCode_}";
            if (exitCode_ == success)
            {
                param_.logger.Info(message);
            }
            else
            {
                param_.logger.Error(message);
            }
        }
        #endregion
        void EventResponseProxy(int request_id, JArray any)
        {
            Debug.Assert(Status == Mode.ClientInitializeFinish);
            Send.Response(any, request_id, NullValueHandling.Include);
        }

        public Sender.ResponseResult QueryResponse(RequestId id, int millisecondsTimeout = -1)
        {
            try
            {
                return Send.QueryResponse(id, millisecondsTimeout);
            }
            catch (Exception e)
            {
                if (param_.logger.IsDebugEnabled)
                {
                    param_.logger.Error(e.ToString());
                }
            }
            return null;
        }
        public PublishDiagnosticsParams[] PullTextDocumentPublishDiagnostics()
        {
            try
            {
                return clientEvents_.PullTextDocumentPublishDiagnostics();
            }
            catch (Exception e)
            {
                if (param_.logger.IsDebugEnabled)
                {
                    param_.logger.Error(e.ToString());
                }
            }
            return new PublishDiagnosticsParams[0];
        }
    }
}
