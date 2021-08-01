using LSP.Client;
using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HidemaruLspClient
{
	sealed class Worker: IWorker
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

		const int defaultTimeout = 6000;
		static bool				initialized_ = false;
		static LspClientLogger	lspLogger_   = null;


		internal LspKey key { get; }
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
			
			var reqId=InitializeServer();
			var response = client_.QueryResponse(reqId, millisecondsTimeout: defaultTimeout) as InitializeResult;
            if (response == null)
            {
				return false;
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
		RequestId InitializeServer()
		{			
			var param = UtilInitializeParams.Initialzie();
			var rootUri = new Uri(options_.RootUri);
			param.rootUri          = rootUri.AbsoluteUri;
			param.rootPath         = rootUri.AbsolutePath;
			param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "FooBarHoge" } };
			return client_.Send.Initialize(param);
		}

		[LogMethod]
		void InitializedClient()
		{
			var param = new InitializedParams();
			client_.Send.Initialized(param);			
		}

		[LogMethod]
		public void Stop()
		{
			prevCompletionTempFile_.Delete();
			Shutdown();
			client_.Send.LoggingResponseLeak();
		}
		[LogMethod]
		void Shutdown()
		{
			var requestId = client_.Send.Shutdown();
			var error = client_.QueryResponse(requestId) as ResponseError;
			if (error != null)
			{
				if (lspLogger_.IsErrorEnabled) {
					lspLogger_.Error(string.Format("Shutdown. code={0}/message={1}",error.code,error.message));
				}
				return;
			}
			client_.Send.Exit();
		}				
		static string GetCompletionWord(CompletionItem item)
        {
			if ((item.textEdit != null) && item.textEdit.IsTextEdit && (item.textEdit.TextEdit.newText != ""))
			{
				if(item.insertTextFormat == InsertTextFormat.PlainText)
                {
					return item.textEdit.TextEdit.newText;
				}
				return ParseSnippet(item.textEdit.TextEdit.newText);	
			}
            else if ((item.insertText != null) && (item.insertText != ""))
            {
				if (item.insertTextFormat == InsertTextFormat.PlainText)
                {
					return item.insertText;
				}
				return ParseSnippet(item.insertText);
            }
            else {
				return item.label;
			}						
        }
		static string ParseSnippet(string input)
        {
			//Todo: 実装する
			return input;
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

        #region 実装
        [LogMethod]
		/// <summary>
		/// 
		/// </summary>
		/// <param name="absFilename"></param>
		/// <returns></returns>
		void IWorker.DigOpen(string absFilename, string text, int contentsVersion)
		{            
			var languageId = FileNameToLanguageId(absFilename);
			var sourceUri = new Uri(absFilename);

			var param = new DidOpenTextDocumentParams();
			param.textDocument.uri			= sourceUri.AbsoluteUri;
			param.textDocument.version		= contentsVersion;
			param.textDocument.text			= text;
			param.textDocument.languageId	= languageId;			
			client_.Send.TextDocumentDidOpen(param);
		}

		[LogMethod]
		void IWorker.DigChange(string absFilename, string text, int contentsVersion)
        {						
			var param = new DidChangeTextDocumentParams { 
							contentChanges = new[] { new TextDocumentContentChangeEvent { text = text } },
			};
			var sourceUri = new Uri(absFilename);
			param.textDocument.uri		= sourceUri.AbsoluteUri;
			param.textDocument.version	= contentsVersion;			
			client_.Send.TextDocumentDidChange(param);
		}

		[LogMethod]
		/// <summary>
		/// 
		/// </summary>
		/// <param name="absFilename"></param>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <returns>辞書の一時ファイル名(絶対パス)</returns>
		string IWorker.Completion(string absFilename, long line, long column)
		{
			prevCompletionTempFile_.Delete();
            if ((line < 0) || (column < 0))
            {
				return "";
            }

			CompletionList completionList;			
			{
				object result;
				RequestId id;
				{
					var param = new CompletionParams();
					var sourceUri = new Uri(absFilename);
#if true
					param.context.triggerKind = CompletionTriggerKind.Invoked;
#else
					param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
					param.context.triggerCharacter = ;
#endif
					param.textDocument.uri		= sourceUri.AbsoluteUri;
					param.position.line			= (uint)line;
					param.position.character	= (uint)column;
					id = client_.Send.TextDocumentCompletion(param);
				}

				result = client_.QueryResponse(id, millisecondsTimeout: defaultTimeout);
				if (result == null)
				{
					return "";
				}
				completionList = result as CompletionList;
			}			
			if (completionList.items.Length == 0)
			{
				return "";
			}

			var fs = TempFile.Create();
			prevCompletionTempFile_.tempFilename = fs.Name;
			using (var sw = new StreamWriter(fs))
			{
				foreach (var item in completionList.items)
				{
					sw.WriteLine(GetCompletionWord(item));
				}
			}
			return fs.Name;
		}
		#region Diagnostics
		[LogMethod]
		IPublishDiagnosticsParams IWorker.Diagnostics(string absFilename)
		{			
			var notification=client_.PullTextDocumentPublishDiagnostics(new Uri(absFilename).AbsoluteUri);
			test_= new PublishDiagnosticsParamsImpl(notification);
			return test_;
		}
		PublishDiagnosticsParamsImpl test_;
		sealed class PublishDiagnosticsParamsImpl : IPublishDiagnosticsParams
        {
			PublishDiagnosticsParams param_;
			DiagnosticImpl[] diagnostics_;
			public PublishDiagnosticsParamsImpl(PublishDiagnosticsParams param)
            {
				param_ = param;
			}
            string IPublishDiagnosticsParams.uri
			{
                get
                {
                    if (param_ == null)
                    {
						return "";
                    }
					return param_.uri;
				}
			}
            long IPublishDiagnosticsParams.version
			{
                get
                {
                    if (param_ == null)
                    {
						return 0;
                    }
					return param_.version;
				} 
			}
			void initializeDiagnostics()
            {
				if (diagnostics_ != null)
				{
					return;
				}
				if ((param_ == null) || (param_.diagnostics == null))
				{
					diagnostics_ = new DiagnosticImpl[0];
					return;
				}
				
				diagnostics_ = new DiagnosticImpl[param_.diagnostics.Length];
				int i = 0;
				foreach (var item in param_.diagnostics)
				{
					diagnostics_[i].Initialize(item.range);
					++i;
				}
			}
			long IPublishDiagnosticsParams.getDiagnosticsLength()
			{
				initializeDiagnostics();
				return diagnostics_.Length;
			}
			IDiagnostic IPublishDiagnosticsParams.getDiagnostics(long index)
            {
				initializeDiagnostics();
				return diagnostics_[index];
            }
        }
		sealed class DiagnosticImpl : IDiagnostic
        {
			LSP.Model.IRange param_;
			public void Initialize(LSP.Model.IRange range) {
				param_ = range;
			}
            IRange IDiagnostic.range => throw new NotImplementedException();
        }
		
        sealed class RangeImpl : IRange
        {
            IPosition IRange.start => throw new NotImplementedException();

            IPosition IRange.end => throw new NotImplementedException();
        }
        sealed class PositionImpl : IPosition
        {
            public long character => throw new NotImplementedException();

            public long line => throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
