using Newtonsoft.Json;
using System;
using DocumentUri = System.String;

namespace LSP.Model
{
    /**
    * クライアントについての情報
    *
    * @since 3.15.0
    */
    class ClientInfo_
    {
        public string name;
        public string version;
    };

    interface IInitializeParams : IWorkDoneProgressParams
    {
        int processId { get; set; }
        ClientInfo_ clientInfo { get; /*set;*/ }
        //
        [Obsolete("@deprecated in favour of rootUri.")]
        string rootPath { get; set; }
        DocumentUri rootUri { get; set; }
        object initializationOptions { get; set; }
        ClientCapabilities capabilities { get; set; }
        string trace { get; set; }
        WorkspaceFolder[] workspaceFolders { get; set; }
    }
    class InitializeParams : IInitializeParams
    {
        public int processId { get; set; }
        public ClientInfo_ clientInfo
        {
            get
            {
                if (m_clientInfo == null) { m_clientInfo = new ClientInfo_(); };
                return m_clientInfo;
            }
        }
        public string rootUri { get; set; }
        public object initializationOptions { get; set; }
        public ClientCapabilities capabilities { get; set; }
        public string trace { get; set; } = "off"; //"off" | "messages" | "verbose";
        /*Memo: インスタンスを生成するとサーバがResponseを返さないため、nullで運用中。
		 */
        public WorkspaceFolder[] workspaceFolders { get; set; } = null;
        public string rootPath { get; set; }
        ProgressToken IWorkDoneProgressParams.workDoneToken { get; set; }

        [JsonIgnore] ClientInfo_ m_clientInfo = null;
    }

    interface IInitializeResult
    {
        ServerCapabilities capabilities { get; set; }
        serverInfo_ serverInfo { get; set; }
    }
    class InitializeResult : IInitializeResult
    {
        public ServerCapabilities capabilities { get; set; } = new ServerCapabilities();
        public serverInfo_ serverInfo { get; set; } = new serverInfo_();
    }



    class serverInfo_
    {
        /**
		 * The name of the server as defined by the server.
		 */
        public string name;

        /**
		 * The server's version as defined by the server.
		 */
        public string version;
    }
    class ServerCapabilities
    {
        /**
		 * Defines how text documents are synced.
		 * Is either a detailed structure defining each notification
		 * or for backwards compatibility, the TextDocumentSyncKind number.
		 * If omitted, it defaults to `TextDocumentSyncKind.None`.
		 */
        public TextDocumentSync /*TextDocumentSyncOptions | TextDocumentSyncKind*/ textDocumentSync = null;

        /**
		 * The server provides completion support.
		 */
        public CompletionOptions completionProvider = null;

        /**
		 * The server provides hover support.
		 */
        public BooleanOr<HoverOptions> /*boolean | HoverOptions*/ hoverProvider = null;

        /**
		 * The server provides signature help support.
		 */
        public SignatureHelpOptions signatureHelpProvider = null;

        /**
		 * The server provides go to declaration support.
		 *
		 * @since 3.14.0
		 */
        public BooleanOr<DeclarationRegistrationOptions> /*boolean | DeclarationOptions| DeclarationRegistrationOptions*/ declarationProvider = null;

        /**
		 * The server provides goto definition support.
		 */
        public BooleanOr<DefinitionRegistrationOptions> /*boolean | DefinitionOptions| */ definitionProvider = null;

        /**
		 * The server provides goto type definition support.
		 *
		 * @since 3.6.0
		 */
        public BooleanOr<TypeDefinitionRegistrationOptions> /*boolean | TypeDefinitionOptions | TypeDefinitionRegistrationOptions*/ typeDefinitionProvider = null;

        /**
		 * The server provides goto implementation support.
		 *
		 * @since 3.6.0
		 */
        public BooleanOr<ImplementationRegistrationOptions> /*boolean | ImplementationOptions| */  implementationProvider = null;

        /**
		 * The server provides find references support.
		 */
        public BooleanOr<ReferenceOptions> /*boolean | ReferenceOptions*/referencesProvider = null;

        /**
		 * The server provides document highlight support.
		 */
        public BooleanOr<DocumentHighlightOptions> /*boolean | DocumentHighlightOptions;*/ documentHighlightProvider = null;

        /**
		 * The server provides document symbol support.
		 */
        public BooleanOr<DocumentSymbolOptions> /*boolean | DocumentSymbolOptions*/documentSymbolProvider = null;

        /**
		 * The server provides code actions.
		 * The `CodeActionOptions` return type is only valid if the client signals
		 * code action literal support via the property
		 * `textDocument.codeAction.codeActionLiteralSupport`.
		 */
        public BooleanOr<CodeActionOptions> /* boolean | CodeActionOptions*/codeActionProvider = null;

        /**
		 * The server provides CodeLens.
		 */
        public CodeLensOptions codeLensProvider = null;

        /**
		 * The server provides document link support.
		 */
        public DocumentLinkOptions documentLinkProvider = null;

        /**
		 * The server provides color provider support.
		 *
		 * @since 3.6.0
		 */
        public BooleanOr<DocumentColorRegistrationOptions>/*boolean | DocumentColorOptions| DocumentColorRegistrationOptions;*/colorProvider = null;

        /**
		 * The server provides document formatting.
		 */
        public BooleanOr<DocumentFormattingOptions> /*boolean | DocumentFormattingOptions;*/documentFormattingProvider = null;

        /**
		 * The server provides document range formatting.
		 */
        public BooleanOr<DocumentRangeFormattingOptions> /*boolean | DocumentRangeFormattingOptions*/ documentRangeFormattingProvider = null;

        /**
		 * The server provides document formatting on typing.
		 */
        public DocumentOnTypeFormattingOptions documentOnTypeFormattingProvider = null;

        /**
		 * The server provides rename support. RenameOptions may only be
		 * specified if the client states that it supports
		 * `prepareSupport` in its initial `initialize` request.
		 */
        public BooleanOr<RenameOptions> /*boolean | RenameOptions*/renameProvider = null;

        /**
		 * The server provides folding provider support.
		 *
		 * @since 3.10.0
		 */
        public BooleanOr<FoldingRangeRegistrationOptions>/*boolean | FoldingRangeOptions| FoldingRangeRegistrationOptions*/foldingRangeProvider = null;

        /**
		 * The server provides execute command support.
		 */
        public ExecuteCommandOptions executeCommandProvider = null;

        /**
		 * The server provides selection range support.
		 *
		 * @since 3.15.0
		 */
        public BooleanOr<SelectionRangeRegistrationOptions> /*boolean | SelectionRangeOptions| SelectionRangeRegistrationOptions;*/ selectionRangeProvider = null;

        /**
		 * The server provides linked editing range support.
		 *
		 * @since 3.16.0
		 */
        public BooleanOr<LinkedEditingRangeRegistrationOptions>/*boolean | LinkedEditingRangeOptions| LinkedEditingRangeRegistrationOptions;*/ linkedEditingRangeProvider = null;

        /**
		 * The server provides call hierarchy support.
		 *
		 * @since 3.16.0
		 */
        public BooleanOr<CallHierarchyRegistrationOptions>/*boolean | CallHierarchyOptions| CallHierarchyRegistrationOptions;*/ callHierarchyProvider = null;

        //ToDo:後で実装(semanticTokensProvider)
#if false
		/**
		 * The server provides semantic tokens support.
		 *
		 * @since 3.16.0
		 */
		public BooleanOr<SemanticTokensRegistrationOptions> /*SemanticTokensOptions| SemanticTokensRegistrationOptions;*/ semanticTokensProvider=null;
#endif
        /**
		 * Whether server provides moniker support.
		 *
		 * @since 3.16.0
		 */
        public BooleanOr<MonikerRegistrationOptions>/*boolean | MonikerOptions | MonikerRegistrationOptions;*/	monikerProvider = null;


        /**
		 * The server provides workspace symbol support.
		 */
        public BooleanOr<WorkspaceSymbolOptions> /*boolean | WorkspaceSymbolOptions*/ workspaceSymbolProvider = null;

        /**
		 * Workspace specific server capabilities
		 */
        public class _workspace
        {
            /**
			 * The server supports workspace folder.
			 *
			 * @since 3.6.0
			 */
            public WorkspaceFoldersServerCapabilities workspaceFolders = null;

            /**
			 * The server is interested in file notifications/requests.
			 *
			 * @since 3.16.0
			 */
            public class _fileOperations
            {
                /**
				 * The server is interested in receiving didCreateFiles
				 * notifications.
				 */
                public FileOperationRegistrationOptions didCreate = null;

                /**
				 * The server is interested in receiving willCreateFiles requests.
				 */
                public FileOperationRegistrationOptions willCreate = null;

                /**
				 * The server is interested in receiving didRenameFiles
				 * notifications.
				 */
                public FileOperationRegistrationOptions didRename = null;

                /**
				 * The server is interested in receiving willRenameFiles requests.
				 */
                public FileOperationRegistrationOptions willRename = null;

                /**
				 * The server is interested in receiving didDeleteFiles file
				 * notifications.
				 */
                public FileOperationRegistrationOptions didDelete = null;

                /**
				 * The server is interested in receiving willDeleteFiles file
				 * requests.
				 */
                public FileOperationRegistrationOptions willDelete = null;
            }
            public _fileOperations fileOperations
            {
                get
                {
                    if (m_fileOperations == null)
                    {
                        m_fileOperations = new _fileOperations();
                    }
                    return m_fileOperations;
                }
                set
                {
                    m_fileOperations = value;
                }
            }
            [JsonIgnore] _fileOperations m_fileOperations = null;
        }
        public _workspace workspace;

        /**
		 * Experimental server capabilities.
		 */
        public object experimental;
    }
}
