using System;
using System.Collections.Generic;
using System.Text;

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
	interface IDeclarationRegistrationOptions : IDeclarationOptions,ITextDocumentRegistrationOptions, IStaticRegistrationOptions {
	}
	interface IDeclarationParams : ITextDocumentPositionParams,IWorkDoneProgressParams, IPartialResultParams {
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
		public ITextDocumentIdentifier textDocument { get; set; }
		public IPosition position { get; set; }
		public string workDoneToken { get; set; }
		public string partialResultToken { get; set; }
	}
}
