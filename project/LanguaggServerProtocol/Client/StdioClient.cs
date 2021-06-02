using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LSP.Utils;

namespace LSP.Client
{
	
	class StdioClient
	{
		ServerProcess server_;
		Mediator mediator_;
		CancellationTokenSource source_=new CancellationTokenSource();
		RequestIdGenerator requestIdGenerator_=new RequestIdGenerator();

		public enum Mode
		{
			Init,
			ServerInitializeStart,
			ServerInitializeFinish,
			ClientInitializeFinish,
		}

		public Mode Status {  get; private set; }
		
		[Serializable]
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
		public StdioClient()
		{
			Status = Mode.Init;
		}
		public void StartLspProcess(LspParameter param)
		{
			Debug.Assert(Status==Mode.Init);
			if (param.logger == null)
			{
				param.logger = new NullLogger();
			}
#if true
			param_ = param;
#else
			param_ = param.DeepClone();
#endif
			mediator_ = new Mediator(
						new Protocol.InitializeParameter {
							OnWindowLogMessage= this.OnWindowLogMessage, 
							OnWindowShowMessage=this.OnWindowShowMessage,
							OnResponseError=this.OnResponseError,
							OnWorkspaceConfiguration=this.OnWorkspaceConfiguration,
							OnClientRegisterCapability=this.OnClientRegisterCapability,
							logger = param.logger
						},
						source_.Token);

			server_ = new ServerProcess(param.exeFileName, param.exeArguments, param.exeWorkingDirectory);			
			server_.StartProcess();
			server_.standardErrorReceived += Client_standardErrorReceived;
			server_.standardOutputReceived += Client_standardOutputReceived;
			server_.Exited += Server_Exited;
			server_.StartRedirect();
			server_.StartThreadLoop();
		}
#region LSP_Event
		void OnResponseError(ResponseMessage response)
		{
			Console.WriteLine(string.Format("[OnResponseError] id={0}/error={1}", response.id, response.error));
			const string indent = "  ";
			Console.WriteLine(indent + string.Format("code={0}",response.error.code));
			Console.WriteLine(indent + string.Format("data={0}", response.error.@data));
			Console.WriteLine(indent + string.Format("message={0}", response.error.message));
		}
		void OnWindowLogMessage(LogMessageParams param)
		{
			Console.WriteLine(String.Format("[OnWindowLogMessage]{0}",param.message));
		}
		void OnWindowShowMessage(ShowMessageParams param) {
			Console.WriteLine(String.Format("[OnWindowShowMessage]{0}",param.message));
		}
		void OnWorkspaceConfiguration(int request_id, ConfigurationParams param)
		{
			var any = new JArray();
			foreach (var item in param.items)
			{
				try
				{
					var jsonValue = param_.jsonWorkspaceConfiguration[item.section];
					any.Add(jsonValue);
				}
				catch (Exception)
				{
					any.Add(null);
				}
			}
			
			ResponseWorkspaceConfiguration(request_id,any);
		}
		void OnClientRegisterCapability(int id, RegistrationParams param)
		{
			Console.WriteLine("client/registerCapabilityは未実装です");
		}
#endregion

		#region Process_Event
		private void Client_standardOutputReceived(object sender, byte[] e)
		{
			mediator_.StoreBuffer(e);
		}

		private void Client_standardErrorReceived(object sender, byte[] e)
		{
			var unicodeString = Encoding.UTF8.GetString(e.ToArray());
			Console.WriteLine(string.Format("[StandardError]{0}", unicodeString));
		}
		private void Server_Exited(object sender, EventArgs e)
		{
			Console.WriteLine("[Server_Exited]");
			source_.Cancel();
		}
#endregion

#region Send-Rpc
		public void SendInitialize(IInitializeParams param)
		{
			Debug.Assert(Status == Mode.Init);
			SendRequest(param, "initialize", ActionInitialize);			
			Status = Mode.ServerInitializeStart;
		}		
		//Memo: デバッグ用にpublicとしている
		void ActionInitialize(JToken arg)
		{
			var result = arg.ToObject<InitializeResult>();
			Status = Mode.ServerInitializeFinish;
		}
		public void SendInitialized(IInitializedParams param)
		{
			Debug.Assert(Status == Mode.ServerInitializeFinish);
			//Memo: クライアントからサーバへの通知なので、サーバからクライアントへの返信は無い。			
			SendNotification(param, "initialized");
			Status = Mode.ClientInitializeFinish;
		}
		public void SendTextDocumentDigOpen(IDidOpenTextDocumentParams param)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			SendNotification(param, "textDocument/didOpen");
		}
		public void SendTextDocumentDidChange(IDidChangeTextDocumentParams param)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			SendNotification(param, "textDocument/didChange");
		}
		public void SendTextDocumentCompletion(ICompletionParams param)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			SendRequest(param, "textDocument/completion", ActionTextDocumentCompletion);
		}		
		public void ActionTextDocumentCompletion(JToken arg)
		{
			if (arg == null)
			{
				Console.WriteLine("Completion==null");
				return;
			}
			if(arg is JArray)
			{
				//CompletionItem[]
				var items = arg.ToObject<CompletionItem[]>();
				Console.WriteLine("Completion. num={0}",items.Length);
				return;
			}
			var obj = arg.ToObject<JObject>();
			if(obj.ContainsKey("isIncomplete"))
			{
				//CompletionList
				var list = obj.ToObject<CompletionList>();
				Console.WriteLine("Completion. num={0}", list.items.Length);
				return;
			}
			Console.WriteLine("Completion. Not found.");
		}

		public void SendWorkspaceDidChangeConfiguration(IDidChangeConfigurationParams param)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			SendNotification(param, "workspace/didChangeConfiguration");
		}
		/// <summary>
		/// "workspace/configuration" に対する返信。
		/// </summary>
		/// <param name="request_id"></param>
		/// <param name="any"></param>
		void ResponseWorkspaceConfiguration(int request_id, JArray any)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			SendResponse(any, request_id, NullValueHandling.Include);
		}

#endregion

#region 低レイヤー
		//
		//低レイヤー
		//
		public void SendRequest(object param, string method, Action<JToken> callback)
		{
			var id = requestIdGenerator_.NextId();
			mediator_.StoreResponse(id, callback);
			SendRequest(param,method,id);
		}
		/// <summary>
		/// リクエスト送信
		/// </summary>
		/// <param name="param"></param>
		/// <param name="method"></param>
		/// <param name="id"></param>
		public void SendRequest(object param, string method, int id, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
		{
			var request = new RequestMessage { id = id, method = method, @params = param };
			var jsonRpc = JsonConvert.SerializeObject(request, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = nullValueHandling });
			var payload = CreatePayLoad(jsonRpc);
			server_.WriteStandardInput(payload);
		}
		/// <summary>
		/// 通知の送信
		/// </summary>
		/// <param name="jsonRpc"></param>
		public void SendNotification(object param, string method, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
		{//memo: 通知なので返信は無い。
			var notification = new NotificationMessage {method = method, @params = param };
			var jsonRpc = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = nullValueHandling });
			var payload = CreatePayLoad(jsonRpc);
			server_.WriteStandardInput(payload);
		}
		public void SendResponse(object param, int id, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
		{
			var response = new ResponseMessage { id=id, result=param};
			var jsonRpc = JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = nullValueHandling });
			var payload = CreatePayLoad(jsonRpc);
			server_.WriteStandardInput(payload);
		}
		
		/// <summary>
		/// デバッグ用途のメソッド
		/// </summary>
		/// <param name="jsonRpc"></param>
		/// <param name="id"></param>
		/// <param name="callback"></param>
		public void SendRaw(string jsonRpc, int id, Action<JToken> callback)
		{
			mediator_.StoreResponse(id, callback);
			var payload = CreatePayLoad(jsonRpc);
			server_.WriteStandardInput(payload);
		}
		static byte[] CreatePayLoad(string unicodeJson)
		{
			byte[] utf8Header;
			var utf8Json = Encoding.UTF8.GetBytes(unicodeJson);
			{
				var unicodeHeader = string.Format("Content-Length: {0}\r\n\r\n", utf8Json.Length);
				utf8Header = Encoding.UTF8.GetBytes(unicodeHeader);
			}
			var payload = new byte[utf8Header.Length + utf8Json.Length];

			/*(Memo) payload=Content-Length: 1234\r\n\r\n
			 */
			Array.Copy(utf8Header, 0, payload, 0,                 utf8Header.Length);

			/*(Memo) payload=Content-Length: 1234\r\n\r\n{"id":1,"jsonrpc":"2.0" ...
			 */
			Array.Copy(utf8Json,   0, payload, utf8Header.Length, utf8Json.Length);
			return payload;
		}
#endregion
	}	
}
