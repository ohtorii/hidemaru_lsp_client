using LSP.Model;
using LSP.Client;
using System;
using System.IO;
using System.Threading;


namespace ClientExample
{

    class Program
	{
        //static RequestIdGenerator requestIdGenerator = new RequestIdGenerator();
        static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\csharp\");
        static string solutionFileName = Path.Combine(rootPath, "simple.sln");
        static Uri rootUri = new Uri(rootPath);
        static Uri sourceUri= new Uri(rootUri, @"simple\Program.cs");
        static int sourceVersion = 0;

        static void Main(string[] args)
        {
            

            //var FileName = @"d:\Temp\LSP-Server\omnisharp-win-x64-1.37.5\OmniSharp.exe";//OmniSharp.exeが例外はいて動かない
            var FileName = @"d:\Temp\LSP-Server\omnisharp-win-x64-1.37.8\OmniSharp.exe";
            //var FileName = @"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\omnisharp-lsp\OmniSharp.exe";
#if false
            var Arguments = string.Format(@"-lsp -v --hostPID {0} --encoding utf-8", System.Diagnostics.Process.GetCurrentProcess().Id);
#else
            // --zero-based-indices
            var Arguments = string.Format(@"-lsp -v --source ""{0}"" --hostPID {1} --encoding utf-8", solutionFileName, System.Diagnostics.Process.GetCurrentProcess().Id);
#endif
            var client = new Client();
            client.StartLspProcess(FileName, Arguments);
            Console.WriteLine("==== InitializeServer ====");
            InitializeServer(client);
			while (client.Status != Client.Mode.ServerInitializeFinish)
			{
                Thread.Sleep(100);
			}

            Console.WriteLine("==== InitializedClient ====");
            InitializedClient(client);
            //Thread.Sleep(1000);
#if false
            Console.WriteLine("==== didChangeConfiguration ====");
            DidChangeConfiguration(client);
            Thread.Sleep(1000);
#endif
            Console.WriteLine("==== OpenTextDocument ====");
            DigOpen(client);
            //Thread.Sleep(1000);
#if false
            Console.WriteLine("==== DidChangen ====");
            DidChange(client);
            Thread.Sleep(1000);
#endif
            Console.WriteLine("==== Completion ====");
            Completion(client);
            //Thread.Sleep(1000);

            Console.WriteLine("続行するには何かキーを押してください．．．");
            Console.ReadKey();
        }
        
        
        static void InitializeServer(Client client)
		{
#if true
            var param = new InitializeParams ();
            param.rootUri = rootUri.AbsoluteUri;
            param.rootPath = rootUri.AbsolutePath;
            param.initializationOptions = null;
            param.workspaceFolders = new[] { new WorkspaceFolder {uri=rootUri.AbsoluteUri,name="VisualStudio-Solution" } };
            param.processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            param.capabilities = new ClientCapabilities();
            {
                var clientInfo=param.clientInfo;
                clientInfo.name = "hidemal-lsp";
                clientInfo.version = "1.1.1";
            }
            param.trace = "verbose";//"off";
            {
                var workspace = param.capabilities.workspace;
                //workspace.configuration = true;
                workspace.applyEdit = true;
                //workspace.workspaceFolders = true;
            }
			{
                var window = param.capabilities.window;
                window.workDoneProgress = false;
			}
            {
                param.capabilities.textDocument = new TextDocumentClientCapabilities();
                var textDocument = param.capabilities.textDocument;

                {//codeAction
                    textDocument.codeAction = new CodeActionClientCapabilities();
                    var codeAction = textDocument.codeAction;
                    codeAction.disabledSupport = true;
                    codeAction.codeActionLiteralSupport.codeActionKind.valueSet = new[]
                    {
                        CodeActionKind.Empty,
                        CodeActionKind.QuickFix,
                        CodeActionKind.Refactor,
                        CodeActionKind.RefactorExtract,
                        CodeActionKind.RefactorInline,
                        CodeActionKind.RefactorRewrite,
                        CodeActionKind.Source,
                        CodeActionKind.SourceOrganizeImports,
                    };
                    codeAction.dynamicRegistration = false;
                }
                {//completion
                    textDocument.completion = new CompletionClientCapabilities();
                    var completion = textDocument.completion;
                    completion.completionItem.snippetSupport = true;
                    completion.completionItem.resolveSupport.properties = new[] { "additionalTextEdits" };
                    completion.completionItem.documentationFormat = new[] { MarkupKind.markdown, MarkupKind.plaintext };
                    completion.dynamicRegistration = false;
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
                {//formatting
                    textDocument.formatting = new DocumentFormattingClientCapabilities();
                    textDocument.formatting.dynamicRegistration = false;
				}
                {//codeLens
                    textDocument.codeLens = new CodeLensClientCapabilities();
                    textDocument.codeLens.dynamicRegistration = false;
                }
				{//hover
                    textDocument.hover = new HoverClientCapabilities();
                    var hover = textDocument.hover;
                    hover.dynamicRegistration = false;
                    hover.contentFormat = new[] { MarkupKind.markdown, MarkupKind.plaintext };
				}
                {//rangeFormatting
                    textDocument.rangeFormatting = new DocumentRangeFormattingClientCapabilities();
                    textDocument.rangeFormatting.dynamicRegistration = false;
                }
				{
                    textDocument.declaration = new DeclarationClientCapabilities();
                    var declaration = textDocument.declaration;
                    declaration.dynamicRegistration = false;
                    declaration.linkSupport = true;
				}
                {//references
                    textDocument.references = new ReferenceClientCapabilities();
                    textDocument.references.dynamicRegistration = false;
                }
                {//foldingRange
                    textDocument.foldingRange = new FoldingRangeClientCapabilities();
                    var foldingRange = textDocument.foldingRange;
                    foldingRange.rangeLimit = 5000;
                    foldingRange.dynamicRegistration = false;
                    foldingRange.lineFoldingOnly = true;
                }
                {//documentSymbol
                    textDocument.documentSymbol = new DocumentSymbolClientCapabilities();
                    var documentSymbol = textDocument.documentSymbol;
                    documentSymbol.symbolKind.valueSet = new[] {
                        SymbolKind.File,
                        SymbolKind.Module,
                        SymbolKind.Namespace,
                        SymbolKind.Package,
                        SymbolKind.Class,
                        SymbolKind.Method,
                        SymbolKind.Property,
                        SymbolKind.Field,
                        SymbolKind.Constructor,
                        SymbolKind.Enum,
                        SymbolKind.Interface,
                        SymbolKind.Function,
                        SymbolKind.Variable,
                        SymbolKind.Constant,
                        SymbolKind.String,
                        SymbolKind.Number,
                        SymbolKind.Boolean,
                        SymbolKind.Array,
                        SymbolKind.Object,
                        SymbolKind.Key,
                        SymbolKind.Null,
                        SymbolKind.EnumMember,
                        SymbolKind.Struct,
                        SymbolKind.Event,
                        SymbolKind.Operator,
                        SymbolKind.TypeParameter,
                    };
                    documentSymbol.dynamicRegistration = false;
                    documentSymbol.labelSupport = false;
                    documentSymbol.hierarchicalDocumentSymbolSupport = false;
                }
                {//synchronization
                    textDocument.synchronization = new TextDocumentSyncClientCapabilities();
                    var synchronization=textDocument.synchronization;
                    synchronization.dynamicRegistration = false;
                    synchronization.willSaveWaitUntil = false;
                    synchronization.willSave = false;
                    synchronization.didSave = true;
                }
                {//documentHighlight
                    textDocument.documentHighlight = new DocumentHighlightClientCapabilities();
                    var documentHighlight = textDocument.documentHighlight;
                    documentHighlight.dynamicRegistration = false;
                }
                {//implementation
                    textDocument.implementation = new ImplementationClientCapabilities();
                    var implementation=textDocument.implementation;
                    implementation.dynamicRegistration = false;
                    implementation.linkSupport = true;
                }
                {//typeDefinition
                    textDocument.typeDefinition = new TypeDefinitionClientCapabilities();
                    var typeDefinition=textDocument.typeDefinition;
                    typeDefinition.dynamicRegistration = false;
                    typeDefinition.linkSupport = true;
                }
                {//definition
                    textDocument.definition = new DefinitionClientCapabilities();
                    var definition=textDocument.definition;
                    definition.dynamicRegistration = false;
                    definition.linkSupport = true;
                }
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
        static void DigOpen(Client client)
		{
            sourceVersion = 1;//Openしたのでとりあえず1にする。

            var param = new DidOpenTextDocumentParams();
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.textDocument.version = sourceVersion;
            param.textDocument.text = FileLoad(sourceUri.AbsolutePath);
            param.textDocument.languageId = "cs";
            client.SendTextDocumentDigOpen(param);
        }
        static string FileLoad(string filename)
		{
            var text = File.ReadAllText(filename, System.Text.Encoding.UTF8);
#if true
            return text;
#else
            return text.Replace("\r\n", "\n");
#endif
        }
        static void DidChangeConfiguration(Client client)
		{
            var param = new DidChangeConfigurationParams();
            param.settings = new object();
            client.SendWorkspaceDidChangeConfiguration(param);
        }
        static void DidChange(Client client)
		{
            var text = FileLoad(sourceUri.AbsolutePath);
            ++sourceVersion;//ソースを更新したので+1する

            var param = new DidChangeTextDocumentParams();            
            param.contentChanges = new[] { new TextDocumentContentChangeEvent { text = text } };
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.textDocument.version= sourceVersion;

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
            param.position.line=12;
            param.position.character = 27;
            client.SendTextDocumentCompletion(param);
		}
    }    
}
