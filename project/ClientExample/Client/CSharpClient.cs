using LSP.Model;
using System;
using System.Threading;
using System.IO;
using LSP.Client;

namespace ClientExample
{
	class CSharpClient : ExampleBase
    {
        static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\csharp\");
        static string solutionFileName = Path.Combine(rootPath, "simple.sln");
        static string logFilename = Environment.ExpandEnvironmentVariables(@"D:\temp\LSP-Server\lsp_server_response_cs.txt");

        internal override Uri rootUri => new Uri(rootPath);

        internal override Uri sourceUri => new Uri(rootUri, @"simple\Program.cs");

        internal override CompilationPosition compilationPosition => new ExampleBase.CompilationPosition { line = 12, character = 27 };

        internal override string languageId => "csharp";

        internal override StdioClient CreateClient()
        {
            
#if false
            //OK
            var FileName = @"d:\Temp\LSP-Server\omnisharp-win-x64-1.37.8\OmniSharp.exe";
            var Arguments = string.Format(@"-lsp -v --source ""{0}"" --hostPID {1} --encoding utf-8", solutionFileName, System.Diagnostics.Process.GetCurrentProcess().Id);
            var WorkingDirectory = @"";
#elif true
            //OK
            var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\omnisharp-lsp\OmniSharp.exe");
            var Arguments = string.Format(@"-lsp -v --source ""{0}"" --hostPID {1} --encoding utf-8", solutionFileName, System.Diagnostics.Process.GetCurrentProcess().Id);
            var WorkingDirectory = @"";
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
    }
}
