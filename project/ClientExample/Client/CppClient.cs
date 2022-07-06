using System;

namespace ClientExample
{
    internal class CppClient : ExampleBase
    {
        private static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\cpp\");
        internal override Uri rootUri => new Uri(rootPath);
        internal override Uri sourceUri => new Uri(rootUri, @"ConsoleApplication\ConsoleApplication.cpp");
        internal override string languageId => "cpp";
        internal override CompilationPosition compilationPosition => new CompilationPosition { line = 9, character = 25 };

        internal override LSP.Client.LanguageClient CreateClient()
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