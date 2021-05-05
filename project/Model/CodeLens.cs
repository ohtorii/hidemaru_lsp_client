using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface ICodeLensOptions : IWorkDoneProgressOptions
	{		
		bool resolveProvider { get; set; }
	}
	interface ICodeLensRegistrationOptions:ITextDocumentRegistrationOptions, ICodeLensOptions {
	}

	class CodeLensOptions : ICodeLensOptions
	{
		public bool resolveProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
	class CodeLensRegistrationOptions : ICodeLensRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool resolveProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
}
