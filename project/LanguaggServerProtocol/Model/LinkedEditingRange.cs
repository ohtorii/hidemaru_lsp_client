using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface ILinkedEditingRangeClientCapabilities
	{
		/**
		 * Whether implementation supports dynamic registration.
		 * If this is set to `true` the client supports the new
		 * `(TextDocumentRegistrationOptions & StaticRegistrationOptions)`
		 * return value for the corresponding server capability as well.
		 */
		bool dynamicRegistration { get; set; }
	}

	interface ILinkedEditingRangeOptions : IWorkDoneProgressOptions
	{
	}
	interface ILinkedEditingRangeRegistrationOptions :ITextDocumentRegistrationOptions, ILinkedEditingRangeOptions,IStaticRegistrationOptions 
	{
	}

	interface ILinkedEditingRangeParams :ITextDocumentPositionParams,IWorkDoneProgressParams 
	{
	}

	interface ILinkedEditingRanges
	{
		/**
		 * A list of ranges that can be renamed together. The ranges must have
		 * identical length and contain identical text content. The ranges cannot overlap.
		 */
		Range[]  ranges {get;set;}

		/**
		 * An optional word pattern (regular expression) that describes valid contents for
		 * the given ranges. If no pattern is provided, the client configuration's word
		 * pattern will be used.
		 */
		string wordPattern { get; set; }
	}


	class LinkedEditingRangeRegistrationOptions : ILinkedEditingRangeRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
		public string id { get; set; }
	}
}
