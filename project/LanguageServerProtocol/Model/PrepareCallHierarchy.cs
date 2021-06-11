using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface ICallHierarchyClientCapabilities
	{
		/**
		 * Whether implementation supports dynamic registration. If this is set to
		 * `true` the client supports the new `(TextDocumentRegistrationOptions &
		 * StaticRegistrationOptions)` return value for the corresponding server
		 * capability as well.
		 */
		bool dynamicRegistration { get; set; }
	}
	interface ICallHierarchyOptions : IWorkDoneProgressOptions
	{
	}
	interface ICallHierarchyRegistrationOptions :ITextDocumentRegistrationOptions, ICallHierarchyOptions,IStaticRegistrationOptions 
	{
	}
	interface ICallHierarchyPrepareParams : ITextDocumentPositionParams,IWorkDoneProgressParams 
	{
	}

#if false
export interface CallHierarchyItem {
	/**
	 * The name of this item.
	 */
	name: string;

	/**
	 * The kind of this item.
	 */
	kind: SymbolKind;

	/**
	 * Tags for this item.
	 */
	tags?: SymbolTag[];

	/**
	 * More detail for this item, e.g. the signature of a function.
	 */
	detail?: string;

	/**
	 * The resource identifier of this item.
	 */
	uri: DocumentUri;

	/**
	 * The range enclosing this symbol not including leading/trailing whitespace
	 * but everything else, e.g. comments and code.
	 */
	range: Range;

	/**
	 * The range that should be selected and revealed when this symbol is being
	 * picked, e.g. the name of a function. Must be contained by the
	 * [`range`](#CallHierarchyItem.range).
	 */
	selectionRange: Range;

	/**
	 * A data entry field that is preserved between a call hierarchy prepare and
	 * incoming calls or outgoing calls requests.
	 */
	data?: unknown;
}
#endif


	class CallHierarchyRegistrationOptions : ICallHierarchyRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
		public string id { get; set; }
	}
}
