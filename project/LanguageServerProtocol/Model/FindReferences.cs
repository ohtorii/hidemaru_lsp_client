using Newtonsoft.Json;
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
	interface IReferencesParams : ITextDocumentPositionParams,IWorkDoneProgressParams, IPartialResultParams {
		IReferenceContext context { get;  }
	}
    class ReferencesParams : IReferencesParams
    {
        public IReferenceContext context
        {
            get
            {
                if (m_ReferenceContext == null)
                {
					m_ReferenceContext = new ReferenceContext();
                }
				return m_ReferenceContext;
			} 
		}

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

		[JsonIgnore] ReferenceContext m_ReferenceContext;
		[JsonIgnore] TextDocumentIdentifier m_textDocumentIdentifier;
		[JsonIgnore] Position m_position;
	}
    interface IReferenceContext
	{
		/**
		 * Include the declaration of the current symbol.
		 */
		bool includeDeclaration { get; set; }
	}
    class ReferenceContext : IReferenceContext
    {
        bool IReferenceContext.includeDeclaration { get; set; }
    }
}
