using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IDocumentSymbolOptions : IWorkDoneProgressOptions
	{
	}
	interface IDocumentSymbolRegistrationOptions:ITextDocumentRegistrationOptions, IDocumentSymbolOptions {
	}

	class DocumentSymbolOptions : IDocumentSymbolOptions
	{
		public bool workDoneProgress { get; set; }
	}
	class DocumentSymbolRegistrationOptions : IDocumentSymbolRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
	}
}
