﻿using LSP.Model;
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
		static int sourceVersion = 0;
		static bool useMicrosoftPythonLanguageServer = false;

		public static void Start()
		{
#if false
			//OK
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_pyls.txt";
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\pyls-all\venv\Scripts\pyls.exe");
			var Arguments = @"-v --log-file D:\temp\LSP-Server\pyls.log";
			var WorkingDirectory = @"";
#elif true
			//OK
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_Microsoft_Python.txt";
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\.vscode\extensions\ms-python.python-2021.5.842923320\languageServer.0.5.59\Microsoft.Python.LanguageServer.exe");
			var Arguments = @"";
			var WorkingDirectory = @"";
			useMicrosoftPythonLanguageServer = true;
#elif false
			//OK
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_pyls-ms.txt";
			var FileName  = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\pyls-ms\dotnet.exe");
			var Arguments = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\pyls-ms\Microsoft.Python.LanguageServer.dll");
			var WorkingDirectory = @"";
			useMicrosoftPythonLanguageServer = true;
#elif false
			//OK
			//.NET Runtime version 3.1.15
			//Microsoft.Python.LanguageServer version Latest 0.2
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_pyls-ms.txt";
			var FileName = @"D:\temp\LSP-Server\python-language-server-0.2\output\bin\Release\dotnet.exe";
			var Arguments = @"D:\temp\LSP-Server\python-language-server-0.2\output\bin\Release\Microsoft.Python.LanguageServer.dll";
			var WorkingDirectory = @"";
			useMicrosoftPythonLanguageServer = true;
#elif false
			//OK
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_jedi_language_server.txt";
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\jedi-language-server\venv\Scripts\jedi-language-server.exe");
			var Arguments = @"";
			var WorkingDirectory = Path.GetDirectoryName(sourceUri.AbsolutePath);
#endif

			var client = new Client();
			client.StartLspProcess(FileName, Arguments, WorkingDirectory, logFilename);

			Console.WriteLine("==== InitializeServer ====");
			InitializeServer(client);
			while (client.Status != Client.Mode.ServerInitializeFinish)
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
		static void InitializeServer(Client client)
		{
			var param = Util.Initialzie();
			param.rootUri = rootUri.AbsoluteUri;
			param.rootPath = rootUri.AbsolutePath;
			param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "Test-Root" } };

			if (useMicrosoftPythonLanguageServer)
			{
				/*	Memo: Use only "Microsoft.Python.LanguageServer.";
				 * 
				 * */
				var pythonOptions = new Microsoft.Python.LanguageServer.Protocol.PythonInitializationOptions();
				pythonOptions.interpreter = new Microsoft.Python.LanguageServer.Protocol.PythonInitializationOptions.Interpreter();
				pythonOptions.interpreter.properties = new Microsoft.Python.LanguageServer.Protocol.PythonInitializationOptions.Interpreter.InterpreterProperties();
				pythonOptions.interpreter.properties.InterpreterPath = @"C:\Python39\python.exe";
				/*Memo: 本来ならば"3.9.2"を指定するべきだが、Serverが例外(Unsupported Python version)を返すため"3.8.0"を指定した。
				 * 
				 * 以下enumへ3.9の定義を追加することで対応できると思われる。
				 * python-language-server-0.2\src\Parsing\Impl\PythonLanguageVersion.cs
				 * enum PythonLanguageVersion
				 */
				pythonOptions.interpreter.properties.Version = "3.8.0";

				param.initializationOptions = pythonOptions;
			}
			client.SendInitialize(param);
		}
		static void InitializedClient(Client client)
		{
			var param = new InitializedParams();
			client.SendInitialized(param);
		}
		static void DigOpen(Client client)
		{
			sourceVersion = 1;//Openしたのでとりあえず1にする。

			var param = new DidOpenTextDocumentParams();
			param.textDocument.uri = sourceUri.AbsoluteUri;
			param.textDocument.version = sourceVersion;
			param.textDocument.text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8); 
			param.textDocument.languageId = "python";
			client.SendTextDocumentDigOpen(param);
		}
		static void Completion(Client client)
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
			param.position.line = 1;
			param.position.character = 10;
			client.SendTextDocumentCompletion(param);
		}
	}
}
