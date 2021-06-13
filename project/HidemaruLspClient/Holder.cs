using LSP.Client;
using LSP.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class Holder
    {
        static LSP.Client.StdioClient client_ = null;
		static Configuration.Option options_=null;
		static HashSet<string> openedFiles = new HashSet<string>();
		public static bool StartServer(string serverConfigFilename)
		{
            if (! StartClient(serverConfigFilename))
            {
				return false;
            }
			InitializeServer();
			while (client_.Status != LSP.Client.StdioClient.Mode.ServerInitializeFinish)
			{
				Thread.Sleep(10);
			}
			InitializedClient();

			return true;
		}

		static bool StartClient(string serverConfigFilename)
        {
			if (client_ != null)
			{
				return true;
			}
			options_ = Configuration.Eval(serverConfigFilename);
			if (options_ == null)
			{
				return false;
			}
			client_ = new LSP.Client.StdioClient();
			client_.StartLspProcess(
				new LSP.Client.StdioClient.LspParameter
				{
					exeFileName = options_.Excutable,
					exeArguments = options_.Arguments
				}
			);
			return true;
		}
		static void InitializeServer()
		{
			var param = UtilInitializeParams.Initialzie();
			var rootUri = new Uri(options_.RootUri);
			param.rootUri = rootUri.AbsoluteUri;
			param.rootPath = rootUri.AbsolutePath;
			param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "VisualStudio-Solution" } };
			client_.SendInitialize(param);
		}

		static void InitializedClient()
		{
			var param = new InitializedParams();
			client_.SendInitialized(param);
		}
        public static bool DigOpen(string filename)
		{
            if (openedFiles.Contains(filename))
            {
				return true;
            }
			var languageId = FileNameToLanguageId(filename);
			var sourceVersion = 1;									//Todo: Openしたのでとりあえず1にする。（後で修正）
			var sourceUri = new Uri(filename);
			var param = new DidOpenTextDocumentParams();
			param.textDocument.uri = sourceUri.AbsoluteUri;
			param.textDocument.version = sourceVersion;
			param.textDocument.text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8);
			param.textDocument.languageId = languageId;
			client_.SendTextDocumentDigOpen(param);

			openedFiles.Add(filename);
			return true;
		}
		static public void Completion(string filename, uint line , uint column)
		{
			RequestId id;
			{
				var param = new CompletionParams();
				var sourceUri = new Uri(filename);
				param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
				param.context.triggerCharacter = ".";
				param.textDocument.uri = sourceUri.AbsoluteUri;
				param.position.line = line;
				param.position.character = column;
				id = client_.SendTextDocumentCompletion(param);
			}
			var millisecondsTimeout = 1000;
			var result = client_.QueryResponse(id,millisecondsTimeout);
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
