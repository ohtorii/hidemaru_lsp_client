using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
    interface IDidCloseTextDocumentParams
    {
        /**
         * The document that was closed.
         */
        TextDocumentIdentifier textDocument { get; set; }
    }

    class DidCloseTextDocumentParams : IDidCloseTextDocumentParams
    {
        public TextDocumentIdentifier textDocument { get; set; } = new TextDocumentIdentifier();        
    }


}
