using System.Runtime.InteropServices;

namespace HidemaruLspClient_FrontEnd.BackEndContractImpl
{
	[ComVisible(true)]
	[Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B95A0")]
	sealed class ServerCapabilitiesImpl : HidemaruLspClient_BackEndContract.IServerCapabilities
    {
        HidemaruLspClient_BackEndContract.IServerCapabilities serverCapabilities_;
        public ServerCapabilitiesImpl(HidemaruLspClient_BackEndContract.IServerCapabilities serverCapabilities)
        {
            serverCapabilities_ = serverCapabilities;
        }
        public sbyte CompletionProvider => serverCapabilities_.CompletionProvider;

        public sbyte HoverProvider => serverCapabilities_.HoverProvider;

        public sbyte SignatureHelpProvider => serverCapabilities_.SignatureHelpProvider;

        public sbyte DeclarationProvider => serverCapabilities_.DeclarationProvider;

        public sbyte DefinitionProvider => serverCapabilities_.DefinitionProvider;

        public sbyte TypeDefinitionProvider => serverCapabilities_.TypeDefinitionProvider;

        public sbyte ImplementationProvider => serverCapabilities_.ImplementationProvider;

        public sbyte ReferencesProvider => serverCapabilities_.ReferencesProvider;

        public sbyte DocumentHighlightProvider => serverCapabilities_.DocumentHighlightProvider;

        public sbyte DocumentSymbolProvider => serverCapabilities_.DocumentSymbolProvider;

        public sbyte CodeActionProvider => serverCapabilities_.CodeActionProvider;

        public sbyte CodeLensProvider => serverCapabilities_.CodeLensProvider;

        public sbyte DocumentLinkProvider => serverCapabilities_.DocumentLinkProvider;

        public sbyte ColorProvider => serverCapabilities_.ColorProvider;

        public sbyte DocumentFormattingProvider => serverCapabilities_.DocumentFormattingProvider;

        public sbyte DocumentRangeFormattingProvider => serverCapabilities_.DocumentRangeFormattingProvider;

        public sbyte DocumentOnTypeFormattingProvider => serverCapabilities_.DocumentOnTypeFormattingProvider;

        public sbyte RenameProvider => serverCapabilities_.RenameProvider;

        public sbyte FoldingRangeProvider => serverCapabilities_.FoldingRangeProvider;

        public sbyte ExecuteCommandProvider => serverCapabilities_.ExecuteCommandProvider;

        public sbyte SelectionRangeProvider => serverCapabilities_.SelectionRangeProvider;

        public sbyte LinkedEditingRangeProvider => serverCapabilities_.LinkedEditingRangeProvider;

        public sbyte CallHierarchyProvider => serverCapabilities_.CallHierarchyProvider;

        public sbyte SemanticTokensProvider => serverCapabilities_.SemanticTokensProvider;

        public sbyte MonikerProvider => serverCapabilities_.MonikerProvider;

        public sbyte WorkspaceSymbolProvider => serverCapabilities_.WorkspaceSymbolProvider;
    }
}
