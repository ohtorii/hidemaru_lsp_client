using LSP.Client;
using LSP.Model;
using System;
using System.IO;
using System.Threading;

namespace ClientExample
{
    internal class PythonClient : ExampleBase
    {
        private string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\python\");
        private bool useMicrosoftPythonLanguageServer = false;

        internal override Uri rootUri => new Uri(rootPath);

        internal override Uri sourceUri => new Uri(rootUri, @"completion.py");

        internal override string languageId => "python";

        internal override CompilationPosition compilationPosition => new CompilationPosition { line = 1, character = 10 };

        internal override LSP.Client.StdioClient CreateClient()
        {
#if false
			//Memo:問題なく動作する
			var logFilename = @"D:\temp\LSP-Server\lsp_server_response_pyls.txt";
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\pyls-all\venv\Scripts\pyls.exe");
			var Arguments = @"-v --log-file D:\temp\LSP-Server\pyls.log";
			var WorkingDirectory = @"";
#elif false
			//Memo:問題なく動作する
			var logFilename = @"D:\temp\LSP-Server\lsp_server_response_Microsoft_Python.txt";
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\.vscode\extensions\ms-python.python-2021.5.842923320\languageServer.0.5.59\Microsoft.Python.LanguageServer.exe");
			var Arguments = @"";
			var WorkingDirectory = @"";
			useMicrosoftPythonLanguageServer = true;
#elif false
			//Memo:問題なく動作する
			var logFilename = @"D:\temp\LSP-Server\lsp_server_response_pyls-ms.txt";
			var FileName  = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\pyls-ms\dotnet.exe");
			var Arguments = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\pyls-ms\Microsoft.Python.LanguageServer.dll");
			var WorkingDirectory = @"";
			useMicrosoftPythonLanguageServer = true;
#elif false
			//Memo:問題なく動作する
			//.NET Runtime version 3.1.15
			//Microsoft.Python.LanguageServer version Latest 0.2
			var logFilename = @"D:\temp\LSP-Server\lsp_server_response_pyls-ms.txt";
			var FileName = @"D:\ReferenceSourceCodes\LSP-Server\Servers\python-language-server-0.2\output\bin\Release\dotnet.exe";
			var Arguments = @"D:\ReferenceSourceCodes\LSP-Server\Servers\python-language-server-0.2\output\bin\Release\Microsoft.Python.LanguageServer.dll";
			var WorkingDirectory = @"";
			useMicrosoftPythonLanguageServer = true;
#elif true
            //Memo:問題なく動作する
            var logFilename = @"D:\temp\LSP-Server\lsp_server_response_jedi_language_server.txt";
            var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\jedi-language-server\venv\Scripts\jedi-language-server.exe");
            var Arguments = @"";
            var WorkingDirectory = Path.GetDirectoryName(sourceUri.AbsolutePath);
#endif

            var client = new LSP.Client.StdioClient();
            client.StartLspProcess(
                new LSP.Client.StdioClient.LspParameter
                {
                    exeFileName = FileName,
                    exeArguments = Arguments,
                    exeWorkingDirectory = WorkingDirectory,
                    logger = new Logger(logFilename)
                }
            );
            return client;
        }

        internal override RequestId InitializeServer(LSP.Client.StdioClient client)
        {
            var param = UtilInitializeParams.Initialzie();
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
            return client.Send.Initialize(param);
        }

        internal override void DigOpen(LSP.Client.StdioClient client)
        {
            base.DigOpen(client);
            //Todo: サーバが準備できるまで正攻法でまつ

            //サーバが準備できるまでちょっと待つ
            int time = 6000;
            Console.WriteLine("Waiting {0}ms.", time);
            Thread.Sleep(time);
        }
    }
}