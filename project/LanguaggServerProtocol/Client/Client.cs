﻿using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LSP.Client
{
	class Client
	{
		ServerProcess server;
		Handler handler;
		CancellationTokenSource source=new CancellationTokenSource();
		RequestIdGenerator requestIdGenerator=new RequestIdGenerator();

		public enum Mode
		{
			Init,
			ServerInitializeStart,
			ServerInitializeFinish,
			ClientInitializeFinish,
		}

		public Mode Status {  get; private set; }

		public Client()
		{
			Status = Mode.Init;
		}
		public void StartLspProcess(string exeFileName, string Arguments, string WorkingDirectory, string logFilename)
		{
			Debug.Assert(Status==Mode.Init);

			handler = new Handler(
							new Response.InitializeParameter {
									OnWindowLogMessage= this.OnWindowLogMessage, 
									OnWindowShowMessage=this.OnWindowShowMessage,
									OnResponseError=this.OnResponseError,
									logFileName = logFilename
							},
							source.Token);

			server = new ServerProcess(exeFileName, Arguments, WorkingDirectory);			
			server.StartProcess();
			server.standardErrorReceived += Client_standardErrorReceived;
			server.standardOutputReceived += Client_standardOutputReceived;
			server.Exited += Server_Exited;
			server.StartRedirect();
			server.StartThreadLoop();
		}
		
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
		private void Client_standardOutputReceived(object sender, byte[] e)
		{
			handler.StoreBuffer(e);
		}

		private void Client_standardErrorReceived(object sender, byte[] e)
		{
			var unicodeString = Encoding.UTF8.GetString(e.ToArray());
			Console.WriteLine(string.Format("[StandardError]", unicodeString));
		}
		private void Server_Exited(object sender, EventArgs e)
		{
			Console.WriteLine("[Server_Exited]");
			source.Cancel();
		}
		
		public void SendInitialize(IInitializeParams param)
		{
			Debug.Assert(Status == Mode.Init);
			SendRequest(param, "initialize", ResponseInitialize);			
			Status = Mode.ServerInitializeStart;
		}		
		//Memo: デバッグ用にpublicとしている
		public void ResponseInitialize(JToken arg)
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
		public void SendWorkspaceDidChangeConfiguration(IDidChangeConfigurationParams param)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			SendNotification(param, "workspace/didChangeConfiguration");
		}
		public void SendTextDocumentDidChange(IDidChangeTextDocumentParams param)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			SendNotification(param, "textDocument/didChange");
		}
		public void SendTextDocumentCompletion(ICompletionParams param)
		{
			Debug.Assert(Status == Mode.ClientInitializeFinish);
			SendRequest(param, "textDocument/completion", ResponseTextDocumentCompletion);
		}
		public void ResponseTextDocumentCompletion(JToken arg)
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
		//
		//低レイヤー
		//
		public void SendRequest(object param, string method, Action<JToken> callback)
		{
			var id = requestIdGenerator.NextId();
			handler.StoreCallback(id, callback);
			SendRequest(param,method,id);
		}
		public void SendRequest(object param, string method, int id)
		{
			var request = new RequestMessage { id = id, method = method, @params = param };
			var jsonRpc = JsonConvert.SerializeObject(request, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore });
			var payload = CreatePayLoad(jsonRpc);
			server.WriteStandardInput(payload);
		}
		/// <summary>
		/// 投げっぱなしのリクエスト
		/// </summary>
		/// <param name="jsonRpc"></param>
		public void SendNotification(object param, string method)
		{			
			var notification = new NotificationMessage {method = method, @params = param };
			var jsonRpc = JsonConvert.SerializeObject(notification, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore });
			var payload = CreatePayLoad(jsonRpc);
			server.WriteStandardInput(payload);
		}
		
		/// <summary>
		/// デバッグ用途のメソッド
		/// </summary>
		/// <param name="jsonRpc"></param>
		/// <param name="id"></param>
		/// <param name="callback"></param>
		public void SendRaw(string jsonRpc, int id, Action<JToken> callback)
		{
			handler.StoreCallback(id, callback);
			var payload = CreatePayLoad(jsonRpc);
			server.WriteStandardInput(payload);
		}
		static string CreatePayLoad(string jsonRpc)
		{
			return string.Format("Content-Length: {0}\r\n\r\n{1}", jsonRpc.Length, jsonRpc);
		}
	}

	class RequestIdGenerator
	{
		int id = 1;
		public int NextId()
		{
			var ret = id;
			id++;
			return ret;
		}
	}
}