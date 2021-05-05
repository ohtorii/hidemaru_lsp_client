using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
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
}


