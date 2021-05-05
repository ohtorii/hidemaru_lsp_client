using System;
using System.Collections.Generic;
using System.Text;
using CodeActionKind =System.String;

namespace LSP.Model
{
	interface ICodeActionOptions : IWorkDoneProgressOptions
	{
		CodeActionKind[] codeActionKinds  { get;set; }
	}
	interface ICodeActionRegistrationOptions : ITextDocumentRegistrationOptions, ICodeActionOptions {
	}

	class CodeActionOptions : ICodeActionOptions
	{
		public string[] codeActionKinds { get; set; }
		public bool workDoneProgress { get; set; }
	}
	class CodeActionRegistrationOptions : ICodeActionRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public string[] codeActionKinds { get; set; }
		public bool workDoneProgress { get; set; }
	}
}
