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
