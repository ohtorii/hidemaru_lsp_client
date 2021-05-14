using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Model
{
	class ClientCapabilities
	{
		/**
			 * ワークスペース固有のクライアント機能。
			 */
		public class _workspace
		{
			/**
			 * The client supports applying batch edits
			 * to the workspace by supporting the request
			 * 'workspace/applyEdit'
			 */
			public bool applyEdit;
			public WorkspaceEditClientCapabilities workspaceEdit;

			public DidChangeConfigurationClientCapabilities didChangeConfiguration;

			public DidChangeWatchedFilesClientCapabilities didChangeWatchedFiles;

			public WorkspaceSymbolClientCapabilities symbol;
			public ExecuteCommandClientCapabilities executeCommand;
			public bool workspaceFolders;
			public bool configuration;
			public SemanticTokensWorkspaceClientCapabilities semanticTokens;
			public CodeLensWorkspaceClientCapabilities codeLens;

			public class _fileOperations {				
				public bool dynamicRegistration;
				public bool didCreate;
				public bool willCreate;
				public bool didRename;
				public bool willRename;
				public bool didDelete;
				public bool willDelete;
			};
			public _fileOperations fileOperations;
		};

		public _workspace workspace { 
			get 
			{
				if (m_workspace == null)
				{
					m_workspace = new _workspace();
				}
				return m_workspace;
			} 
			set 
			{
				m_workspace = value;
			} 
		}
		[JsonIgnore] _workspace m_workspace=null;

		public TextDocumentClientCapabilities textDocument;
	
		public class _window {
			public bool workDoneProgress;
			public ShowMessageRequestClientCapabilities showMessage;
			public ShowDocumentClientCapabilities showDocument;
		};
		public _window window { 
			get 
			{
				if (m_window == null)
				{
					m_window = new _window();
				}
				return m_window;
			} 
			set 
			{
				m_window = value;
			} 
		}
		[JsonIgnore] _window m_window=null;

		public class _general {
			public RegularExpressionsClientCapabilities regularExpressions ;
			public MarkdownClientCapabilities markdown ;
		};
		public _general general { 
			get 
			{
				if (m_general == null)
				{
					m_general = new _general();
				}
				return m_general;
			}
			set 
			{
				m_general = value;
			} 
		}
		[JsonIgnore] _general m_general = null;

		public object experimental;
	}

	class TextDocumentClientCapabilities
	{
		public TextDocumentSyncClientCapabilities synchronization ;

		/**
		 * Capabilities specific to the `textDocument/completion` request.
		 */
		public CompletionClientCapabilities completion ;

		/**
		 * Capabilities specific to the `textDocument/hover` request.
		 */
		public HoverClientCapabilities hover;

		/**
		 * Capabilities specific to the `textDocument/signatureHelp` request.
		 */
		public SignatureHelpClientCapabilities signatureHelp;

		/**
		 * Capabilities specific to the `textDocument/declaration` request.
		 *
		 * @since 3.14.0
		 */
		public DeclarationClientCapabilities declaration;

		/**
		 * Capabilities specific to the `textDocument/definition` request.
		 */
		public DefinitionClientCapabilities definition;

		/**
		 * Capabilities specific to the `textDocument/typeDefinition` request.
		 *
		 * @since 3.6.0
		 */
		public TypeDefinitionClientCapabilities typeDefinition;

		/**
		 * Capabilities specific to the `textDocument/implementation` request.
		 *
		 * @since 3.6.0
		 */
		public ImplementationClientCapabilities implementation;

		/**
		 * Capabilities specific to the `textDocument/references` request.
		 */
		public ReferenceClientCapabilities references;

		/**
		 * Capabilities specific to the `textDocument/documentHighlight` request.
		 */
		public DocumentHighlightClientCapabilities documentHighlight;

		/**
		 * Capabilities specific to the `textDocument/documentSymbol` request.
		 */
		public DocumentSymbolClientCapabilities documentSymbol;

		/**
		 * Capabilities specific to the `textDocument/codeAction` request.
		 */
		public CodeActionClientCapabilities codeAction;

		/**
		 * Capabilities specific to the `textDocument/codeLens` request.
		 */
		public CodeLensClientCapabilities codeLens;

		/**
		 * Capabilities specific to the `textDocument/documentLink` request.
		 */
		public DocumentLinkClientCapabilities documentLink;

		/**
		 * Capabilities specific to the `textDocument/documentColor` and the
		 * `textDocument/colorPresentation` request.
		 *
		 * @since 3.6.0
		 */
		public DocumentColorClientCapabilities colorProvider;

		/**
		 * Capabilities specific to the `textDocument/formatting` request.
		 */
		public DocumentFormattingClientCapabilities formatting;

		/**
		 * Capabilities specific to the `textDocument/rangeFormatting` request.
		 */
		public DocumentRangeFormattingClientCapabilities rangeFormatting;

		/** request.
		 * Capabilities specific to the `textDocument/onTypeFormatting` request.
		 */
		public DocumentOnTypeFormattingClientCapabilities onTypeFormatting;

		/**
		 * Capabilities specific to the `textDocument/rename` request.
		 */
		public RenameClientCapabilities rename;

		/**
		 * Capabilities specific to the `textDocument/publishDiagnostics`
		 * notification.
		 */
		public PublishDiagnosticsClientCapabilities publishDiagnostics;

		/**
		 * Capabilities specific to the `textDocument/foldingRange` request.
		 *
		 * @since 3.10.0
		 */
		public FoldingRangeClientCapabilities foldingRange;

		/**
		 * Capabilities specific to the `textDocument/selectionRange` request.
		 *
		 * @since 3.15.0
		 */
		public SelectionRangeClientCapabilities selectionRange;
	}

	class TextDocumentSyncClientCapabilities
	{
		/**
		 * Whether text document synchronization supports dynamic registration.
		 */
		public bool dynamicRegistration = false;

		/**
		 * The client supports sending will save notifications.
		 */
		public bool willSave = false;

		/**
		 * The client supports sending a will save request and
		 * waits for a response providing text edits which will
		 * be applied to the document before it is saved.
		 */
		public bool willSaveWaitUntil = false;

		/**
		 * The client supports did save notifications.
		 */
		public bool didSave = false;
	}


	
}
