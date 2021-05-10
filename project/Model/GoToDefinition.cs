using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class DefinitionClientCapabilities
	{
		/**
		 * Whether definition supports dynamic registration.
		 */
		public bool dynamicRegistration;

		/**
		 * The client supports additional metadata in the form of definition links.
		 *
		 * @since 3.14.0
		 */
		public bool linkSupport;
	}

	interface IDefinitionOptions : IWorkDoneProgressOptions
	{
	}
	class DefinitionOptions : IWorkDoneProgressOptions
	{
		public bool workDoneProgress { get; set; } = false;
	}
	class DefinitionRegistrationOptions : ITextDocumentRegistrationOptions, IDefinitionOptions {
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; } = false;
	}
	interface IDefinitionParams : ITextDocumentPositionParams,IWorkDoneProgressParams, IPartialResultParams {
	}
}
