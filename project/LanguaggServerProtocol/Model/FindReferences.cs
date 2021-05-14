using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class ReferenceClientCapabilities
	{
		/**
		 * Whether references supports dynamic registration.
		 */
		public bool dynamicRegistration;
	}

	interface IReferenceOptions : IWorkDoneProgressOptions
	{
	}
	interface  IReferenceRegistrationOptions: ITextDocumentRegistrationOptions, IReferenceOptions {
	}
	class ReferenceOptions : IReferenceOptions
	{
		public bool workDoneProgress { get; set; }
	}
	class ReferenceRegistrationOptions : IReferenceRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
	}
	interface IReferenceParams : ITextDocumentPositionParams,IWorkDoneProgressParams, IPartialResultParams {
		IReferenceContext context { get; set; }
	}

	interface IReferenceContext
	{
		/**
		 * Include the declaration of the current symbol.
		 */
		bool includeDeclaration { get; set; }
	}
}
