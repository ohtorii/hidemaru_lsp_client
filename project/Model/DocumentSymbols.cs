using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class DocumentSymbolClientCapabilities
	{
		/**
		 * Whether document symbol supports dynamic registration.
		 */
		public bool dynamicRegistration;

		/**
		 * Specific capabilities for the `SymbolKind` in the
		 * `textDocument/documentSymbol` request.
		 */
		public class _symbolKind { 
			/**
			 * The symbol kind values the client supports. When this
			 * property exists the client also guarantees that it will
			 * handle values outside its set gracefully and falls back
			 * to a default value when unknown.
			 *
			 * If this property is not present the client only supports
			 * the symbol kinds from `File` to `Array` as defined in
			 * the initial version of the protocol.
			 */
			public SymbolKind[] valueSet;
		}
		public _symbolKind symbolKind;

		/**
		 * The client supports hierarchical document symbols.
		 */
		public bool hierarchicalDocumentSymbolSupport;

		/**
		 * The client supports tags on `SymbolInformation`. Tags are supported on
		 * `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set to true.
		 * Clients supporting tags have to handle unknown tags gracefully.
		 *
		 * @since 3.16.0
		 */
		public class _tagSupport {
			/**
			 * The tags supported by the client.
			 */
			public SymbolTag[]  valueSet;
		};
		public _tagSupport tagSupport;
		/**
		 * The client supports an additional label presented in the UI when
		 * registering a document symbol provider.
		 *
		 * @since 3.16.0
		 */
		public bool labelSupport ;
	}
	interface IDocumentSymbolOptions : IWorkDoneProgressOptions
	{
		/**
	 * A human-readable string that is shown when multiple outlines trees
	 * are shown for the same document.
	 *
	 * @since 3.16.0
	 */
		string label { get; set; }
	}
	interface IDocumentSymbolRegistrationOptions:ITextDocumentRegistrationOptions, IDocumentSymbolOptions {
	}

	class DocumentSymbolOptions : IDocumentSymbolOptions
	{
		public bool workDoneProgress { get; set; }
		public string label { get; set; }
	}
	class DocumentSymbolRegistrationOptions : IDocumentSymbolRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
		public string label { get; set; }
	}
	interface IDocumentSymbolParams : IWorkDoneProgressParams,IPartialResultParams {
		/**
		 * The text document.
		 */
		TextDocumentIdentifier textDocument { get; set; }
	}



//[JsonConverter(typeof(NumberEnumConverter))]
public enum SymbolKind
	{
		File = 1,
		Module = 2,
		Namespace = 3,
		Package = 4,
		Class = 5,
		Method = 6,
		Property = 7,
		Field = 8,
		Constructor = 9,
		Enum = 10,
		Interface = 11,
		Function = 12,
		Variable = 13,
		Constant = 14,
		String = 15,
		Number = 16,
		Boolean = 17,
		Array = 18,
		Object = 19,
		Key = 20,
		Null = 21,
		EnumMember = 22,
		Struct = 23,
		Event = 24,
		Operator = 25,
		TypeParameter = 26
	}


	//[JsonConverter(typeof(NumberEnumConverter))]
	public enum SymbolTag
	{
		Deprecated = 1
	}

	class DocumentSymbol
	{
		public string name;
		public string detail;
		public SymbolKind kind;		
		public SymbolTag[]  tags;
		public bool deprecated;
		public Range range;
		public Range selectionRange;
		public DocumentSymbol[] children;
	}

	/**
	 * Represents information about programming constructs like variables, classes,
	 * interfaces etc.
	 */
	class SymbolInformation
	{
		/**
		 * The name of this symbol.
		 */
		public string name;

		/**
		 * The kind of this symbol.
		 */
		public SymbolKind kind;

		/**
		 * Tags for this symbol.
		 *
		 * @since 3.16.0
		 */
		public SymbolTag[]  tags;

		/**
		 * Indicates if this symbol is deprecated.
		 *
		 * @deprecated Use tags instead
		 */
		public bool deprecated;

		/**
		 * The location of this symbol. The location's range is used by a tool
		 * to reveal the location in the editor. If the symbol is selected in the
		 * tool the range's start information is used to position the cursor. So
		 * the range usually spans more then the actual symbol's name and does
		 * normally include things like visibility modifiers.
		 *
		 * The range doesn't have to denote a node range in the sense of a abstract
		 * syntax tree. It can therefore not be used to re-construct a hierarchy of
		 * the symbols.
		 */
		public Location location;

		/**
		 * The name of the symbol containing this symbol. This information is for
		 * user interface purposes (e.g. to render a qualifier in the user interface
		 * if necessary). It can't be used to re-infer a hierarchy for the document
		 * symbols.
		 */
		public string containerName;
	}
}
