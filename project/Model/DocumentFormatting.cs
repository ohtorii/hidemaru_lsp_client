using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IDocumentFormattingOptions : IWorkDoneProgressOptions
	{
	}
	interface IDocumentFormattingRegistrationOptions : ITextDocumentRegistrationOptions, IDocumentFormattingOptions {
	}

	class DocumentFormattingOptions : IDocumentFormattingOptions
	{
		public bool workDoneProgress { get; set; }
	}
	class DocumentFormattingRegistrationOptions : IDocumentFormattingRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
	}
}
