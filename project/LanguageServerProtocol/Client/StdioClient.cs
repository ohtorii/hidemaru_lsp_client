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

		class Response{
			public object Item { get; set; } = null;
			public bool Exist { get; set; } = false;
        }
		Dictionary<RequestId, Response> response_ = new Dictionary<RequestId, Response>();

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
			mediator_ = new Mediator(	source_.Token);
			{
                var Protocol = mediator_.Protocol;
				Protocol.OnWindowLogMessage				+= this.OnWindowLogMessage;
				Protocol.OnWindowShowMessage            += this.OnWindowShowMessage;
                Protocol.OnWorkspaceConfiguration       += this.OnWorkspaceConfiguration;
				Protocol.OnWorkspaceSemanticTokensRefresh += this.OnWorkspaceSemanticTokensRefresh;
				Protocol.OnClientRegisterCapability     += this.OnClientRegisterCapability;
                Protocol.OnWindowWorkDoneProgressCreate += this.OnWindowWorkDoneProgressCreate;
                Protocol.OnProgress                     += this.OnProgress;
				Protocol.OnTextDocumentPublishDiagnostics += this.OnTextDocumentPublishDiagnostics;
			}
			server_ = new ServerProcess(param.exeFileName, param.exeArguments, param.exeWorkingDirectory);			
			server_.StartProcess();
			server_.standardErrorReceived += Client_standardErrorReceived;
			server_.standardOutputReceived += Client_standardOutputReceived;
			server_.Exited += Server_Exited;
			server_.StartRedirect();
			server_.StartThreadLoop();
		}
#region LSP_Event
		/*void OnResponseError(ResponseMessage response)
		{
			Console.WriteLine(string.Format("[OnResponseError] id={0}/error={1}", response.id, response.error));
			const string indent = "  ";
			Console.WriteLine(indent + string.Format("code={0}",response.error.code));
			Console.WriteLine(indent + string.Format("data={0}", response.error.@data));
			Console.WriteLine(indent + string.Format("message={0}", response.error.message));
		}*/
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
		void OnWorkspaceSemanticTokensRefresh()
        {
			//Todo: workspace/semanticTokens/refresh
			Console.WriteLine("Todo:workspace/semanticTokens/refresh");
		}
		void OnClientRegisterCapability(int id, RegistrationParams param)
		{
			//Todo: client/registerCapability
			Console.WriteLine("Todo: client/registerCapability");
		}
		void OnWindowWorkDoneProgressCreate(int id, WorkDoneProgressCreateParams param)
		{
			//Todo: window/workDoneProgress/create
			Console.WriteLine("Todo: window/workDoneProgress/create");
		}
		void OnProgress(ProgressParams param)
		{
			//Todo: $/progress
			Console.WriteLine("Todo: $/progress");
		}
		void OnTextDocumentPublishDiagnostics(PublishDiagnosticsParams param)
		{
			//Todo: ‘textDocument/publishDiagnostics'
			Console.WriteLine("Todo: textDocument/publishDiagnostics");
		}
		#endregion

		#region Process_Event
		private void Client_standardOutputReceived(object sender, byte[] e)
		{
			if(e.Length==0)
			{
				return;
			}
			{
				var jsonDataUnicode = Encoding.UTF8.GetString(e);
				param_.logger.Info("<--- " + jsonDataUnicode);
			}
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
		void ActionInitialize(ResponseMessage response)
		{
			var arg = (JToken)response.result;
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
		public RequestId SendTextDocumentCompletion(ICompletionParams param)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			return SendRequest(param, "textDocument/completion", ActionTextDocumentCompletion);
		}		
		public void ActionTextDocumentCompletion(ResponseMessage response)
		{
            if (response.error != null)
            {
				//エラーあり
				return;
            }

			CompletionList f(JToken arg) {
				if (arg == null)
				{
					Console.WriteLine("Completion==null");
					return null;
				}
				if (arg is JArray)
				{
					//CompletionItem[]
					var items = arg.ToObject<CompletionItem[]>();
					Console.WriteLine("Completion. num={0}", items.Length);
					return new CompletionList { items=items};
				}
				var obj = arg.ToObject<JObject>();
				if (obj.ContainsKey("isIncomplete"))
				{
					//CompletionList
					var list = obj.ToObject<CompletionList>();
					Console.WriteLine("Completion. num={0}", list.items.Length);
					return list;
				}
				Console.WriteLine("Completion. Not found.");
				return null;
			}
			
			var id = new RequestId(response.id);
			var value = new Response { 
								Item = f((JToken)response.result), 
								Exist = true 
							};
            lock (response_)
            {
				Debug.Assert(response_.ContainsKey(id));
				response_[id] = value;
			}
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

		#region  リクエストの返信
		/// <summary>
		/// リクエストの返信を取得する
		/// </summary>
		/// <param name="id"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public object QueryResponse(RequestId id, int millisecondsTimeout = -1)
        {
			//Todo: ポーリングをやめる(System.Collections.ObjectModel.KeyedCollection<TKey,TItem> クラスを利用できると思う)
			var sw = new Stopwatch();
			sw.Start();
			while (true) {
				try
				{
					lock (response_)
					{
						var item = response_[id];
						if ((item != null) && item.Exist)
						{
							response_.Remove(id);
							return item.Item;
						}
					}
				}
				catch (KeyNotFoundException)
				{
					//pass
				}

				if(millisecondsTimeout == -1)
                {
                    //pass
                }
                else
                {
					int t = Math.Max(0, millisecondsTimeout);
                    if (t < sw.ElapsedMilliseconds)
                    {
						break;
                    }
				}

				Thread.Sleep(0);
			}
			return null;
        }
		#endregion

		#region 低レイヤー
		//
		//低レイヤー
		//
		public RequestId SendRequest(object param, string method, Action<ResponseMessage> callback)
		{
			var id = requestIdGenerator_.NextId();
			mediator_.StoreResponse(id, callback);
			SendRequest(param,method,id);
			return id;
		}
		/// <summary>
		/// リクエスト送信
		/// </summary>
		/// <param name="param"></param>
		/// <param name="method"></param>
		/// <param name="id"></param>
		/// <param name="nullValueHandling"></param>
		/// <returns></returns>
		public RequestId SendRequest(object param, string method, RequestId id, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
		{
			byte[] payload;
			{
				var request = new RequestMessage
				{
					id      = id.intId,
					method  = method,
					@params = param
				};
				var jsonRpc = JsonConvert.SerializeObject(
									request,
									new JsonSerializerSettings
									{
										Formatting			= Formatting.None,
										NullValueHandling	= nullValueHandling
									}
								);
				payload = CreatePayLoad(jsonRpc);
			}
			PrepareToReceiveResponse(id);
			WriteStandardInput(payload);
			return id;
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
			WriteStandardInput(payload);
		}
		public void SendResponse(object param, int id, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
		{
			var response = new ResponseMessage { id=id, result=param};
			var jsonRpc = JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = nullValueHandling });
			var payload = CreatePayLoad(jsonRpc);
			WriteStandardInput(payload);
		}
		
		/// <summary>
		/// デバッグ用途のメソッド
		/// </summary>
		/// <param name="jsonRpc"></param>
		/// <param name="id"></param>
		/// <param name="callback"></param>
		public void SendRaw(string jsonRpc, RequestId id, Action<ResponseMessage> callback)
		{
			mediator_.StoreResponse(id, callback);
			var payload = CreatePayLoad(jsonRpc);
			WriteStandardInput(payload);
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
		void WriteStandardInput(byte[] payload)
		{

			{
				var jsonDataUnicode = Encoding.UTF8.GetString(payload);
				param_.logger.Info("---> " + jsonDataUnicode);
			}

			server_.WriteStandardInput(payload);
		}
		/// <summary>
		/// レスポンスを受け取る用意をする
		/// </summary>
		/// <param name="id"></param>
		void PrepareToReceiveResponse(RequestId id)
        {
			lock (response_)
			{
				Debug.Assert(response_.ContainsKey(id)==false);
				response_[id] = new Response();
			}
		}
#endregion
	}	
}
