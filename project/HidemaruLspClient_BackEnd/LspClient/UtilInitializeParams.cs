using LSP.Model;
using System.Reflection;

namespace HidemaruLspClient.LspClient
{
    class UtilInitializeParams
    {
        static public InitializeParams Initialzie()
        {
            var param = new InitializeParams();
            param.initializationOptions = null;
            param.processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            param.capabilities = new ClientCapabilities();
            {
                var clientInfo = param.clientInfo;
                clientInfo.name = "hidemal-lsp";
                clientInfo.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            param.trace = "verbose";//"off";
            {
                var workspace = param.capabilities.workspace;
                workspace.configuration = true;
                workspace.applyEdit = true;
                workspace.workspaceFolders = true;
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
                    completion.completionItem.snippetSupport = false;
                    completion.completionItem.resolveSupport.properties = new[] { "additionalTextEdits" };
                    completion.completionItem.documentationFormat = new[] { /*MarkupKind.markdown,*/ MarkupKind.plaintext };
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
                    hover.contentFormat = new[] { /*MarkupKind.markdown,*/ MarkupKind.plaintext };
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
                    var synchronization = textDocument.synchronization;
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
                    var implementation = textDocument.implementation;
                    implementation.dynamicRegistration = false;
                    implementation.linkSupport = true;
                }
                {//typeDefinition
                    textDocument.typeDefinition = new TypeDefinitionClientCapabilities();
                    var typeDefinition = textDocument.typeDefinition;
                    typeDefinition.dynamicRegistration = false;
                    typeDefinition.linkSupport = true;
                }
                {//definition
                    textDocument.definition = new DefinitionClientCapabilities();
                    var definition = textDocument.definition;
                    definition.dynamicRegistration = false;
                    definition.linkSupport = true;
                }
            }
            return param;
        }
    }
}
