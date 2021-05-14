using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class CodeLensClientCapabilities
	{
		/**
		 * Whether code lens supports dynamic registration.
		 */
		public bool dynamicRegistration;
	}
	interface ICodeLensOptions : IWorkDoneProgressOptions
	{		
		bool resolveProvider { get; set; }
	}
	interface ICodeLensRegistrationOptions:ITextDocumentRegistrationOptions, ICodeLensOptions {
	}

	class CodeLensOptions : ICodeLensOptions
	{
		public bool resolveProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
	class CodeLensRegistrationOptions : ICodeLensRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool resolveProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
#if false
	interface CodeLensParams extends WorkDoneProgressParams, PartialResultParams {
		/**
		 * The document to request code lens for.
		 */
		textDocument: TextDocumentIdentifier;
	}

	/**
	 * A code lens represents a command that should be shown along with
	 * source text, like the number of references, a way to run tests, etc.
	 *
	 * A code lens is _unresolved_ when no command is associated to it. For
	 * performance reasons the creation of a code lens and resolving should be done
	 * in two stages.
	 */
	interface CodeLens {
		/**
		 * The range in which this code lens is valid. Should only span a single
		 * line.
		 */
		range: Range;

		/**
		 * The command this code lens represents.
		 */
		command?: Command;

		/**
		 * A data entry field that is preserved on a code lens item between
		 * a code lens and a code lens resolve request.
		 */
		data?: any;
	}
#endif
}
