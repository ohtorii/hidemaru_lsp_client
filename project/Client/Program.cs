using LSP.Model;
using System;
using System.IO;
using System.Threading;


namespace LSP.Client
{

    class Program
	{
        //static RequestIdGenerator requestIdGenerator = new RequestIdGenerator();
        static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\example\");
        static Uri rootUri = new Uri(rootPath);
        static Uri sourceUri= new Uri(rootUri, "test.cs");

        static void Main(string[] args)
        {
            

            //var FileName = @"d:\Temp\LSP-Server\omnisharp-win-x64-1.37.5\OmniSharp.exe";//OmniSharp.exeが例外はいて動かない
            var FileName = @"d:\Temp\LSP-Server\omnisharp-win-x64-1.37.8\OmniSharp.exe";
            //var FileName = @"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\omnisharp-lsp\OmniSharp.exe";
            var Arguments = "-lsp -v";
            var client = new Client();
            client.StartLspProcess(FileName, Arguments);
            InitializeServer(client);
			while (client.Status != Client.Mode.ServerInitializeFinish)
			{
                Thread.Sleep(100);
			}
            InitializedClient(client);
            Thread.Sleep(1000);
            OpenTextDocument(client);
            Thread.Sleep(1000);
            Completion(client);
            Thread.Sleep(1000);
            Console.WriteLine("続行するには何かキーを押してください．．．");
            Console.ReadKey();
        }
        
        
        static void InitializeServer(Client client)
		{
#if true
            var param = new InitializeParams ();
            param.rootUri = rootUri.AbsoluteUri;
            param.initializationOptions = null;
            param.processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            param.capabilities = new ClientCapabilities();
            param.clientInfo = new ClientInfo_();
            {
                param.capabilities.workspace = new ClientCapabilities._workspace();
                var workspace = param.capabilities.workspace;
                workspace.configuration = true;
                workspace.applyEdit = true;
            }
			{
                param.capabilities.window = new ClientCapabilities._window();
                var window = param.capabilities.window;
                window.workDoneProgress = false;
			}
            {
                param.capabilities.textDocument = new TextDocumentClientCapabilities();
                var textDocument = param.capabilities.textDocument;
				{
                    //textDocument.se

                }
                textDocument.completion = new CompletionClientCapabilities();
                var completion = textDocument.completion;
                completion.completionItem.documentationFormat = new[] { MarkupKind.Markdown, MarkupKind.PlainText };
                completion.completionItemKind.valueSet = new[] {
                        CompletionItemKind.Text         ,
                        CompletionItemKind.Method       ,
                        CompletionItemKind.Function     ,
                        CompletionItemKind.Constructor  ,
                        CompletionItemKind.Field        ,
                        CompletionItemKind.Variable     ,
                        CompletionItemKind.Class        ,
                        CompletionItemKind.Interface    ,
                        CompletionItemKind.Module       ,
                        CompletionItemKind.Property     ,
                        CompletionItemKind.Unit         ,
                        CompletionItemKind.Value        ,
                        CompletionItemKind.Enum         ,
                        CompletionItemKind.Keyword      ,
                        CompletionItemKind.Snippet      ,
                        CompletionItemKind.Color        ,
                        CompletionItemKind.File         ,
                        CompletionItemKind.Reference    ,
                        CompletionItemKind.Folder       ,
                        CompletionItemKind.EnumMember   ,
                        CompletionItemKind.Constant     ,
                        CompletionItemKind.Struct       ,
                        CompletionItemKind.Event        ,
                        CompletionItemKind.Operator     ,
                        CompletionItemKind.TypeParameter,
                    };
            }
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
        static void OpenTextDocument(Client client)
		{
            var param = new DidOpenTextDocumentParams();
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.textDocument.version = 1;
            param.textDocument.text = FileLoad(sourceUri.AbsolutePath);
            param.textDocument.languageId = "cs";
            client.SendDigOpenTextDocument(param);
        }
        static string FileLoad(string filename)
		{
            var text = File.ReadAllText(filename, System.Text.Encoding.UTF8);
            return text.Replace("\n", "\\n").Replace("\r", "").Replace("\t", "\\t");
        }
        static void Completion(Client client)
		{
            var param = new CompletionParams();
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.position.line=12;
            param.position.character=27;
            client.SendTextDocumentCompletion(param);
		}
    }    
}
