using LSP.Model;
using System;
using System.IO;
using System.Threading;

namespace ClientExample
{
	class CppClient
	{
		static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\cpp\");
		static Uri rootUri = new Uri(rootPath);
		static Uri sourceUri = new Uri(rootUri, @"ConsoleApplication\ConsoleApplication.cpp");
		static int sourceVersion = 0;
		public static void Start()
		{
#if false
			//OK
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_clangd.txt";
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\clangd\clangd.exe");
			var Arguments = @"";
			var WorkingDirectory = @"";
#elif true
			//OK
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_clangd.txt";
			var FileName = @"C:\Program Files\LLVM\bin\clangd.exe";
			var Arguments = @"";//@"--log=verbose";
			var WorkingDirectory = System.IO.Path.GetDirectoryName(sourceUri.AbsolutePath);
#elif false
			//NG
			/*
			 * cpptools.exeの子プロセスとしてcpptools-srv.exe が起動していないためLSPとして動作しないようだ。
			 */
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_cpptools.txt";
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\.vscode\extensions\ms-vscode.cpptools-1.3.1\bin\cpptools.exe");
			var Arguments = @"";
			var WorkingDirectory = rootPath;
#endif
			var client = new LSP.Client.StdioClient();
			client.StartLspProcess(
				new LSP.Client.StdioClient.LspParameter { 
					exeFileName = FileName, 
					exeArguments = Arguments, 
					exeWorkingDirectory = WorkingDirectory,
					logger = new Logger(logFilename)
				}
			);

			Console.WriteLine("==== InitializeServer ====");
			InitializeServer(client);
			while (client.Status != LSP.Client.StdioClient.Mode.ServerInitializeFinish)
			{
				Thread.Sleep(100);
			}

			Console.WriteLine("==== InitializedClient ====");
			InitializedClient(client);

			Console.WriteLine("==== OpenTextDocument ====");
			DigOpen(client);

			Thread.Sleep(6000);
			Console.WriteLine("==== Completion ====");
			Completion(client);

			Console.WriteLine("続行するには何かキーを押してください．．．");
			Console.ReadKey();
		}
		static void InitializeServer(LSP.Client.StdioClient client)
		{
			var param = UtilInitializeParams.Initialzie();
			param.rootUri = rootUri.AbsoluteUri;
			param.rootPath = rootUri.AbsolutePath;
			//param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "Test-Root" } };		
			client.SendInitialize(param);
		}
		static void InitializedClient(LSP.Client.StdioClient client)
		{
			var param = new InitializedParams();
			client.SendInitialized(param);
		}
		static void DigOpen(LSP.Client.StdioClient client)
		{
			sourceVersion = 1;//Openしたのでとりあえず1にする。

			var param = new DidOpenTextDocumentParams();
			param.textDocument.uri = sourceUri.AbsoluteUri;
			param.textDocument.version = sourceVersion;
			param.textDocument.text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8);
			param.textDocument.languageId = "cpp";
			client.SendTextDocumentDigOpen(param);
		}
		static void Completion(LSP.Client.StdioClient client)
		{
			var param = new CompletionParams();
#if false
            param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
            param.context.triggerCharacter = ".";
#else
			param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
			param.context.triggerCharacter = ".";
#endif
			param.textDocument.uri = sourceUri.AbsoluteUri;
			param.position.line = 9;
			param.position.character = 25;
			client.SendTextDocumentCompletion(param);
		}
	}
}
