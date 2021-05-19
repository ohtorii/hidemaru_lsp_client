using LSP.Model;
using LSP.Client;
using System;
using System.Threading;
using System.IO;

namespace ClientExample
{
	class PythonClient
	{
		static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\python\");
		static Uri rootUri = new Uri(rootPath);
		static Uri sourceUri = new Uri(rootUri, @"completion.py");
		static string logFilename = Environment.ExpandEnvironmentVariables(@"D:\temp\LSP-Server\lsp_server_response_py.txt");
		public static void Start()
		{
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\pyls-all\venv\Scripts\pyls.exe");
			var Arguments = @"-v --log-file D:\temp\LSP-Server\pyls.log";
			var client = new Client();
			client.StartLspProcess(FileName, Arguments, logFilename);

			Console.WriteLine("==== InitializeServer ====");
			InitializeServer(client);
			while (client.Status != Client.Mode.ServerInitializeFinish)
			{
				Thread.Sleep(100);
			}

			Console.WriteLine("==== InitializedClient ====");
			InitializedClient(client);
		}
		static void InitializeServer(Client client)
		{
			var param = Util.Initialzie();
			param.rootUri = rootUri.AbsoluteUri;
			param.rootPath = rootUri.AbsolutePath;
			param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "Test-Root" } };
			client.SendInitialize(param);
		}
		static void InitializedClient(Client client)
		{
			var param = new InitializedParams();
			client.SendInitialized(param);
		}
	}
}
