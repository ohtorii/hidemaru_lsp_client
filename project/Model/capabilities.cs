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
			* クライアントは `workspace/applyEdit` リクエストをサポートすることにより、
			* ワークスペースへのバッチ編集をサポートする。
			*/
			public bool applyEdit;

			/**
			* `WorkspaceEdit` 固有の機能。
			*/
			//workspaceEdit?: WorkspaceEditClientCapabilities;

			/**
			* `workspace/didChangeConfiguration` 通知固有の機能。
			*/
			//didChangeConfiguration?: DidChangeConfigurationClientCapabilities;

			/**
			* `workspace/didChangeWatchedFiles` 通知固有の機能。
			*/
			//didChangeWatchedFiles?: DidChangeWatchedFilesClientCapabilities;

			/**
			* `workspace/symbol` リクエスト固有の機能。
			*/
			//symbol?: WorkspaceSymbolClientCapabilities;

			/**
			* `workspace/executeCommand` リクエスト固有の機能。
			*/
			//executeCommand?: ExecuteCommandClientCapabilities;


			/**
			* The client has support for workspace folders.
			*
			* Since 3.6.0
			*/
			public bool workspaceFolders;

			/**
			* The client supports `workspace/configuration` requests.
			*
			* Since 3.6.0
			*/
			public bool configuration;
		};
		public _workspace workspace = new _workspace();

		/**
		 * テキストドキュメント固有のクライアント機能。
		 */
		public TextDocumentClientCapabilities textDocument = new TextDocumentClientCapabilities();

		/**
		* Whether client supports handling progress notifications.
		* If set, servers are allowed to report in `workDoneProgress` property
		* in the request specific server capabilities.
		*
		* Since 3.15.0
		*/
		public WorkDoneProgressOptions window = new WorkDoneProgressOptions();

		/**
		 * 実験的なクライアント機能。
		 */
		//experimental?: any;
	}

	class TextDocumentClientCapabilities
	{
		public TextDocumentSyncClientCapabilities synchronization = new TextDocumentSyncClientCapabilities();

		/**
		 * Capabilities specific to the `textDocument/completion` request.
		 */
		public CompletionClientCapabilities completion = new CompletionClientCapabilities();

		/**
		 * Capabilities specific to the `textDocument/hover` request.
		 */
		//hover?: HoverClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/signatureHelp` request.
		 */
		//signatureHelp?: SignatureHelpClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/declaration` request.
		 *
		 * @since 3.14.0
		 */
		//declaration?: DeclarationClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/definition` request.
		 */
		//definition?: DefinitionClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/typeDefinition` request.
		 *
		 * @since 3.6.0
		 */
		//typeDefinition?: TypeDefinitionClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/implementation` request.
		 *
		 * @since 3.6.0
		 */
		//implementation?: ImplementationClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/references` request.
		 */
		//references?: ReferenceClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/documentHighlight` request.
		 */
		//documentHighlight?: DocumentHighlightClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/documentSymbol` request.
		 */
		//documentSymbol?: DocumentSymbolClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/codeAction` request.
		 */
		//codeAction?: CodeActionClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/codeLens` request.
		 */
		//codeLens?: CodeLensClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/documentLink` request.
		 */
		//documentLink?: DocumentLinkClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/documentColor` and the
		 * `textDocument/colorPresentation` request.
		 *
		 * @since 3.6.0
		 */
		//colorProvider?: DocumentColorClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/formatting` request.
		 */
		//formatting?: DocumentFormattingClientCapabilities

		/**
		 * Capabilities specific to the `textDocument/rangeFormatting` request.
		 */
		//rangeFormatting?: DocumentRangeFormattingClientCapabilities;

		/** request.
		 * Capabilities specific to the `textDocument/onTypeFormatting` request.
		 */
		//onTypeFormatting?: DocumentOnTypeFormattingClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/rename` request.
		 */
		//rename?: RenameClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/publishDiagnostics`
		 * notification.
		 */
		//publishDiagnostics?: PublishDiagnosticsClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/foldingRange` request.
		 *
		 * @since 3.10.0
		 */
		//foldingRange?: FoldingRangeClientCapabilities;

		/**
		 * Capabilities specific to the `textDocument/selectionRange` request.
		 *
		 * @since 3.15.0
		 */
		//selectionRange?: SelectionRangeClientCapabilities;
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

	/**
	 * Describes the content type that a client supports in various
	 * result literals like `Hover`, `ParameterInfo` or `CompletionItem`.
	 *
	 * Please note that `MarkupKinds` must not start with a `$`. This kinds
	 * are reserved for internal usage.
	 */
	class MarkupKind
	{
		/**
			* Plain text is supported as a content format
			*/
		public const string PlainText = "plaintext";

		/**
		 * Markdown is supported as a content format
		 */
		public const string Markdown = "markdown";
	}


	/**
	 * The kind of a completion entry.
	 */
	enum CompletionItemKind
	{
		Text = 1,
		Method = 2,
		Function = 3,
		Constructor = 4,
		Field = 5,
		Variable = 6,
		Class = 7,
		Interface = 8,
		Module = 9,
		Property = 10,
		Unit = 11,
		Value = 12,
		Enum = 13,
		Keyword = 14,
		Snippet = 15,
		Color = 16,
		File = 17,
		Reference = 18,
		Folder = 19,
		EnumMember = 20,
		Constant = 21,
		Struct = 22,
		Event = 23,
		Operator = 24,
		TypeParameter = 25,
	}

	class CompletionClientCapabilities
	{
		/**
		 * Whether completion supports dynamic registration.
		 */
		public bool dynamicRegistration;

		/**
		 * The client supports the following `CompletionItem` specific
		 * capabilities.
		 */
		public class _completionItem {
			/**
			 * Client supports snippets as insert text.
			 *
			 * A snippet can define tab stops and placeholders with `$1`, `$2`
			 * and `${3:foo}`. `$0` defines the final tab stop, it defaults to
			 * the end of the snippet.
			 * Placeholders with equal identifiers are linked, so that typing in
			 * one will update others as well.
			 */
			public bool snippetSupport;

			/**
			 * Client supports commit characters on a completion item.
			 */
			public bool commitCharactersSupport;

			/**
			 * Client supports the follow content formats for the documentation
			 * property. The order describes the preferred format of the client.
			 */
			public string /*MarkupKind*/[] documentationFormat;

			/**
			 * Client supports the deprecated property on a completion item.
			 */
			public bool deprecatedSupport;

			/**
			 * Client supports the preselect property on a completion item.
			 */
			public bool preselectSupport;
#if false
			/**
			 * Client supports the tag property on a completion item.
			 * Clients supporting tags have to handle unknown tags gracefully.
			 * Clients especially need to preserve unknown tags when sending
			 * a completion item back to the server in a resolve call.
			 *
			 * @since 3.15.0
			 */
			tagSupport?: {
				/**
				 * The tags supported by the client.
				 */
				valueSet: CompletionItemTag[]
			}
#endif
		}
		public _completionItem	completionItem=new _completionItem();

		public class _completionItemKind {
			/**
			 * The completion item kind values the client supports. When this
			 * property exists the client also guarantees that it will
			 * handle values outside its set gracefully and falls back
			 * to a default value when unknown.
			 *
			 * If this property is not present the client only supports
			 * the completion items kinds from `Text` to `Reference` as defined in
			 * the initial version of the protocol.
			 */
			public CompletionItemKind[] valueSet;
		}
		public _completionItemKind completionItemKind = new _completionItemKind();

		/**
		 * The client supports to send additional context information for a
		 * `textDocument/completion` request.
		 */
		public bool contextSupport;
	}
}
