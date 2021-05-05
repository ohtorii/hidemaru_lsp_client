using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
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
}
