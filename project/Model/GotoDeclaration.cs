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

}
