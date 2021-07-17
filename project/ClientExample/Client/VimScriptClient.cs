using LSP.Model;
using System;
using System.IO;
using System.Threading;


namespace ClientExample
{
    class VimScriptClient : ExampleBase
	{
        static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\vim\");        
        internal override Uri rootUri => new Uri(rootPath);
        internal override Uri sourceUri => new Uri(rootUri, @"test1.vim");
        internal override string languageId => "vim";
        internal override CompilationPosition compilationPosition => new CompilationPosition { line = 9, character = 6 };
        internal override object serverInitializationOptions { get { return CreateInitializationOptions(); } }
        internal override LSP.Client.StdioClient CreateClient()
        {
            string logFilename = Environment.ExpandEnvironmentVariables(@"D:\temp\LSP-Server\lsp_server_response_vim.txt");

            //OK
            var FileName = @"cmd";
            var vimLanguageServerCmd = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\vim-language-server\vim-language-server.cmd");
            var Arguments = string.Format("/c\"{0}\" --stdio", vimLanguageServerCmd);
            var WorkingDirectory = rootUri.AbsolutePath;

            var client = new LSP.Client.StdioClient();
            client.StartLspProcess(
                new LSP.Client.StdioClient.LspParameter { 
                    exeFileName = FileName, 
                    exeArguments = Arguments, 
                    exeWorkingDirectory = WorkingDirectory, 
                    logger = new Logger(logFilename) 
                }
            );
            return client;
        }
        static string CreateInitializationOptions()
		{
            /*各パラメータは以下ファイルを参照すること。
            %HOMEDRIVE%%HOMEPATH%\.vim\bundle\vim-lsp-settings\settings\vim-language-server.vim
            */
            var vimruntime = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\Desktop\program\vim82-kaoriya-win64\vim82");
            var isNeovim = 0;
            var runtimePaths=Directory.GetDirectories(Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\.vim\bundle"));
            var runtimepath = String.Join(",",runtimePaths);

            var initializationOptions=
@"{{
      ""vimruntime"":""{0}"",
      ""iskeyword"":""@,48-57,_,128-167,224-235,#,:"",
      ""diagnostic"":{{
            ""enable"":true
      }},
      ""runtimepath"":""{1}"",
      ""isNeovim"":{2}
  }}";

            return string.Format(initializationOptions,vimruntime,runtimepath, isNeovim);
        }
        internal override void DigOpen(LSP.Client.StdioClient client)
        {
            base.DigOpen(client);
            //Todo: サーバが準備できるまで正攻法でまつ

            //サーバが準備できるまでちょっと待つ
            int time = 1000;
            Console.WriteLine("Waiting {0}ms.", time);
            Thread.Sleep(time);
        }
    }
}
