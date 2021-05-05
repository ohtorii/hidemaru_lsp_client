using System;
using System.Collections.Generic;
using System.Text;
using DocumentUri = System.String;

namespace LSP.Model
{

	interface IDidOpenTextDocumentParams
	{
		ITextDocumentItem textDocument { get; set; }
	}

	interface ITextDocumentItem
	{
		DocumentUri uri { get; set; }
		string languageId { get; set; }
		int version { get; set; }
		string text { get; set; }
	}

}
