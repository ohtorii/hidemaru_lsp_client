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
	public class Option
	{
		public string ExcutablePath { get; set; }
		public string Arguments { get; set; }
		public string RootUri { get; set; }
		public string WorkspaceConfig { get; set; }
	}

	class Holder
    {
		static bool initialized_ = false;
        static LSP.Client.StdioClient	client_		= null;
        static LspClientLogger			lspLogger_		= null;
		static NLog.Logger					logger;
		static Option		options_	=null;
		static List<string> tempFilename=new List<string>();
		class Document
		{
			public string Filename { get; set; } = "";
			public Uri Uri { get; set; } = null;
			public int ContentsVersion { get; set; } = 1;
			public int ContentsHash { get; set; } = 0;
        }
		static Document openedFiles = null;
		enum DigOpenStatus
        {
			/// <summary>
			/// ファイルを開いた
			/// </summary>
			Opened,
			/// <summary>
			/// ファイールはオープン済み
			/// </summary>
			AlreadyOpened,
        }
		enum DigChangeStatus
        {
			/// <summary>
			/// 変更あり
			/// </summary>
			Changed,
			/// <summary>
			/// 変更無し
			/// </summary>
			NoChanged,
        }
		public static void Initialized(LspClientLogger l)
        {
            if (initialized_)
            {
				return;
            }
			lspLogger_ = l;
			logger= LogManager.GetCurrentClassLogger();
			TempFile.Initialize();
			initialized_ = true;
		}
		public static void Destroy()
        {
			var logger = LogManager.GetCurrentClassLogger();
			foreach (var name in tempFilename)
            {
				try
				{
					File.Delete(name);
                }
                catch(Exception e)
                {
					logger.Error(e);
				}
            }
			tempFilename.Clear();
		}
		public static bool Start(string serverConfigFilename, string currentSourceCodeDirectory)
		{
			var temp = new EnterLeaveLogger("Start",logger);
			{
				if (client_ != null)
				{//起動済み
					return true;
				}
				if (!InitializeClient(serverConfigFilename, currentSourceCodeDirectory))
				{
					return false;
				}
				InitializeServer();
				while (client_.Status != LSP.Client.StdioClient.Mode.ServerInitializeFinish)
				{
					Thread.Sleep(0);
				}
				InitializedClient();
			}
			return true;
		}
		
		static bool InitializeClient(string serverConfigFilename, string currentSourceCodeDirectory)
        {
/* Todo: 後で実装
			var temp = new EnterLeaveLogger("InitializeClient", logger);
			{
				
				JObject WorkspaceConfiguration=null;
				if (options_.WorkspaceConfig!="") {
					WorkspaceConfiguration = (JObject)JsonConvert.DeserializeObject(options_.WorkspaceConfig);
				}
				client_ = new LSP.Client.StdioClient();
				client_.StartLspProcess(
					new LSP.Client.StdioClient.LspParameter
					{
						logger = lspLogger_,
						exeFileName = options_.ExcutablePath,
						exeArguments = options_.Arguments,
						jsonWorkspaceConfiguration= WorkspaceConfiguration,
					}
				);
			}
*/
			return true;
		}
		static void InitializeServer()
		{
			var temp = new EnterLeaveLogger("InitializeServer", logger);
			{
				var param = UtilInitializeParams.Initialzie();
				var rootUri = new Uri(options_.RootUri);
				param.rootUri          = rootUri.AbsoluteUri;
				param.rootPath         = rootUri.AbsolutePath;
				param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "FooBarHoge" } };
				client_.SendInitialize(param);
			}
		}

		static void InitializedClient()
		{
			var temp = new EnterLeaveLogger("InitializedClient", logger);
			{
				var param = new InitializedParams();
				client_.SendInitialized(param);
			}
		}
        /// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		static DigOpenStatus DigOpen(string filename)
		{
            if (openedFiles!=null)
            {
				Debug.Assert(openedFiles.Filename==filename);
				return DigOpenStatus.AlreadyOpened;
            }
			var languageId = FileNameToLanguageId(filename);
			var sourceVersion = 1;
			var sourceUri = new Uri(filename);

			var param = new DidOpenTextDocumentParams();
			param.textDocument.uri			= sourceUri.AbsoluteUri;
			param.textDocument.version		= sourceVersion;
			param.textDocument.text			= File.ReadAllText(filename,Encoding.UTF8);//Hidemaru.GetTotalTextUnicode();
			param.textDocument.languageId	= languageId;			
			client_.SendTextDocumentDigOpen(param);

			openedFiles = new Document { 
							Filename		= filename, 
							Uri				= sourceUri, 
							ContentsVersion	= sourceVersion , 
							ContentsHash	= param.textDocument.text .GetHashCode()
			};
			return DigOpenStatus.Opened;
		}
        static DigChangeStatus DigChange(string filename)
        {
			Debug.Assert(openedFiles.Filename==filename);

			var text = File.ReadAllText(filename, Encoding.UTF8); //Hidemaru.GetTotalTextUnicode();
			{
				var currentHash = text.GetHashCode();
				var prevHash    = openedFiles.ContentsHash;
				if (currentHash == prevHash)
				{
					return DigChangeStatus.NoChanged;
				}
			}
			++openedFiles.ContentsVersion;

			var param = new DidChangeTextDocumentParams { 
							contentChanges = new[] { new TextDocumentContentChangeEvent { text = text } },
			};
			param.textDocument.uri = openedFiles.Uri.AbsoluteUri;
			param.textDocument.version = openedFiles.ContentsVersion;			
			client_.SendTextDocumentDidChange(param);
			return DigChangeStatus.Changed;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <returns>辞書の一時ファイル名(絶対パス)</returns>
		static public string Completion(string filename, uint line, uint column)
		{
            if (DigOpen(filename) == DigOpenStatus.AlreadyOpened)
            {
				DigChange(filename);
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
					param.position.line			= line;
					param.position.character	= column;
					id = client_.SendTextDocumentCompletion(param);
				}

				var millisecondsTimeout = 2000;
				result = client_.QueryResponse(id, millisecondsTimeout);
				if (result == null)
				{
					logger.Error("Completion timeout. millisecondsTimeout={0}", millisecondsTimeout);
					return "";
				}
			}

			var completionList = (CompletionList)result;
			if (completionList.items.Length == 0)
			{
				logger.Info("completionList.items.Length == 0");
				return "";
			}
			var fs = TempFile.Create();
			using (var sw = new StreamWriter(fs))
			{
				foreach (var item in completionList.items)
				{
					sw.WriteLine(item.label);
				}
			}
			tempFilename.Add(fs.Name);
			return fs.Name;
		}

		static string FileNameToLanguageId(string filename)
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
