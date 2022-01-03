using LSP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HidemaruLspClient_BackEnd.LSP
{
    class ServerCapabilities: HidemaruLspClient_BackEndContract.IServerCapabilities
    {        
        public ServerCapabilities(InitializeResult result)
        {

            var capabilities=result.capabilities;

            CompletionProvider_               = Check(capabilities.completionProvider);
            HoverProvider_                    = Check(capabilities.hoverProvider);
            SignatureHelpProvider_            = Check(capabilities.signatureHelpProvider);
            DeclarationProvider_              = Check(capabilities.declarationProvider);
            DefinitionProvider_               = Check(capabilities.definitionProvider);
            TypeDefinitionProvider_           = Check(capabilities.typeDefinitionProvider);
            ImplementationProvider_           = Check(capabilities.implementationProvider);
            ReferencesProvider_               = Check(capabilities.referencesProvider);
            DocumentHighlightProvider_        = Check(capabilities.documentHighlightProvider);
            DocumentSymbolProvider_           = Check(capabilities.documentSymbolProvider);
            CodeActionProvider_               = Check(capabilities.codeActionProvider);
            CodeLensProvider_                 = Check(capabilities.codeLensProvider);
            DocumentLinkProvider_             = Check(capabilities.documentLinkProvider);
            ColorProvider_                    = Check(capabilities.colorProvider);
            DocumentFormattingProvider_       = Check(capabilities.documentFormattingProvider);
            DocumentRangeFormattingProvider_  = Check(capabilities.documentRangeFormattingProvider);
            DocumentOnTypeFormattingProvider_ = Check(capabilities.documentOnTypeFormattingProvider);
            RenameProvider_                   = Check(capabilities.renameProvider);
            FoldingRangeProvider_             = Check(capabilities.foldingRangeProvider);
            ExecuteCommandProvider_           = Check(capabilities.executeCommandProvider);
            SelectionRangeProvider_           = Check(capabilities.selectionRangeProvider);
            LinkedEditingRangeProvider_       = Check(capabilities.linkedEditingRangeProvider);
            CallHierarchyProvider_            = Check(capabilities.callHierarchyProvider);

            //ToDo:後で実装(semanticTokensProvider)
            //SemanticTokensProvider_         = Check(capabilities.semanticTokensProvider);

            MonikerProvider_                  = Check(capabilities.monikerProvider);
            WorkspaceSymbolProvider_          = Check(capabilities.workspaceSymbolProvider);

        }
        public sbyte CompletionProvider               => CompletionProvider_;
        public sbyte HoverProvider                    => HoverProvider_;
        public sbyte SignatureHelpProvider            => SignatureHelpProvider_;
        public sbyte DeclarationProvider              => DeclarationProvider_;
        public sbyte DefinitionProvider               => DefinitionProvider_;
        public sbyte TypeDefinitionProvider           => TypeDefinitionProvider_;
        public sbyte ImplementationProvider           => ImplementationProvider_;
        public sbyte ReferencesProvider               => ReferencesProvider_;
        public sbyte DocumentHighlightProvider        => DocumentHighlightProvider_;
        public sbyte DocumentSymbolProvider           => DocumentSymbolProvider_;
        public sbyte CodeActionProvider               => CodeActionProvider_;
        public sbyte CodeLensProvider                 => CodeLensProvider_;
        public sbyte DocumentLinkProvider             => DocumentLinkProvider_;
        public sbyte ColorProvider                    => ColorProvider_;
        public sbyte DocumentFormattingProvider       => DocumentFormattingProvider_;
        public sbyte DocumentRangeFormattingProvider  => DocumentRangeFormattingProvider_;
        public sbyte DocumentOnTypeFormattingProvider => DocumentOnTypeFormattingProvider_;
        public sbyte RenameProvider                   => RenameProvider_;
        public sbyte FoldingRangeProvider             => FoldingRangeProvider_;
        public sbyte ExecuteCommandProvider           => ExecuteCommandProvider_;
        public sbyte SelectionRangeProvider           => SelectionRangeProvider_;
        public sbyte LinkedEditingRangeProvider       => LinkedEditingRangeProvider_;
        public sbyte CallHierarchyProvider            => CallHierarchyProvider_;
        public sbyte SemanticTokensProvider           => SemanticTokensProvider_;
        public sbyte MonikerProvider                  => MonikerProvider_;
        public sbyte WorkspaceSymbolProvider          => WorkspaceSymbolProvider_;

        sbyte/*bool*/ CompletionProvider_;
        sbyte/*bool*/ HoverProvider_;
        sbyte/*bool*/ SignatureHelpProvider_;
        sbyte/*bool*/ DeclarationProvider_;
        sbyte/*bool*/ DefinitionProvider_;
        sbyte/*bool*/ TypeDefinitionProvider_;
        sbyte/*bool*/ ImplementationProvider_;
        sbyte/*bool*/ ReferencesProvider_;
        sbyte/*bool*/ DocumentHighlightProvider_;
        sbyte/*bool*/ DocumentSymbolProvider_;
        sbyte/*bool*/ CodeActionProvider_;
        sbyte/*bool*/ CodeLensProvider_;
        sbyte/*bool*/ DocumentLinkProvider_;
        sbyte/*bool*/ ColorProvider_;
        sbyte/*bool*/ DocumentFormattingProvider_;
        sbyte/*bool*/ DocumentRangeFormattingProvider_;
        sbyte/*bool*/ DocumentOnTypeFormattingProvider_;
        sbyte/*bool*/ RenameProvider_;
        sbyte/*bool*/ FoldingRangeProvider_;
        sbyte/*bool*/ ExecuteCommandProvider_;
        sbyte/*bool*/ SelectionRangeProvider_;
        sbyte/*bool*/ LinkedEditingRangeProvider_;
        sbyte/*bool*/ CallHierarchyProvider_;
        sbyte/*bool*/ SemanticTokensProvider_;
        sbyte/*bool*/ MonikerProvider_;
        sbyte/*bool*/ WorkspaceSymbolProvider_;


        static readonly sbyte True = Convert.ToSByte(true);
        static readonly sbyte False = Convert.ToSByte(false);

        static sbyte Check(object value)
        {
            if (value == null)
            {
                return False;
            }
            return True;
        }
        static sbyte Check<T>( BooleanOr<T> value) where T : class
        {
            if (value == null)
            {
                return False;
            }
            if (value.IsBool)
            {
                return value.Bool ? True : False;
            }
            return True;
        }
    }
}
