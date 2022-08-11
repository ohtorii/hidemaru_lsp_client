using Newtonsoft.Json;


namespace LSP.Model
{
    class DeclarationClientCapabilities
    {
        /**
		 * Whether declaration supports dynamic registration. If this is set to
		 * `true` the client supports the new `DeclarationRegistrationOptions`
		 * return value for the corresponding server capability as well.
		 */
        public bool dynamicRegistration;

        /**
		 * The client supports additional metadata in the form of declaration links.
		 */
        public bool linkSupport;
    }
    interface IDeclarationOptions : IWorkDoneProgressOptions
    {
    }
    interface IDeclarationRegistrationOptions : IDeclarationOptions, ITextDocumentRegistrationOptions, IStaticRegistrationOptions
    {
    }
    interface IDeclarationParams : ITextDocumentPositionParams, IWorkDoneProgressParams, IPartialResultParams
    {
    }

    class DeclarationOptions : IDeclarationOptions
    {
        public bool workDoneProgress { get; set; }
    }
    class DeclarationRegistrationOptions : IDeclarationRegistrationOptions
    {
        public bool workDoneProgress { get; set; }
        public DocumentFilter[] documentSelector { get; set; }
        public string id { get; set; }
    }
    class DeclarationParams : IDeclarationParams
    {
        public ITextDocumentIdentifier textDocument
        {
            get
            {
                if (m_textDocumentIdentifier == null)
                {
                    m_textDocumentIdentifier = new TextDocumentIdentifier();

                }
                return m_textDocumentIdentifier;

            }
        }
        public IPosition position
        {
            get
            {
                if (m_position == null)
                {
                    m_position = new Position();
                }
                return m_position;
            }
        }
        public ProgressToken workDoneToken { get; set; }
        public ProgressToken partialResultToken { get; set; }

        [JsonIgnore] TextDocumentIdentifier m_textDocumentIdentifier;
        [JsonIgnore] Position m_position;
    }
}
