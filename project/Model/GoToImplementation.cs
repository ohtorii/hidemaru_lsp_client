using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IImplementationOptions : IWorkDoneProgressOptions
	{
	}

	class ImplementationRegistrationOptions : ITextDocumentRegistrationOptions, IImplementationOptions, IStaticRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
		public string id { get; set; }
	}
	
}
