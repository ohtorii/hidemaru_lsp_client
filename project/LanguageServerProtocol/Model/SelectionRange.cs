namespace LSP.Model
{
    class SelectionRangeClientCapabilities
    {
        /**
		 * Whether implementation supports dynamic registration for selection range
		 * providers. If this is set to `true` the client supports the new
		 * `SelectionRangeRegistrationOptions` return value for the corresponding
		 * server capability as well.
		 */
        public bool dynamicRegistration;
    }
    interface ISelectionRangeOptions : IWorkDoneProgressOptions
    {
    }
    interface ISelectionRangeRegistrationOptions : ISelectionRangeOptions, ITextDocumentRegistrationOptions, IStaticRegistrationOptions
    {
    }

    interface ISelectionRangeParams : IWorkDoneProgressParams, IPartialResultParams
    {
        /**
		 * The text document.
		 */
        ITextDocumentIdentifier textDocument { get; set; }

        /**
		 * The positions inside the text document.
		 */
        IPosition[] positions { get; set; }
    }

    interface ISelectionRange
    {
        /**
		 * The [range](#Range) of this selection range.
		 */
        IRange range { get; set; }
        /**
		 * The parent selection range containing this range. Therefore
		 * `parent.range` must contain `this.range`.
		 */
        ISelectionRange parent { get; set; }
    }


    class SelectionRangeRegistrationOptions : ISelectionRangeRegistrationOptions
    {
        public bool workDoneProgress { get; set; }
        public DocumentFilter[] documentSelector { get; set; }
        public string id { get; set; }
    }
}
