using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using static LSP.Client.StdioClient;

namespace LSP.Client
{
    class Sender
    {
		RequestIdGenerator requestIdGenerator_ = new RequestIdGenerator();

		class ResponseObject
		{
			public object Item { get; set; } = null;
			public bool Exist { get; set; } = false;
		}
		Dictionary<RequestId, ResponseObject> response_ = new Dictionary<RequestId, ResponseObject>();
		
        Action<RequestId, Action<ResponseMessage>> storeResponse_;
		StdioClient.LspParameter param_;
		Func<StdioClient.Mode> GetStatus_;
		Action<StdioClient.Mode> SetStatus_;
		Action<byte[]> WriteStandardInput_;

		public Sender(StdioClient.LspParameter param,
                Action<RequestId, Action<ResponseMessage>> StoreResponse,
                Func<StdioClient.Mode> GetStatus,
                Action<StdioClient.Mode> SetStatus,
				Action<byte[]> WriteStandardInput)
        {
			storeResponse_      = StoreResponse;
			param_              = param;
			GetStatus_          = GetStatus;
			SetStatus_          = SetStatus;
			WriteStandardInput_ = WriteStandardInput;
		}
		public RequestId Initialize(IInitializeParams param)
		{
			Debug.Assert(GetStatus_() == Mode.Init);
			var id = Request(param, "initialize", ActionInitialize);
			SetStatus_(Mode.ServerInitializeStart);
			return id;
		}
		void ActionInitialize(ResponseMessage response)
		{
			var arg = (JToken)response.result;
			var id = new RequestId(response.id);
			var value = new ResponseObject
			{
				Item = arg.ToObject<InitializeResult>(),
				Exist = true
			};
			lock (response_)
			{
				Debug.Assert(response_.ContainsKey(id));

				//Memo: response_[]とStatusは一緒に設定する。
				response_[id] = value;
				SetStatus_(Mode.ServerInitializeFinish);
			}
		}
		public void Initialized(IInitializedParams param)
		{
			Debug.Assert(GetStatus_()== Mode.ServerInitializeFinish);
			//Memo: クライアントからサーバへの通知なので、サーバからクライアントへの返信は無い。			
			Notification(param, "initialized");
			SetStatus_(Mode.ClientInitializeFinish);
		}
		public RequestId Shutdown()
		{
			Debug.Assert(GetStatus_()==Mode.ClientInitializeFinish);
			SetStatus_(Mode.ServerShutdownStart);
			return Request(null, "shutdown", ActionShutdown);
		}
		void ActionShutdown(ResponseMessage response)
		{
			//Todo: ここでのエラー処理は、実際にエラーが発生してから考える。（今は仮実装）
			Debug.Assert(response.error == null);

			if (response.error == null)
			{
				SetStatus_(Mode.ServerShutdownFinish);
			}
			else
			{
				SetStatus_(Mode.ClientInitializeFinish);
			}

			var id = new RequestId(response.id);
			var value = new ResponseObject
			{
				Item = response.error,
				Exist = true
			};
			lock (response_)
			{
				Debug.Assert(response_.ContainsKey(id));
				response_[id] = value;
			}
		}
		public void LoggingResponseLeak()
		{
			if (param_.logger.IsErrorEnabled == false)
			{
				return;
			}
			lock (response_)
			{
				foreach (var item in response_)
				{
					var requestId = item.Key;
					param_.logger.Error(string.Format("requestId is leaked. requestId={0}", requestId));
				}
			}
		}
		public void Exit()
		{
			Debug.Assert(GetStatus_() == Mode.ServerShutdownFinish);
			Notification(null, "exit");
			SetStatus_(Mode.ServerExit);
		}
		public void TextDocumentDidOpen(IDidOpenTextDocumentParams param)
		{
			Debug.Assert(GetStatus_() == Mode.ClientInitializeFinish);
			Notification(param, "textDocument/didOpen");
		}
		public void TextDocumentDidChange(IDidChangeTextDocumentParams param)
		{
			Debug.Assert(GetStatus_() == Mode.ClientInitializeFinish);
			Notification(param, "textDocument/didChange");
		}
		public RequestId TextDocumentCompletion(ICompletionParams param)
		{
			Debug.Assert(GetStatus_() == Mode.ClientInitializeFinish);
			return Request(param, "textDocument/completion", ActionTextDocumentCompletion);
		}
		void ActionTextDocumentCompletion(ResponseMessage response)
		{
			if (response.error != null)
			{
				//エラーあり
				return;
			}

			CompletionList f(JToken arg)
			{
				if (arg == null)
				{
					return null;
				}
				if (arg is JArray)
				{
					//CompletionItem[]の場合
					var items = arg.ToObject<CompletionItem[]>();
					return new CompletionList { items = items };
				}
				var obj = arg.ToObject<JObject>();
				if (obj.ContainsKey("isIncomplete"))
				{
					//CompletionListの場合
					var list = obj.ToObject<CompletionList>();
					return list;
				}
				return null;
			}

			var id = new RequestId(response.id);
			var value = new ResponseObject
			{
				Item = f((JToken)response.result),
				Exist = true
			};
			lock (response_)
			{
				Debug.Assert(response_.ContainsKey(id));
				response_[id] = value;
			}
		}

		public void WorkspaceDidChangeConfiguration(IDidChangeConfigurationParams param)
		{
			Debug.Assert(GetStatus_() == Mode.ClientInitializeFinish);
			Notification(param, "workspace/didChangeConfiguration");
		}
		/// <summary>
		/// clientEvents_で発生したイベントを送信する
		/// </summary>
		/// <param name="request_id"></param>
		/// <param name="any"></param>
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
			while (true)
			{
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

				if (millisecondsTimeout == -1)
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
			if (param_.logger.IsInfoEnabled)
			{
				param_.logger.Info(string.Format("QueryResponse timeout. id={0}/millisecondsTimeout={1}", id, millisecondsTimeout));
			}
			return null;
		}
		#endregion

		#region 低レイヤー
		//
		//低レイヤー
		//
		RequestId Request(object param, string method, Action<ResponseMessage> callback)
		{
			var id = requestIdGenerator_.NextId();
			storeResponse_(id, callback);
			Request(param, method, id);
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
		RequestId Request(object param, string method, RequestId id, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
		{
			byte[] payload;
			{
				var request = new RequestMessage
				{
					id = id.intId,
					method = method,
					@params = param
				};
				var jsonRpc = JsonConvert.SerializeObject(
									request,
									new JsonSerializerSettings
									{
										Formatting = Formatting.None,
										NullValueHandling = nullValueHandling
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
		void Notification(object param, string method, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
		{//memo: 通知なので返信は無い。
			var notification = new NotificationMessage { method = method, @params = param };
			var jsonRpc = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = nullValueHandling });
			var payload = CreatePayLoad(jsonRpc);
			WriteStandardInput(payload);
		}
		public void Response(object param, int id, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
		{
			var response = new ResponseMessage { id = id, result = param };
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
		void SendRaw(string jsonRpc, RequestId id, Action<ResponseMessage> callback)
		{
			storeResponse_(id, callback);
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
			Array.Copy(utf8Header, 0, payload, 0, utf8Header.Length);

			/*(Memo) payload=Content-Length: 1234\r\n\r\n{"id":1,"jsonrpc":"2.0" ...
			 */
			Array.Copy(utf8Json, 0, payload, utf8Header.Length, utf8Json.Length);
			return payload;
		}
		void WriteStandardInput(byte[] payload)
		{
			if (param_.logger.IsDebugEnabled)
			{
				var jsonDataUnicode = Encoding.UTF8.GetString(payload);
				param_.logger.Debug("---> " + jsonDataUnicode.Replace("\n", "\\n").Replace("\r", "\\r"));
			}

			WriteStandardInput_(payload);
		}
		/// <summary>
		/// レスポンスを受け取る用意をする
		/// </summary>
		/// <param name="id"></param>
		void PrepareToReceiveResponse(RequestId id)
		{
			lock (response_)
			{
				Debug.Assert(response_.ContainsKey(id) == false);
				response_[id] = new ResponseObject();
			}
		}
		#endregion
	}

}
