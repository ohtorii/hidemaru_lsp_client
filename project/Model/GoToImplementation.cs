using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class ImplementationClientCapabilities
	{
		/**
		 * Whether implementation supports dynamic registration. If this is set to
		 * `true` the client supports the new `ImplementationRegistrationOptions`
		 * return value for the corresponding server capability as well.
		 */
		public bool dynamicRegistration;

		/**
		 * The client supports additional metadata in the form of definition links.
		 *
		 * @since 3.14.0
		 */
		public bool linkSupport;
	}

	interface IImplementationOptions : IWorkDoneProgressOptions
	{
	}

	interface IImplementationRegistrationOptions : ITextDocumentRegistrationOptions, IImplementationOptions, IStaticRegistrationOptions{		
	}
	class ImplementationRegistrationOptions : IImplementationRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
		public string id { get; set; }
	}
	interface IImplementationParams : ITextDocumentPositionParams,IWorkDoneProgressParams, IPartialResultParams {
	}
}
