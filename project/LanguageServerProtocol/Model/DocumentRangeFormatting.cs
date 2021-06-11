using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class DocumentRangeFormattingClientCapabilities
	{
		/**
		 * Whether formatting supports dynamic registration.
		 */
		public bool dynamicRegistration;
	}
	interface IDocumentRangeFormattingOptions:IWorkDoneProgressOptions
	{
	}

	interface IDocumentRangeFormattingRegistrationOptions :ITextDocumentRegistrationOptions, IDocumentRangeFormattingOptions {

	}

	class DocumentRangeFormattingOptions : IDocumentRangeFormattingOptions
	{
		public bool workDoneProgress { get; set; }
	}
	class DocumentRangeFormattingRegistrationOptions : IDocumentRangeFormattingRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
	}
#if false
	interface DocumentRangeFormattingParams extends WorkDoneProgressParams {
		/**
		 * The document to format.
		 */
		textDocument: TextDocumentIdentifier;

		/**
		 * The range to format
		 */
		range: Range;

		/**
		 * The format options
		 */
		options: FormattingOptions;
	}
#endif
}


