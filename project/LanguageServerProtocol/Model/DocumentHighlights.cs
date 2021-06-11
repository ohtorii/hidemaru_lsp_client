using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class DocumentHighlightClientCapabilities
	{
		/**
		 * Whether document highlight supports dynamic registration.
		 */
		public bool dynamicRegistration;
	}
	interface IDocumentHighlightOptions :IWorkDoneProgressOptions
	{
	}
	interface IDocumentHighlightRegistrationOptions :ITextDocumentRegistrationOptions, IDocumentHighlightOptions {
	}
	interface IDocumentHighlightParams : ITextDocumentPositionParams,IWorkDoneProgressParams, IPartialResultParams {
	}
	/**
	 * A document highlight is a range inside a text document which deserves
	 * special attention. Usually a document highlight is visualized by changing
	 * the background color of its range.
	 *
	 */
	interface DocumentHighlight
	{
		/**
		 * The range this highlight applies to.
		 */
		IRange range { get; set; }

		/**
		 * The highlight kind, default is DocumentHighlightKind.Text.
		 */
		DocumentHighlightKind kind { get; set; }
	}

	/**
	 * A document highlight kind.
	 */
	enum DocumentHighlightKind
	{
		/**
		 * A textual occurrence.
		 */
		Text = 1,

		/**
		 * Read-access of a symbol, like reading a variable.
		 */
		Read = 2,

		/**
		 * Write-access of a symbol, like writing to a variable.
		 */
		Write = 3,
	}


	class DocumentHighlightOptions : IDocumentHighlightOptions
	{
		public bool workDoneProgress { get; set; }
	}
}
