using LSP.Model;
using System;
using System.Threading;


namespace LSP.Client
{

    class Program
	{
        static RequestIdGenerator requestIdGenerator = new RequestIdGenerator();

        static void Main(string[] args)
        {
            var FileName = @"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\omnisharp-win-x64-1.37.5\OmniSharp.exe";
            var Arguments = "-lsp";
            var client = new Client();
            client.StartLspProcess(FileName, Arguments);
            InitializeServer(client);
			while (client.Status != Client.Mode.ServerInitializeFinish)
			{
                Thread.Sleep(100);
			}
            InitializedClient(client);

            Console.WriteLine("続行するには何かキーを押してください．．．");
            Console.ReadKey();
        }
        
        static void InitializeServer(Client client)
		{
#if true
            var data = new InitializeParams ();
            data.rootUri = "file:///C:/Users/ikeuc/GitHub/hidemaru_language_server_protocol/example";
            data.processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            {
                var completion = data.capabilities.textDocument.completion;
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
            client.SendInitialize(data);
#else
            //旧バージョン
			//var json_string = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"initialize\",\"params\":{\"rootUri\":\"file:///C:/Users/ikeuc/GitHub/hidemaru_language_server_protocol\",\"initializationOptions\":null,\"capabilities\":{\"workspace\":{\"configuration\":true,\"applyEdit\":true},\"window\":{\"workDoneProgress\":false},\"textDocument\":{\"semanticHighlightingCapabilities\":{\"semanticHighlighting\":false},\"codeAction\":{\"disabledSupport\":true,\"codeActionLiteralSupport\":{\"codeActionKind\":{\"valueSet\":[\"\",\"quickfix\",\"refactor\",\"refactor.extract\",\"refactor.inline\",\"refactor.rewrite\",\"source\",\"source.organizeImports\"]}},\"dynamicRegistration\":false},\"completion\":{\"completionItem\":{\"snippetSupport\":true,\"resolveSupport\":{\"properties\":[\"additionalTextEdits\"]},\"documentationFormat\":[\"markdown\",\"plaintext\"]},\"dynamicRegistration\":false,\"completionItemKind\":{\"valueSet\":[10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,1,2,3,4,5,6,7,8,9]}},\"formatting\":{\"dynamicRegistration\":false},\"codeLens\":{\"dynamicRegistration\":false},\"hover\":{\"dynamicRegistration\":false,\"contentFormat\":[\"markdown\",\"plaintext\"]},\"rangeFormatting\":{\"dynamicRegistration\":false},\"declaration\":{\"dynamicRegistration\":false,\"linkSupport\":true},\"references\":{\"dynamicRegistration\":false},\"typeHierarchy\":false,\"foldingRange\":{\"rangeLimit\":5000,\"dynamicRegistration\":false,\"lineFoldingOnly\":true},\"documentSymbol\":{\"symbolKind\":{\"valueSet\":[10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,1,2,3,4,5,6,7,8,9]},\"dynamicRegistration\":false,\"labelSupport\":false,\"hierarchicalDocumentSymbolSupport\":false},\"synchronization\":{\"dynamicRegistration\":false,\"willSaveWaitUntil\":false,\"willSave\":false,\"didSave\":true},\"documentHighlight\":{\"dynamicRegistration\":false},\"implementation\":{\"dynamicRegistration\":false,\"linkSupport\":true},\"typeDefinition\":{\"dynamicRegistration\":false,\"linkSupport\":true},\"definition\":{\"dynamicRegistration\":false,\"linkSupport\":true}}},\"rootPath\":\"C:\\\\Users\\\\ikeuc\\\\GitHub\\\\hidemaru_language_server_protocol\",\"clientInfo\":{\"name\":\"vim-lsp\"},\"processId\":25052,\"trace\":\"off\"}}";

			var json_string = @"{""id"":1,""jsonrpc"":""2.0"",""method"":""initialize"",""params"":{""rootUri"":""file:///C:/Users/ikeuc/GitHub/hidemaru_language_server_protocol/example"",""initializationOptions"":null,""capabilities"":{""workspace"":{""configuration"":true,""applyEdit"":true},""window"":{""workDoneProgress"":false},""textDocument"":{""semanticHighlightingCapabilities"":{""semanticHighlighting"":false},""codeAction"":{""disabledSupport"":true,""codeActionLiteralSupport"":{""codeActionKind"":{""valueSet"":["""",""quickfix"",""refactor"",""refactor.extract"",""refactor.inline"",""refactor.rewrite"",""source"",""source.organizeImports""]}},""dynamicRegistration"":false},""completion"":{""completionItem"":{""snippetSupport"":true,""resolveSupport"":{""properties"":[""additionalTextEdits""]},""documentationFormat"":[""markdown"",""plaintext""]},""dynamicRegistration"":false,""completionItemKind"":{""valueSet"":[10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,1,2,3,4,5,6,7,8,9]}},""formatting"":{""dynamicRegistration"":false},""codeLens"":{""dynamicRegistration"":false},""hover"":{""dynamicRegistration"":false,""contentFormat"":[""markdown"",""plaintext""]},""rangeFormatting"":{""dynamicRegistration"":false},""declaration"":{""dynamicRegistration"":false,""linkSupport"":true},""references"":{""dynamicRegistration"":false},""typeHierarchy"":false,""foldingRange"":{""rangeLimit"":5000,""dynamicRegistration"":false,""lineFoldingOnly"":true},""documentSymbol"":{""symbolKind"":{""valueSet"":[10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,1,2,3,4,5,6,7,8,9]},""dynamicRegistration"":false,""labelSupport"":false,""hierarchicalDocumentSymbolSupport"":false},""synchronization"":{""dynamicRegistration"":false,""willSaveWaitUntil"":false,""willSave"":false,""didSave"":true},""documentHighlight"":{""dynamicRegistration"":false},""implementation"":{""dynamicRegistration"":false,""linkSupport"":true},""typeDefinition"":{""dynamicRegistration"":false,""linkSupport"":true},""definition"":{""dynamicRegistration"":false,""linkSupport"":true}}},""clientInfo"":{""name"":""vim-lsp""},""trace"":""off""}}]";
			client.Send(json_string,1,client.ResponseInitialize);
#endif
        }
        static void InitializedClient(Client client)
		{
            var data = new InitializedParams();
            client.SendInitialized(data);
		}


    }    
}
