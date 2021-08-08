using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace LSP.Client
{
	class StdioClient
	{
		ServerProcess server_;
		Mediator mediator_;
		CancellationTokenSource source_=new CancellationTokenSource();

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

        public Sender Send { get; private set; }

        public StdioClient()
        {
			Status = Mode.Init;
		}
		public void StartLspProcess(LspParameter param)
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
				Protocol.OnWindowLogMessage				  += this.clientEvents_.OnWindowLogMessage;
				Protocol.OnWindowShowMessage              += this.clientEvents_.OnWindowShowMessage;
                Protocol.OnWorkspaceConfiguration         += this.clientEvents_.OnWorkspaceConfiguration;
				Protocol.OnWorkspaceSemanticTokensRefresh += this.clientEvents_.OnWorkspaceSemanticTokensRefresh;
				Protocol.OnClientRegisterCapability       += this.clientEvents_.OnClientRegisterCapability;
                Protocol.OnWindowWorkDoneProgressCreate   += this.clientEvents_.OnWindowWorkDoneProgressCreate;
                Protocol.OnProgress                       += this.clientEvents_.OnProgress;
				Protocol.OnTextDocumentPublishDiagnostics += this.clientEvents_.OnTextDocumentPublishDiagnostics;
			}
			server_ = new ServerProcess(param.exeFileName, param.exeArguments, param.exeWorkingDirectory);
			Send = new Sender(param, mediator_.StoreResponse, Status_Get, Status_Set, server_.WriteStandardInput);

			server_.StartProcess();
			server_.standardErrorReceived  += Client_standardErrorReceived;
			server_.standardOutputReceived += Client_standardOutputReceived;
			server_.Exited += Server_Exited;
			server_.StartRedirect();
			server_.StartThreadLoop();
		}
        #region LSP_Server
        void Client_standardOutputReceived(object sender, byte[] e)
		{
			if(e.Length==0)
			{
				return;
			}
			if(param_.logger.IsDebugEnabled)
			{
				var jsonDataUnicode = Encoding.UTF8.GetString(e);
				param_.logger.Debug("<--- " + jsonDataUnicode.Replace("\n", "\\n").Replace("\r", "\\r"));
			}
			mediator_.StoreBuffer(e);
		}
		void Client_standardErrorReceived(object sender, byte[] e)
		{
			if (param_.logger.IsDebugEnabled)
			{
				var unicodeString = Encoding.UTF8.GetString(e.ToArray());
				param_.logger.Debug(string.Format("[StandardError]{0}", unicodeString));
			}
		}
		void Server_Exited(object sender, EventArgs e)
		{			
			param_.logger.Debug("Server_Exited");			
			source_.Cancel();
		}
        #endregion
        void EventResponseProxy(int request_id, JArray any)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			Send.Response(any, request_id, NullValueHandling.Include);
		}
		public object QueryResponse(RequestId id, int millisecondsTimeout = -1)
        {
			return Send.QueryResponse(id, millisecondsTimeout);
        }
		public PublishDiagnosticsParams[] PullTextDocumentPublishDiagnostics()
        {
			return clientEvents_.PullTextDocumentPublishDiagnostics();
		}
	}
}
