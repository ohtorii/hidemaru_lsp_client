using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class TypeDefinitionClientCapabilities
	{
		/**
		 * Whether implementation supports dynamic registration. If this is set to
		 * `true` the client supports the new `TypeDefinitionRegistrationOptions`
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

	interface ITypeDefinitionOptions : IWorkDoneProgressOptions
	{
	}
	interface ITypeDefinitionRegistrationOptions :ITextDocumentRegistrationOptions, ITypeDefinitionOptions,IStaticRegistrationOptions {
	}
	interface ITypeDefinitionParams : ITextDocumentPositionParams,IWorkDoneProgressParams, IPartialResultParams {
	}

	class TypeDefinitionRegistrationOptions : ITypeDefinitionRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
		public string id { get; set; }
	}
}
