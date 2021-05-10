using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class HoverClientCapabilities
	{
		/**
		 * Whether hover supports dynamic registration.
		 */
		public bool dynamicRegistration;

		/**
		 * Client supports the following content formats if the content
		 * property refers to a `literal of type MarkupContent`.
		 * The order describes the preferred format of the client.
		 */
		public MarkupKind[]  contentFormat;
	}

	interface IHoverOptions : IWorkDoneProgressOptions
	{
	}
	class HoverOptions : IHoverOptions
	{
		public bool workDoneProgress { get; set; }
	}
	interface IHoverRegistrationOptions:ITextDocumentRegistrationOptions, IHoverOptions {
	}

	interface HoverParams : ITextDocumentPositionParams,IWorkDoneProgressParams {
	}

#if false
	/**
	 * The result of a hover request.
	 */
	interface Hover
	{
		/**
		 * The hover's content
		 */
		contents: MarkedString | MarkedString[] | MarkupContent;

		/**
		 * An optional range is a range inside a text document
		 * that is used to visualize a hover, e.g. by changing the background color.
		 */
		range?: Range;
	}
#endif

	
}
