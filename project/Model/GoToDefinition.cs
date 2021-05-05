using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IDefinitionOptions : IWorkDoneProgressOptions
	{
	}
	class DefinitionOptions : IWorkDoneProgressOptions
	{
		public bool workDoneProgress { get; set; } = false;
	}
	class DefinitionRegistrationOptions : ITextDocumentRegistrationOptions, IDefinitionOptions {
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; } = false;
	}
}
