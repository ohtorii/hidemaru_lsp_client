using LSP.Client;
using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace HidemaruLspClient
{	
	class Worker: IWorker
	{
		public class Option
		{
			public string ServerName { get; set; }
			public string ExcutablePath { get; set; }
			public string Arguments { get; set; }
			public string RootUri { get; set; }
			public string WorkspaceConfig { get; set; }
		}
		class TempFileOne
        {
			public string tempFilename { get; set; } = null;
			public void Delete()
            {
                if (tempFilename != null)
                {
					File.Delete(tempFilename);
					tempFilename = null;
				}
            }
        }

		static bool				initialized_ = false;
		static LspClientLogger	lspLogger_   = null;


		public LspKey key { get; }
		StdioClient				client_		 = null;
		Option					options_	 = null;
		//List<string>			tempFilename = new List<string>();

		/// <summary>
		/// completionで返した一時ファイル名
		/// </summary>
		TempFileOne prevCompletionTempFile_ =new TempFileOne();
		
		public Worker(LspKey key)
        {
			this.key = key;
        }
		public static void Initialize(LspClientLogger l)
        {
            if (initialized_)
            {
				return;
            }
			lspLogger_ = l;
			TempFile.Initialize();
			initialized_ = true;
		}		
		
		[LogMethod]
		public void Stop()
        {
			prevCompletionTempFile_.Delete();
			//Todo: LSPサーバを止める
			//Todo: LSPサーバのプロセスを終了させる
		}

		[LogMethod]
		public bool Start(
				string ServerName,
				string ExcutablePath,
				string Arguments,
				string RootUri,
				string WorkspaceConfig)
		{			
			if (client_ != null)
			{//起動済み
				return true;
			}
			options_ = new Option
			{
				ServerName		= ServerName,
				ExcutablePath	= ExcutablePath,
				Arguments		= Arguments,
				RootUri			= RootUri,
                WorkspaceConfig = WorkspaceConfig,
			};

			if (!InitializeClient())
			{
				return false;
			}
			InitializeServer();
			while (client_.Status != LSP.Client.StdioClient.Mode.ServerInitializeFinish)
			{
				//Todo: timeoutを処理する
				Thread.Sleep(0);
			}
			InitializedClient();
			
			return true;
		}

		[LogMethod]
		bool InitializeClient()
        {			
			JObject WorkspaceConfiguration=null;
			if (options_.WorkspaceConfig!="") {
				WorkspaceConfiguration = (JObject)JsonConvert.DeserializeObject(options_.WorkspaceConfig);
			}
			client_ = new LSP.Client.StdioClient();
			client_.StartLspProcess(
				new LSP.Client.StdioClient.LspParameter
				{
                    logger						= lspLogger_,
                    exeFileName					= options_.ExcutablePath,
					exeArguments				= options_.Arguments,
					jsonWorkspaceConfiguration	= WorkspaceConfiguration,
				}
			);		
			return true;
		}

		[LogMethod]
		void InitializeServer()
		{			
			var param = UtilInitializeParams.Initialzie();
			var rootUri = new Uri(options_.RootUri);
			param.rootUri          = rootUri.AbsoluteUri;
			param.rootPath         = rootUri.AbsolutePath;
			param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "FooBarHoge" } };
			client_.SendInitialize(param);
		}

		[LogMethod]
		void InitializedClient()
		{
			var param = new InitializedParams();
			client_.SendInitialized(param);			
		}

		[LogMethod]
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public void DigOpen(string filename, string text, int contentsVersion)
		{            
			var languageId = FileNameToLanguageId(filename);
			var sourceUri = new Uri(filename);

			var param = new DidOpenTextDocumentParams();
			param.textDocument.uri			= sourceUri.AbsoluteUri;
			param.textDocument.version		= contentsVersion;
			param.textDocument.text			= text;
			param.textDocument.languageId	= languageId;			
			client_.SendTextDocumentDigOpen(param);
		}

		[LogMethod]
		public void DigChange(string filename, string text, int contentsVersion)
        {						
			var param = new DidChangeTextDocumentParams { 
							contentChanges = new[] { new TextDocumentContentChangeEvent { text = text } },
			};
			var sourceUri = new Uri(filename);
			param.textDocument.uri		= sourceUri.AbsoluteUri;
			param.textDocument.version	= contentsVersion;			
			client_.SendTextDocumentDidChange(param);
		}

		[LogMethod]
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <returns>辞書の一時ファイル名(絶対パス)</returns>
		public string Completion(string filename, long line, long column)
		{
			prevCompletionTempFile_.Delete();
            if ((line < 0) || (column < 0))
            {
				return "";
            }
            
			object result;
			{
				RequestId id;
				{
					var param = new CompletionParams();
					var sourceUri = new Uri(filename);
#if true
					param.context.triggerKind = CompletionTriggerKind.Invoked;
#else
					param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
					param.context.triggerCharacter = ;
#endif
					param.textDocument.uri		= sourceUri.AbsoluteUri;
					param.position.line			= (uint)line;
					param.position.character	= (uint)column;
					id = client_.SendTextDocumentCompletion(param);
				}

				var millisecondsTimeout = 2000;
				result = client_.QueryResponse(id, millisecondsTimeout);
				if (result == null)
				{
					var logger = LogManager.GetCurrentClassLogger();
					logger.Error("Completion timeout. millisecondsTimeout={0}", millisecondsTimeout);
					return "";
				}
			}

			var completionList = (CompletionList)result;
			if (completionList.items.Length == 0)
			{
				var logger = LogManager.GetCurrentClassLogger();
				logger.Info("completionList.items.Length == 0");
				return "";
			}

			var fs = TempFile.Create();
			prevCompletionTempFile_.tempFilename = fs.Name;
			using (var sw = new StreamWriter(fs))
			{
				foreach (var item in completionList.items)
				{
					sw.WriteLine(item.label);
				}
			}
			return fs.Name;
		}
		string FileNameToLanguageId(string filename)
        {
			//(Ex) filename="c:/foo/bar.cpp"
			var extension = Path.GetExtension(filename);
            if (extension == null)
            {
				return "";
            }
            if (extension.Length==0)
            {
				return "";
            }
            if (extension.Length == 1)
            {
				return extension;
			}
			//(Ex) ".cpp" -> "cpp"
			return extension.Substring(1);
		}
	}
}
