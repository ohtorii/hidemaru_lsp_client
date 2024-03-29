﻿using LSP.Client;
using System;
using System.IO;

namespace ClientExample
{
    internal class CSharpClient : ExampleBase
    {
        private static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\csharp\");
        private static string solutionFileName = Path.Combine(rootPath, "simple.sln");
        private static string logFilename = Environment.ExpandEnvironmentVariables(@"D:\temp\LSP-Server\lsp_server_response_cs.txt");

        internal override Uri rootUri => new Uri(rootPath);

        //internal override Uri sourceUri => new Uri(rootUri, @"simple\Program.cs");
        internal override Uri sourceUri => new Uri(rootUri, @"simple\Program.cs");

        internal override CompilationPosition compilationPosition => new ExampleBase.CompilationPosition { line = 12, character = 27 };

        internal override string languageId => "csharp";

        internal override LanguageClient CreateClient()
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
#elif false
            //Todo 後で動作確認を行う(OmniSharp)
            Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\\.vscode\extensions\ms-dotnettools.csharp-1.23.14\.omnisharp\1.37.14\OmniSharp.exe");
#endif

            var client = new LSP.Client.LanguageClient();
            client.Start(
                new LSP.Client.LanguageClient.LspParameter
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