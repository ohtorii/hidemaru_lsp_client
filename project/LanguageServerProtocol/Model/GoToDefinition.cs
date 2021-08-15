using Newtonsoft.Json;
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
    class DefinitionParams : IDefinitionParams
    {
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
		ProgressToken IWorkDoneProgressParams.workDoneToken { get; set; }
		ProgressToken IPartialResultParams.partialResultToken { get; set; }
		[JsonIgnore] TextDocumentIdentifier m_textDocumentIdentifier;
		[JsonIgnore] Position m_position;
	}
}
