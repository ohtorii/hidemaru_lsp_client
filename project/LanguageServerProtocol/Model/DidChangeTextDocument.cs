using Newtonsoft.Json;

namespace LSP.Model
{
    /**
	 * Describe options to be used when registering for text document change events.
	 */
    interface ITextDocumentChangeRegistrationOptions : ITextDocumentRegistrationOptions
    {
        /**
		 * How documents are synced to the server. See TextDocumentSyncKind.Full
		 * and TextDocumentSyncKind.Incremental.
		 */
        TextDocumentSyncKind syncKind { get; set; }
    }
    interface IDidChangeTextDocumentParams
    {
        /**
		 * The document that did change. The version number points
		 * to the version after all provided content changes have
		 * been applied.
		 */
        IVersionedTextDocumentIdentifier textDocument { get; set; }

        /**
		 * The actual content changes. The content changes describe single state
		 * changes to the document. So if there are two content changes c1 (at
		 * array index 0) and c2 (at array index 1) for a document in state S then
		 * c1 moves the document from S to S' and c2 from S' to S''. So c1 is
		 * computed on the state S and c2 is computed on the state S'.
		 *
		 * To mirror the content of a document using change events use the following
		 * approach:
		 * - start with the same initial content
		 * - apply the 'textDocument/didChange' notifications in the order you
		 *   receive them.
		 * - apply the `TextDocumentContentChangeEvent`s in a single notification
		 *   in the order you receive them.
		 */
        ITextDocumentContentChangeEvent[] contentChanges { get; set; }
    }
    class DidChangeTextDocumentParams : IDidChangeTextDocumentParams
    {
        public IVersionedTextDocumentIdentifier textDocument
        {
            get
            {
                if (m_textDocument == null)
                {
                    m_textDocument = new VersionedTextDocumentIdentifier();
                }
                return m_textDocument;
            }
            set { m_textDocument = (VersionedTextDocumentIdentifier)value; }
        }
        public ITextDocumentContentChangeEvent[] contentChanges { get; set; } = null;

        [JsonIgnore] VersionedTextDocumentIdentifier m_textDocument = null;
    }

    /**
		* An event describing a change to a text document. If range and rangeLength are
		* omitted the new text is considered to be the full content of the document.
		*/
    interface ITextDocumentContentChangeEvent
    {
        /**
		 * The range of the document that changed.
		 */
        Range range { get; set; }

        /**
		 * The optional length of the range that got replaced.
		 *
		 * @deprecated use range instead.
		 */
        //uint rangeLength;

        /**
		 * The new text for the provided range.
		 */
        string text { get; set; }
    };
    class TextDocumentContentChangeEvent : ITextDocumentContentChangeEvent
    {
        public Range range { get; set; } = null;
        public string text { get; set; } = null;
    }

}
