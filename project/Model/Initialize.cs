using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentUri = System.String;

namespace LSP.Model
{
    /**
    * クライアントについての情報
    *
    * @since 3.15.0
    */
    public class ClientInfo_
    {
        /**
         * クライアント自身により定義されたクライアントの名前。
         */
        public string name = "hidemaru-lsp";

        /**
            * クライアント自身により定義されたクライアントのバージョン。
            */
        public string version = "1.111";
    };        
    
    interface IInitializeParams : IWorkDoneProgressParams
	{                				
        int processId { get; set; }
        ClientInfo_ clientInfo { get; set; }
		/* @deprecated in favour of rootUri.
		string rootPath { get; set; }*/
        DocumentUri rootUri { get; set; }
        object initializationOptions { get; set; }
        ClientCapabilities capabilities { get; set; }       
        string trace { get; set; } 
        WorkspaceFolder workspaceFolders { get; set; }        
    }
	class InitializeParams : IInitializeParams
	{
		public int processId { get; set; }
		public ClientInfo_ clientInfo { get; set; }
		public string rootUri { get; set; }
		public object initializationOptions { get; set; }
		public ClientCapabilities capabilities { get; set; }
		public string trace { get; set; } = "off"; //"off" | "messages" | "verbose";
		/*Memo: インスタンスを生成するとサーバがResponseを返さないため、nullで運用中。
		 */
		public WorkspaceFolder workspaceFolders { get; set; } = null; /*= new WorkspaceFolder();*/
		public short workDoneToken { get; set; }
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
		public TextDocumentSyncOptions /*| number*/ textDocumentSync;

		/**
		 * The server provides completion support.
		 */
		public CompletionOptions completionProvider;

		/**
		 * The server provides hover support.
		 */
		public /*boolean | */HoverOptions hoverProvider;

		/**
		 * The server provides signature help support.
		 */
		public SignatureHelpOptions signatureHelpProvider;

		/**
		 * The server provides go to declaration support.
		 *
		 * @since 3.14.0
		 */
		//Todo: 今のところサーバから送信されていない、public boolean | DeclarationOptions| DeclarationRegistrationOptions declarationProvider;

		/**
		 * The server provides goto definition support.
		 */
		public /*boolean | DefinitionOptions| */ DefinitionRegistrationOptions definitionProvider;

		/**
		 * The server provides goto type definition support.
		 *
		 * @since 3.6.0
		 */
		//Todo: 今のところサーバから送信されていない、typeDefinitionProvider?: boolean | TypeDefinitionOptions | TypeDefinitionRegistrationOptions;

		/**
		 * The server provides goto implementation support.
		 *
		 * @since 3.6.0
		 */
		public /*boolean | ImplementationOptions| */ImplementationRegistrationOptions implementationProvider;

		/**
		 * The server provides find references support.
		 */
		public ReferenceOptions referencesProvider;

		/**
		 * The server provides document highlight support.
		 */
		//Todo:今のところサーバから返信されていない	documentHighlightProvider?: boolean | DocumentHighlightOptions;

		/**
		 * The server provides document symbol support.
		 */
		public DocumentSymbolOptions documentSymbolProvider;

		/**
		 * The server provides code actions.
		 * The `CodeActionOptions` return type is only valid if the client signals
		 * code action literal support via the property
		 * `textDocument.codeAction.codeActionLiteralSupport`.
		 */
		public CodeActionOptions codeActionProvider;

		/**
		 * The server provides CodeLens.
		 */
		public CodeLensOptions codeLensProvider;

		/**
		 * The server provides document link support.
		 */
		//Todo: 今のところサーバから返信されていない　documentLinkProvider?: DocumentLinkOptions;

		/**
		 * The server provides color provider support.
		 *
		 * @since 3.6.0
		 */
		//Todo: 今のところサーバから返信されていない　colorProvider?: boolean | DocumentColorOptions| DocumentColorRegistrationOptions;

		/**
		 * The server provides document formatting.
		 */
		public DocumentFormattingOptions documentFormattingProvider;

		/**
		 * The server provides document range formatting.
		 */
		public DocumentRangeFormattingOptions documentRangeFormattingProvider;

		/**
		 * The server provides document formatting on typing.
		 */
		public DocumentOnTypeFormattingOptions documentOnTypeFormattingProvider;

		/**
		 * The server provides rename support. RenameOptions may only be
		 * specified if the client states that it supports
		 * `prepareSupport` in its initial `initialize` request.
		 */
		public RenameOptions renameProvider;

		/**
		 * The server provides folding provider support.
		 *
		 * @since 3.10.0
		 */
		//Todo: 今のところサーバから送信されていない　foldingRangeProvider?: boolean | FoldingRangeOptions| FoldingRangeRegistrationOptions;

		/**
		 * The server provides execute command support.
		 */
		public ExecuteCommandOptions executeCommandProvider;

		/**
		 * The server provides selection range support.
		 *
		 * @since 3.15.0
		 */
		//Todo: 今のところサーバから送信されていない　selectionRangeProvider?: boolean | SelectionRangeOptions| SelectionRangeRegistrationOptions;

		/**
		 * The server provides workspace symbol support.
		 */
		public WorkspaceSymbolOptions workspaceSymbolProvider;

		/**
		 * Workspace specific server capabilities
		 */	
		public class _workspace {
			/**
			 * The server supports workspace folder.
			 *
			 * @since 3.6.0
			 */
			public WorkspaceFoldersServerCapabilities workspaceFolders;

			/**
			 * The server is interested in file notifications/requests.
			 *
			 * @since 3.16.0
			 */
			public class _fileOperations {
				/**
				 * The server is interested in receiving didCreateFiles
				 * notifications.
				 */
				public FileOperationRegistrationOptions didCreate;

				/**
				 * The server is interested in receiving willCreateFiles requests.
				 */
				public FileOperationRegistrationOptions willCreate;

				/**
				 * The server is interested in receiving didRenameFiles
				 * notifications.
				 */
				public FileOperationRegistrationOptions didRename;

				/**
				 * The server is interested in receiving willRenameFiles requests.
				 */
				public FileOperationRegistrationOptions willRename;

				/**
				 * The server is interested in receiving didDeleteFiles file
				 * notifications.
				 */
				public FileOperationRegistrationOptions didDelete;

				/**
				 * The server is interested in receiving willDeleteFiles file
				 * requests.
				 */
				public FileOperationRegistrationOptions willDelete;
			}
			public _fileOperations fileOperations;
		}
		public _workspace workspace;

		/**
		 * Experimental server capabilities.
		 */
		public object experimental;
	}
}
