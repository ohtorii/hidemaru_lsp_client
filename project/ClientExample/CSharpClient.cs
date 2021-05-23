using LSP.Model;
using LSP.Client;
using System;
using System.Threading;
using System.IO;

namespace ClientExample
{
	class CSharpClient
	{
        static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\csharp\");
        static string solutionFileName = Path.Combine(rootPath, "simple.sln");
        static Uri rootUri = new Uri(rootPath);
        static Uri sourceUri = new Uri(rootUri, @"simple\Program.cs");
        static int sourceVersion = 0;
        static string logFilename = Environment.ExpandEnvironmentVariables(@"D:\temp\LSP-Server\lsp_server_response_cs.txt");

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
#if false
            Console.WriteLine("==== didChangeConfiguration ====");
            DidChangeConfiguration(client);
#endif
            Console.WriteLine("==== OpenTextDocument ====");
            DigOpen(client);

#if false
            Console.WriteLine("==== DidChangen ====");
            DidChange(client);
#endif
            Console.WriteLine("==== Completion ====");
            Completion(client);

            Console.WriteLine("続行するには何かキーを押してください．．．");
            Console.ReadKey();
        }

        public static Client StartCSharpClient()
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

            var client = new Client();
            client.StartLspProcess(FileName, Arguments, WorkingDirectory, logFilename);
            return client;
        }

        static void InitializeServer(Client client)
        {
#if true
            var param = Util.Initialzie();
            param.rootUri = rootUri.AbsoluteUri;
            param.rootPath = rootUri.AbsolutePath;
            param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "VisualStudio-Solution" } };
            client.SendInitialize(param);
#else
			var json_string = string.Format(@"{""id"":1,""jsonrpc"":""2.0"",""method"":""initialize"",""params"":{""rootUri"":""{0}"",""initializationOptions"":null,""capabilities"":{""workspace"":{""configuration"":true,""applyEdit"":true},""window"":{""workDoneProgress"":false},""textDocument"":{""semanticHighlightingCapabilities"":{""semanticHighlighting"":false},""codeAction"":{""disabledSupport"":true,""codeActionLiteralSupport"":{""codeActionKind"":{""valueSet"":["""",""quickfix"",""refactor"",""refactor.extract"",""refactor.inline"",""refactor.rewrite"",""source"",""source.organizeImports""]}},""dynamicRegistration"":false},""completion"":{""completionItem"":{""snippetSupport"":true,""resolveSupport"":{""properties"":[""additionalTextEdits""]},""documentationFormat"":[""markdown"",""plaintext""]},""dynamicRegistration"":false,""completionItemKind"":{""valueSet"":[10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,1,2,3,4,5,6,7,8,9]}},""formatting"":{""dynamicRegistration"":false},""codeLens"":{""dynamicRegistration"":false},""hover"":{""dynamicRegistration"":false,""contentFormat"":[""markdown"",""plaintext""]},""rangeFormatting"":{""dynamicRegistration"":false},""declaration"":{""dynamicRegistration"":false,""linkSupport"":true},""references"":{""dynamicRegistration"":false},""typeHierarchy"":false,""foldingRange"":{""rangeLimit"":5000,""dynamicRegistration"":false,""lineFoldingOnly"":true},""documentSymbol"":{""symbolKind"":{""valueSet"":[10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,1,2,3,4,5,6,7,8,9]},""dynamicRegistration"":false,""labelSupport"":false,""hierarchicalDocumentSymbolSupport"":false},""synchronization"":{""dynamicRegistration"":false,""willSaveWaitUntil"":false,""willSave"":false,""didSave"":true},""documentHighlight"":{""dynamicRegistration"":false},""implementation"":{""dynamicRegistration"":false,""linkSupport"":true},""typeDefinition"":{""dynamicRegistration"":false,""linkSupport"":true},""definition"":{""dynamicRegistration"":false,""linkSupport"":true}}},""clientInfo"":{""name"":""vim-lsp""},""trace"":""off""}}]",rootUri.AbsoluteUri);
			client.Send(json_string,1,client.ResponseInitialize);
#endif
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
            param.textDocument.languageId = "csharp";
            client.SendTextDocumentDigOpen(param);
        }
        
        static void DidChangeConfiguration(Client client)
        {
            var param = new DidChangeConfigurationParams();
            param.settings = new object();
            client.SendWorkspaceDidChangeConfiguration(param);
        }
        static void DidChange(Client client)
        {
            var text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8);
            ++sourceVersion;//ソースを更新したので+1する

            var param = new DidChangeTextDocumentParams();
            param.contentChanges = new[] { new TextDocumentContentChangeEvent { text = text } };
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.textDocument.version = sourceVersion;

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
            param.position.line = 12;
            param.position.character = 27;
            client.SendTextDocumentCompletion(param);
        }
    }
}
