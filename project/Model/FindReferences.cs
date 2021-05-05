using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IReferenceOptions : IWorkDoneProgressOptions
	{
	}
	interface  IReferenceRegistrationOptions: ITextDocumentRegistrationOptions, IReferenceOptions {
	}
	class ReferenceOptions : IReferenceOptions
	{
		public bool workDoneProgress { get; set; }
	}
	class ReferenceRegistrationOptions : IReferenceRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
	}
}
