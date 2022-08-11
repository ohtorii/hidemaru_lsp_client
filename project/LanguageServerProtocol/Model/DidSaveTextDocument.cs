namespace LSP.Model
{
    interface ISaveOptions
    {
        /**
         * The client is supposed to include the content on save.
         */
        bool includeText { set; get; }
    }
    interface ITextDocumentSaveRegistrationOptions : ITextDocumentRegistrationOptions
    {
        /**
         * The client is supposed to include the content on save.
         */
        bool includeText { set; get; }
    }

    interface IDidSaveTextDocumentParams
    {
        /**
         * The document that was saved.
         */
        ITextDocumentIdentifier textDocument { set; get; }

        /**
	     * Optional the content when saved. Depends on the includeText value
	     * when the save notification was requested.
	     */
        string text { set; get; }
    }

    class DidSaveTextDocumentParams : IDidSaveTextDocumentParams
    {
        public ITextDocumentIdentifier textDocument { get; set; } = new TextDocumentIdentifier();
        public string text { get; set; }
    }
}
