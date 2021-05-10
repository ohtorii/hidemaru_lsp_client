using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class  DocumentFormattingClientCapabilities
	{
		/**
		 * Whether formatting supports dynamic registration.
		 */
		public bool dynamicRegistration;
	}

	interface IDocumentFormattingOptions : IWorkDoneProgressOptions
	{
	}
	interface IDocumentFormattingRegistrationOptions : ITextDocumentRegistrationOptions, IDocumentFormattingOptions {
	}

	class DocumentFormattingOptions : IDocumentFormattingOptions
	{
		public bool workDoneProgress { get; set; }
	}
	class DocumentFormattingRegistrationOptions : IDocumentFormattingRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
	}
#if false
interface DocumentFormattingParams extends WorkDoneProgressParams {
	/**
	 * The document to format.
	 */
	textDocument: TextDocumentIdentifier;

	/**
	 * The format options.
	 */
	options: FormattingOptions;
}

/**
 * Value-object describing what options formatting should use.
 */
interface FormattingOptions {
	/**
	 * Size of a tab in spaces.
	 */
	tabSize: uinteger;

	/**
	 * Prefer spaces over tabs.
	 */
	insertSpaces: boolean;

	/**
	 * Trim trailing whitespace on a line.
	 *
	 * @since 3.15.0
	 */
	trimTrailingWhitespace?: boolean;

	/**
	 * Insert a newline character at the end of the file if one does not exist.
	 *
	 * @since 3.15.0
	 */
	insertFinalNewline?: boolean;

	/**
	 * Trim all newlines after the final newline at the end of the file.
	 *
	 * @since 3.15.0
	 */
	trimFinalNewlines?: boolean;

	/**
	 * Signature for further properties.
	 */
	[key: string]: boolean | integer | string;
}
#endif
}
