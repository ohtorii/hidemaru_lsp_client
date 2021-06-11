using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class DocumentOnTypeFormattingClientCapabilities
	{
		/**
		 * Whether on type formatting supports dynamic registration.
		 */
		public bool dynamicRegistration;
	}
	interface IDocumentOnTypeFormattingOptions
	{		
		string firstTriggerCharacter { get; set; }
		string[]  moreTriggerCharacter { get; set; }
	}

	interface IDocumentOnTypeFormattingRegistrationOptions:ITextDocumentRegistrationOptions, IDocumentOnTypeFormattingOptions {

	}

	class DocumentOnTypeFormattingOptions : IDocumentOnTypeFormattingOptions
	{
		public string firstTriggerCharacter { get; set; }
		public string[] moreTriggerCharacter { get; set; }
	}
	class DocumentOnTypeFormattingRegistrationOptions : IDocumentOnTypeFormattingRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public string firstTriggerCharacter { get; set; }
		public string[] moreTriggerCharacter { get; set; }
	}
#if false
	interface DocumentOnTypeFormattingParams extends TextDocumentPositionParams {
		/**
		 * The character that has been typed.
		 */
		ch: string;

		/**
		 * The format options.
		 */
		options: FormattingOptions;
	}
#endif
}
