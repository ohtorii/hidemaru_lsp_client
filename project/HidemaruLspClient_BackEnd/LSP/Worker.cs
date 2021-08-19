using LSP.Implementation;
using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;
using HidemaruLspClient_BackEndContract;
using System.Runtime.CompilerServices;

namespace HidemaruLspClient
{
	sealed class Worker: HidemaruLspClient_BackEndContract.IWorker
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
		/// <summary>
		/// 通信のタイムアウト値(単位は㎳)
		/// (Memo)6000では短いため現在は12000で運用中
		/// </summary>
		const int defaultTimeout                 = 12000;
		static bool				initialized_     = false;
		static LspClientLogger	lspLogger_       = null;


		internal LspKey key { get; }
		LanguageClient				client_		 = null;
		Option					options_	     = null;
		//List<string>			tempFilename     = new List<string>();
		
		InitializeResult		initializeResult_= null;
		HidemaruLspClient_BackEnd.LSP.ServerCapabilities serverCapabilities_ = null;
		/// <summary>
		/// completionで返した一時ファイル名
		/// </summary>
		TempFileOne prevCompletionTempFile_      =new TempFileOne();

		/// <summary>
		/// 秀丸エディタのProcessId
		/// </summary>
		long hidemaruProcessId_;		

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
				string WorkspaceConfig,
				long   HidemaruProcessId)
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
			
			hidemaruProcessId_ = HidemaruProcessId;

			if (!InitializeClient())
			{
				return false;
			}
			
			var reqId=InitializeServer();
			var response = client_.QueryResponse(reqId, millisecondsTimeout: defaultTimeout);
			if ((response == null) || (response.item == null) || (response.error != null))
            {
				return false;
			}
			initializeResult_ = (InitializeResult)response.item;
			/*{//debug
				bool v = false;
				//var p = result.capabilities.declarationProvider;
				var p = result.capabilities.definitionProvider;
				if (p == null) {
					v = false;
                }else { 
					if (p.IsBool)
					{
						v = p.Bool;
					}
					else
					{
						v = true;
					}
				}
				//Console.WriteLine(string.Format("declarationProvider={0}",v));
				Console.WriteLine(string.Format("definitionProvider={0}", v));
			}*/
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
			client_ = new LSP.Implementation.LanguageClient();
			client_.Start(
				new LSP.Implementation.LanguageClient.LspParameter
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
			var error = client_.QueryResponse(requestId).item as ResponseError;
			if (error != null)
			{
				if (lspLogger_.IsErrorEnabled) {
					lspLogger_.Error(string.Format("Shutdown. code={0}/message={1}",error.code,error.message));
				}
				return;
			}
			client_.Send.Exit();
		}

		#region 実装

		sealed class PositionImpl : HidemaruLspClient_BackEndContract.IPosition
		{
			public PositionImpl(LSP.Model.IPosition pos)
			{
				pos_ = pos;
			}			
			long HidemaruLspClient_BackEndContract.IPosition.character => pos_.character;
			long HidemaruLspClient_BackEndContract.IPosition.line => pos_.line;			
			readonly LSP.Model.IPosition pos_;
		}
		sealed class RangeImpl : HidemaruLspClient_BackEndContract.IRange
		{
			public RangeImpl(LSP.Model.IPosition start, LSP.Model.IPosition end)
			{
				start_ = new PositionImpl(start);
				end_ = new PositionImpl(end);
			}		
			HidemaruLspClient_BackEndContract.IPosition HidemaruLspClient_BackEndContract.IRange.start => start_;
			HidemaruLspClient_BackEndContract.IPosition HidemaruLspClient_BackEndContract.IRange.end => end_;
			readonly PositionImpl start_;
			readonly PositionImpl end_;
		}
		


		#region DidOpen
		[LogMethod]
		/// <summary>
		/// 
		/// </summary>
		/// <param name="absFilename"></param>
		/// <returns></returns>
		void IWorker.DidOpen(string absFilename, string text, int contentsVersion)
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
		static string FileNameToLanguageId(string filename)
		{
			//(Ex) filename="c:/foo/bar.cpp"
			var extension = Path.GetExtension(filename);
			if (extension == null)
			{
				return "";
			}
			if (extension.Length == 0)
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
        #endregion
        [LogMethod]
		void IWorker.DidChange(string absFilename, string text, int contentsVersion)
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
		void IWorker.DidClose(string absFilename)
        {
			var sourceUri = new Uri(absFilename);
			var param = new DidCloseTextDocumentParams();
			param.textDocument.uri = sourceUri.AbsoluteUri;
			client_.Send.TextDocumentDidClose(param);
		}
        
		#region Completion
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
		static string GetCompletionWord(CompletionItem item)
		{
			if ((item.textEdit != null) && item.textEdit.IsTextEdit && (item.textEdit.TextEdit.newText != ""))
			{
				if (item.insertTextFormat == InsertTextFormat.PlainText)
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
			else
			{
				return item.label;
			}
		}
		static string ParseSnippet(string input)
		{
			//Todo: 実装する
			return input;
		}
		#endregion

		#region Diagnostics		

		//[LogMethod]		
		IPublishDiagnosticsParamsContainer IWorker.PullDiagnosticsParams()
		{
			var diagnostics = client_.PullTextDocumentPublishDiagnostics();
			PublishDiagnosticsParamsImpl[] dst = new PublishDiagnosticsParamsImpl[diagnostics.Length];
			
			int i = 0;
			foreach(var item in diagnostics)
            {
				dst[i] = new PublishDiagnosticsParamsImpl(item);
				++i;
            }
			return new PublishDiagnosticsParamsContainerImpl(dst);
		}

		sealed class PublishDiagnosticsParamsContainerImpl: IPublishDiagnosticsParamsContainer
        {
			readonly PublishDiagnosticsParamsImpl[] diagnosticsParams_;
			public PublishDiagnosticsParamsContainerImpl(PublishDiagnosticsParamsImpl[] diagnosticsParams)
            {
				diagnosticsParams_ = diagnosticsParams;
			}
			#region implement
			long IPublishDiagnosticsParamsContainer.Length => diagnosticsParams_.Length;

            IPublishDiagnosticsParams IPublishDiagnosticsParamsContainer.Item(long index)
            {
				return diagnosticsParams_[index];
			}
			#endregion
		}
		sealed class PublishDiagnosticsParamsImpl : HidemaruLspClient_BackEndContract.IPublishDiagnosticsParams
		{
			readonly PublishDiagnosticsParams	param_;
			readonly DiagnosticImpl[]			diagnostics_;
			public PublishDiagnosticsParamsImpl(PublishDiagnosticsParams param)
            {
				param_ = param;

				if ((param_ == null) || (param_.diagnostics == null))
				{
					diagnostics_ = new DiagnosticImpl[0];
					return;
				}

				diagnostics_ = new DiagnosticImpl[param_.diagnostics.Length];
				int i = 0;
				foreach (var item in param_.diagnostics)
				{
					diagnostics_[i] = new DiagnosticImpl(item);
					++i;
				}
			}			
            #region implement
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
			
            long IPublishDiagnosticsParams.Length
			{
				get
				{
					return diagnostics_.Length;
				}
			}

            IDiagnostic IPublishDiagnosticsParams.Item(long index)
            {
				return diagnostics_[index];
			}
            #endregion
        }
        sealed class DiagnosticImpl : HidemaruLspClient_BackEndContract.IDiagnostic
		{
			readonly LSP.Model.Diagnostic diagnostic_;
			readonly RangeImpl range_;
			public  DiagnosticImpl(LSP.Model.Diagnostic diagnostic) {
				diagnostic_ = diagnostic;
				range_ = new RangeImpl(diagnostic.range.start, diagnostic.range.end);
			}
			#region implement
			HidemaruLspClient_BackEndContract.IRange HidemaruLspClient_BackEndContract.IDiagnostic.range => range_;

            public HidemaruLspClient_BackEndContract.DiagnosticSeverity severity => (HidemaruLspClient_BackEndContract.DiagnosticSeverity)diagnostic_.severity;

            public string code
            {
                get
                {
                    if (diagnostic_.code == null)
                    {
						return "";
                    }
                    return diagnostic_.code.ToString();
                }
            }

            public string source
            {
                get
                {
                    if (diagnostic_.source == null)
                    {
						return "";
                    }
                    return diagnostic_.source;
                }
            }

            public string message
            {
                get
                {
                    if (diagnostic_.message == null)
                    {
						return "";
                    }
					return diagnostic_.message;
                }
            }
            #endregion
        }


		#endregion

		#region Location
		class LocationImpl : HidemaruLspClient_BackEndContract.ILocation
		{
			public LocationImpl(LSP.Model.ILocation location)
			{
				location_ = location;
			}
			public string uri
			{
				get
				{
					if (location_ == null)
					{
						return "";
					}
					return location_.uri;
				}
			}
			public HidemaruLspClient_BackEndContract.IRange range
			{
				get
				{
					if (location_ == null)
					{
						return null;
					}
					if (location_.range == null)
					{
						return null;
					}
					return new RangeImpl(location_.range.start, location_.range.end);
				}
			}

			readonly LSP.Model.ILocation location_;
		}
		class LocationContainerImpl : HidemaruLspClient_BackEndContract.ILocationContainer
		{
			public LocationContainerImpl(Location[] location)
			{
				location_ = location;
			}
			public HidemaruLspClient_BackEndContract.ILocation Item(long index)
			{
				if (location_ == null)
				{
					return null;
				}
				return new LocationImpl(location_[index]);

			}

			public long Length
			{
				get
				{
					if (location_ == null)
					{
						return 0;
					}
					return location_.LongLength;
				}
			}

			readonly Location[] location_;

		}
		#endregion
		ILocationContainer CommonProcessingOfGoto(string absFilename, long line, long column, ITextDocumentPositionParams param, Func<ITextDocumentPositionParams, RequestId> func)
		{
			var empty = new LocationContainerImpl(null);
			if ((line < 0) || (column < 0))
			{
				return empty;
			}
			Location[] result;
			{
				var sourceUri = new Uri(absFilename);
				param.position.line = (uint)line;
				param.position.character = (uint)column;
				param.textDocument.uri = sourceUri.AbsoluteUri;
				var id = func(param);
				var response = client_.QueryResponse(id, millisecondsTimeout: defaultTimeout);
				if ((response == null) || (response.error != null))
				{
					return empty;
				}
				result = response.item as Location[];
			}
			return new LocationContainerImpl(result);
		}
		[LogMethod]
		ILocationContainer IWorker.Declaration(string absFilename, long line, long column)
		{
			var param = new DeclarationParams();
			return CommonProcessingOfGoto(absFilename, line, column, param, (ITextDocumentPositionParams arg) => client_.Send.TextDocumentDeclaration((DeclarationParams)arg));			
		}						
		ILocationContainer IWorker.Definition(string absFilename, long line, long column)
        {
			var param = new DefinitionParams();
            return CommonProcessingOfGoto(absFilename, line, column, param, (ITextDocumentPositionParams arg) => client_.Send.TextDocumentDefinition((DefinitionParams)arg));
		}
        ILocationContainer IWorker.TypeDefinition(string absFilename, long line, long column)
        {
			var param = new TypeDefinitionParams();
			return CommonProcessingOfGoto(absFilename, line, column, param, (ITextDocumentPositionParams arg) => client_.Send.TextDocumentTypeDefinition((TypeDefinitionParams)arg));
		}
        public ILocationContainer Implementation(string absFilename, long line, long column)
        {
			var param = new ImplementationParams();
			return CommonProcessingOfGoto(absFilename, line, column, param, (ITextDocumentPositionParams arg) => client_.Send.TextDocumentImplementation((ImplementationParams)arg));
		}

        public ILocationContainer References(string absFilename, long line, long column)
        {
			var param = new ReferencesParams();
			return CommonProcessingOfGoto(absFilename, line, column, param, (ITextDocumentPositionParams arg) => client_.Send.TextDocumentReferences((ReferencesParams)arg));
		}

        public IServerCapabilities ServerCapabilities
		{
            get
            {
				if (serverCapabilities_ == null)
				{
					serverCapabilities_ = new HidemaruLspClient_BackEnd.LSP.ServerCapabilities(initializeResult_);
				}
				return serverCapabilities_;
			}
        }
        #endregion
    }
}
