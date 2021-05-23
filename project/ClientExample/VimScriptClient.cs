using LSP.Client;
using LSP.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            var client = StartCSharpClient();

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

            Console.WriteLine("==== Completion ====");
            Completion(client);

            Console.WriteLine("続行するには何かキーを押してください．．．");
            Console.ReadKey();
        }

        public static Client StartCSharpClient()
        {
            //OK
            var FileName = @"cmd";
            var vimLanguageServerCmd = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\vim-language-server\vim-language-server.cmd");
            var Arguments = string.Format("/c\"{0}\" --stdio", vimLanguageServerCmd);
            var WorkingDirectory = rootUri.AbsolutePath;

            var client = new Client();
            client.StartLspProcess(FileName, Arguments, WorkingDirectory, logFilename);
            return client;
        }

        static void InitializeServer(Client client)
        {
            var param = Util.Initialzie();
            param.rootUri = rootUri.AbsoluteUri;
            param.rootPath = rootUri.AbsolutePath;
            param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "test1-root-folder" } };
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
            param.textDocument.languageId = "vim";
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
            param.context.triggerCharacter = ":";
#endif
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.position.line = 9;
            param.position.character = 6;
            client.SendTextDocumentCompletion(param);
        }
    }
}
