using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
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
		public class _completionItem
		{
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
			public MarkupKind[] documentationFormat;

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
		public _completionItem completionItem = new _completionItem();

		public class _completionItemKind
		{
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

	interface ICompletionOptions : IWorkDoneProgressOptions
	{		
		string[] triggerCharacters { get; set; }		
		string[] allCommitCharacters { get; set; }
		bool resolveProvider { get; set; }
	}
	class CompletionOptions : ICompletionOptions
	{
		public string[] triggerCharacters { get; set; }
		public string[] allCommitCharacters { get; set; }
		public bool resolveProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
	interface ICompletionRegistrationOptions : ITextDocumentRegistrationOptions, ICompletionOptions {
	}

	interface ICompletionParams :ITextDocumentPositionParams,IWorkDoneProgressParams, IPartialResultParams {
		ICompletionContext context { get; set; }
	}
	enum CompletionTriggerKind
	{		
		Invoked=1,
		TriggerCharacter=2,
		TriggerForIncompleteCompletions = 3,
	}	
	interface ICompletionContext{

		CompletionTriggerKind triggerKind { get; set; }
		string triggerCharacter { get; set; }
	}
	class CompletionContext : ICompletionContext
	{
		public CompletionTriggerKind triggerKind { get; set; }
		public string triggerCharacter { get; set; }
	}
	class CompletionParams : ICompletionParams
	{
		public ICompletionContext context { get; set; } //= new CompletionContext();
		public ITextDocumentIdentifier textDocument { get; set; } = new TextDocumentIdentifier();
		public IPosition position { get; set; } = new Position();
		[JsonIgnore]
		public short workDoneToken { get; set; }
		public string partialResultToken { get; set; }
	}
}
