using Lsp.Model.Serialization.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{	

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

			/**
			 * Client supports the tag property on a completion item.
			 * Clients supporting tags have to handle unknown tags gracefully.
			 * Clients especially need to preserve unknown tags when sending
			 * a completion item back to the server in a resolve call.
			 *
			 * @since 3.15.0
			 */
			public class _tagSupport {
				/**
				 * The tags supported by the client.
				 */
				public CompletionItemTag[] valueSet;
			}
			public _tagSupport tagSupport{ 
				get 
				{
					if (m_tagSupport == null)
					{
						m_tagSupport = new _tagSupport();
					}
					return m_tagSupport;
				}
				set 
				{
					m_tagSupport = value;
				} 
			}
			[JsonIgnore] _tagSupport m_tagSupport = null;
			/**
			 * Client supports insert replace edit to control different behavior if
			 * a completion item is inserted in the text or should replace text.
			 *
			 * @since 3.16.0
			 */
			public bool insertReplaceSupport;

			/**
			 * Indicates which properties a client can resolve lazily on a
			 * completion item. Before version 3.16.0 only the predefined properties
			 * `documentation` and `detail` could be resolved lazily.
			 *
			 * @since 3.16.0
			 */
			public class _resolveSupport {
				/**
				 * The properties that a client can resolve lazily.
				 */
				public string[] properties;
			};
			public _resolveSupport resolveSupport
			{
				get
				{
					if (m_resolveSupport == null)
					{
						m_resolveSupport = new _resolveSupport();
					}
					return m_resolveSupport;
				}
			}
			[JsonIgnore] _resolveSupport m_resolveSupport;

			/**
			 * The client supports the `insertTextMode` property on
			 * a completion item to override the whitespace handling mode
			 * as defined by the client.
			 *
			 * @since 3.16.0
			 */
			public class _insertTextModeSupport {
				public InsertTextMode[]  valueSet;
			};
			public _insertTextModeSupport insertTextModeSupport {
				get {
					if (m_insertTextModeSupport == null)
					{
						m_insertTextModeSupport = new _insertTextModeSupport();
					}
					return m_insertTextModeSupport;
				} 
				set {
					m_insertTextModeSupport = value;
				} 
			}
			[JsonIgnore] _insertTextModeSupport m_insertTextModeSupport = null;
		}

		public _completionItem completionItem
		{
			get
			{
				if (m_completionItem == null)
				{
					m_completionItem = new _completionItem();
				}
				return m_completionItem;
			}
		}
		[JsonIgnore] _completionItem m_completionItem = null;

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
		public _completionItemKind completionItemKind
		{
			get
			{
				if (m_completionItemKind == null)
				{
					m_completionItemKind = new _completionItemKind();
				}
				return m_completionItemKind;
			}
		}
		[JsonIgnore] _completionItemKind m_completionItemKind=null;

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
		public ICompletionContext context {
			get
			{
				if (m_context == null)
				{
					m_context = new CompletionContext();
				}
				return m_context;
			}
			set
			{
				m_context = (CompletionContext)value;
			}
		} 
		public ITextDocumentIdentifier textDocument {
			get
			{
				if (m_textDocument == null)
				{
					m_textDocument = new TextDocumentIdentifier();
				}
				return m_textDocument;
			} 
			set
			{
				m_textDocument = (TextDocumentIdentifier)value;
			}
		}
		public IPosition position { 
			get 
			{
				if (m_position == null)
				{
					m_position = new Position();
				}
				return m_position;
			} 
			set 
			{
				m_position = (Position)value;
			} 
		}
		
		public string workDoneToken { get; set; } = null;
		public string partialResultToken { get; set; } = null;
		[JsonIgnore]	CompletionContext m_context = null;
		[JsonIgnore]	TextDocumentIdentifier m_textDocument = null;
		[JsonIgnore]	Position m_position = null;
	}

	/**
	 * Represents a collection of [completion items](#CompletionItem) to be
	 * presented in the editor.
	 */
	class CompletionList {
		/**
		 * This list is not complete. Further typing should result in recomputing
		 * this list.
		 */
		public bool isIncomplete=false;

		/**
		 * The completion items.
		 */
		public CompletionItem[]  items=null;
	}

	/**
	 * Defines whether the insert text in a completion item should be interpreted as
	 * plain text or a snippet.
	 */
	enum InsertTextFormat {
		/**
		 * The primary text to be inserted is treated as a plain string.
		 */
		PlainText = 1,

		/**
		 * The primary text to be inserted is treated as a snippet.
		 *
		 * A snippet can define tab stops and placeholders with `$1`, `$2`
		 * and `${3:foo}`. `$0` defines the final tab stop, it defaults to
		 * the end of the snippet. Placeholders with equal identifiers are linked,
		 * that is typing in one will update others too.
		 */
		Snippet =2,
	}	

	/**
	 * Completion item tags are extra annotations that tweak the rendering of a
	 * completion item.
	 *
	 * @since 3.15.0
	 */
	[JsonConverter(typeof(NumberEnumConverter))]
	public enum CompletionItemTag
	{
		/// <summary>
		/// Render a completion as obsolete, usually using a strike-out.
		/// </summary>
		Deprecated = 1
	}

	/**
	 * A special text edit to provide an insert and a replace operation.
	 *
	 * @since 3.16.0
	 */
	class InsertReplaceEdit {
		/**
		 * The string to be inserted.
		 */
		public string newText=null;

		/**
		 * The range if the insert is requested
		 */
		public Range insert = null;

		/**
		 * The range if the replace is requested.
		 */
		public Range replace = null;
	}

	/**
	 * How whitespace and indentation is handled during completion
	 * item insertion.
	 *
	 * @since 3.16.0
	 */
	enum InsertTextMode
	{
		/**
		 * The insertion or replace strings is taken as it is. If the
		 * value is multi line the lines below the cursor will be
		 * inserted using the indentation defined in the string value.
		 * The client will not apply any kind of adjustments to the
		 * string.
		 */
		asIs=1,

		/**
		 * The editor adjusts leading whitespace of new lines so that
		 * they match the indentation up to the cursor of the line for
		 * which the item is accepted.
		 *
		 * Consider a line like this: <2tabs><cursor><3tabs>foo. Accepting a
		 * multi line completion item is indented using 2 tabs and all
		 * following lines inserted will be indented using 2 tabs as well.
		 */
		adjustIndentation= 2,
	}
	
	class CompletionItem {
		/**
		 * The label of this completion item. By default
		 * also the text that is inserted when selecting
		 * this completion.
		 */
		public string label=null;

		/**
		 * The kind of this completion item. Based of the kind
		 * an icon is chosen by the editor. The standardized set
		 * of available values is defined in `CompletionItemKind`.
		 */
		public CompletionItemKind kind;

		/**
		 * Tags for this completion item.
		 *
		 * @since 3.15.0
		 */
		public CompletionItemTag[] tags=null;

		/**
		 * A human-readable string with additional information
		 * about this item, like type or symbol information.
		 */
		public string detail=null;

		/**
		 * A human-readable string that represents a doc-comment.
		 */
		public StringOrMarkupContent documentation=null;

		/**
		 * Indicates if this item is deprecated.
		 *
		 * @deprecated Use `tags` instead if supported.
		 */
		public bool deprecated=false;

		/**
		 * Select this item when showing.
		 *
		 * *Note* that only one completion item can be selected and that the
		 * tool / client decides which item that is. The rule is that the *first*
		 * item of those that match best is selected.
		 */
		public bool preselect=false;

		/**
		 * A string that should be used when comparing this item
		 * with other items. When `falsy` the label is used
		 * as the sort text for this item.
		 */
		public string sortText=null;

		/**
		 * A string that should be used when filtering a set of
		 * completion items. When `falsy` the label is used as the
		 * filter text for this item.
		 */
		public string filterText=null;

		/**
		 * A string that should be inserted into a document when selecting
		 * this completion. When `falsy` the label is used as the insert text
		 * for this item.
		 *
		 * The `insertText` is subject to interpretation by the client side.
		 * Some tools might not take the string literally. For example
		 * VS Code when code complete is requested in this example
		 * `con<cursor position>` and a completion item with an `insertText` of
		 * `console` is provided it will only insert `sole`. Therefore it is
		 * recommended to use `textEdit` instead since it avoids additional client
		 * side interpretation.
		 */
		public string insertText=null;

		/**
		 * The format of the insert text. The format applies to both the
		 * `insertText` property and the `newText` property of a provided
		 * `textEdit`. If omitted defaults to `InsertTextFormat.PlainText`.
		 */
		public InsertTextFormat insertTextFormat;

		/**
		 * How whitespace and indentation is handled during completion
		 * item insertion. If not provided the client's default value is used.
		 *
		 * @since 3.16.0
		 */
		public InsertTextMode insertTextMode;

		/**
		 * An edit which is applied to a document when selecting this completion.
		 * When an edit is provided the value of `insertText` is ignored.
		 *
		 * *Note:* The range of the edit must be a single line range and it must
		 * contain the position at which completion has been requested.
		 *
		 * Most editors support two different operations when accepting a completion
		 * item. One is to insert a completion text and the other is to replace an
		 * existing text with a completion text. Since this can usually not be
		 * predetermined by a server it can report both ranges. Clients need to
		 * signal support for `InsertReplaceEdit`s via the
		 * `textDocument.completion.insertReplaceSupport` client capability
		 * property.
		 *
		 * *Note 1:* The text edit's range as well as both ranges from an insert
		 * replace edit must be a [single line] and they must contain the position
		 * at which completion has been requested.
		 * *Note 2:* If an `InsertReplaceEdit` is returned the edit's insert range
		 * must be a prefix of the edit's replace range, that means it must be
		 * contained and starting at the same position.
		 *
		 * @since 3.16.0 additional type `InsertReplaceEdit`
		 */
		public TextEditOrInsertReplaceEdit textEdit =null;

		/**
		 * An optional array of additional text edits that are applied when
		 * selecting this completion. Edits must not overlap (including the same
		 * insert position) with the main edit nor with themselves.
		 *
		 * Additional text edits should be used to change text unrelated to the
		 * current cursor position (for example adding an import statement at the
		 * top of the file if the completion item will insert an unqualified type).
		 */
		public TextEdit[] additionalTextEdits=null;

		/**
		 * An optional set of characters that when pressed while this completion is
		 * active will accept it first and then type that character. *Note* that all
		 * commit characters should have `length=1` and that superfluous characters
		 * will be ignored.
		 */
		public string[] commitCharacters=null;

		/**
		 * An optional command that is executed *after* inserting this completion.
		 * *Note* that additional modifications to the current document should be
		 * described with the additionalTextEdits-property.
		 */
		public Command command=null;

		/**
		 * A data entry field that is preserved on a completion item between
		 * a completion and a completion resolve request.
		 */
		public /*any*/ JToken data =null;
	}

	/**
	 * The kind of a completion entry.
	 */
	enum CompletionItemKind {
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

	
	
}
