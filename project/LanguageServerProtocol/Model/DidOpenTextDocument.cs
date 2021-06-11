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

	class TextDocumentItem : ITextDocumentItem
	{
		public string uri { get; set; }
		public string languageId { get; set; }
		public int version { get; set; }
		public string text { get; set; }
	}
	class DidOpenTextDocumentParams : IDidOpenTextDocumentParams
	{
		public DidOpenTextDocumentParams()
		{
			this.textDocument = new TextDocumentItem();
		}
		public ITextDocumentItem textDocument { get; set; }
	}
}
