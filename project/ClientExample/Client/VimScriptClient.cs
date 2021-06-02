using LSP.Model;
using System;
using System.IO;
using System.Threading;


namespace ClientExample
{
	class VimScriptClient
	{
        static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\vim\");
        static Uri rootUri = new Uri(rootPath);
        static Uri sourceUri = new Uri(rootUri, @"test1.vim");
        static int sourceVersion = 0;
        static string logFilename = Environment.ExpandEnvironmentVariables(@"D:\temp\LSP-Server\lsp_server_response_vim.txt");

        public static void Start()
        {
#if true
            //OK
            var FileName = @"cmd";
            var vimLanguageServerCmd = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\vim-language-server\vim-language-server.cmd");
            var Arguments = string.Format("/c\"{0}\" --stdio", vimLanguageServerCmd);
            var WorkingDirectory = rootUri.AbsolutePath;
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
            InitializeServer(client, CreateInitializationOptions());
            while (client.Status != LSP.Client.StdioClient.Mode.ServerInitializeFinish)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("==== InitializedClient ====");
            InitializedClient(client);

            Console.WriteLine("==== OpenTextDocument ====");
            DigOpen(client);
            //Memo: ウェイトを入れると補完候補(Completion)を取得できる。（何故だ・・・
            Thread.Sleep(1000);

            Console.WriteLine("==== Completion ====");
            Completion(client);

            Console.WriteLine("続行するには何かキーを押してください．．．");
            Console.ReadKey();
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

		
        static void InitializeServer(LSP.Client.StdioClient client,string jsonInitializationOptions)
        {
            var param = UtilInitializeParams.Initialzie();
            param.rootUri = rootUri.AbsoluteUri;
            param.rootPath = rootUri.AbsolutePath;
            param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "test1-root-folder" } };
            param.initializationOptions = jsonInitializationOptions;
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
            param.textDocument.languageId = "vim";
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
            param.context.triggerCharacter = ":";
#endif
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.position.line = 9;
            param.position.character = 6;
            client.SendTextDocumentCompletion(param);
        }
    }
}
