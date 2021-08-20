using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class HoverClientCapabilities
	{
		/**
		 * Whether hover supports dynamic registration.
		 */
		public bool dynamicRegistration;

		/**
		 * Client supports the following content formats if the content
		 * property refers to a `literal of type MarkupContent`.
		 * The order describes the preferred format of the client.
		 */
		public MarkupKind[]  contentFormat;
	}

	interface IHoverOptions : IWorkDoneProgressOptions
	{
	}
	class HoverOptions : IHoverOptions
	{
		public bool workDoneProgress { get; set; }
	}
	interface IHoverRegistrationOptions:ITextDocumentRegistrationOptions, IHoverOptions {
	}

	interface IHoverParams : ITextDocumentPositionParams,IWorkDoneProgressParams {
	}

    class HoverParams : IHoverParams
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

        public ProgressToken workDoneToken { get; set; }

		[JsonIgnore] TextDocumentIdentifier m_textDocumentIdentifier;
		[JsonIgnore] Position m_position;
	}

	/**
	 * The result of a hover request.
	 */
	class Hover
	{
        /**
		 * The hover's content
		 */
        public /*MarkedString | MarkedString[] |*/ MarkupContent contents;

		/**
		 * An optional range is a range inside a text document
		 * that is used to visualize a hover, e.g. by changing the background color.
		 */
		public Range range;
	}    
}
