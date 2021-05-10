using System;
using System.Collections.Generic;
using System.Text;
using URI = System.String;

namespace LSP.Model
{
	/**
 * Client capabilities for the show document request.
 *
 * @since 3.16.0
 */
	class ShowDocumentClientCapabilities
	{
		/**
		 * The client has support for the show document
		 * request.
		 */
		public bool support;
	}
	/**
	 * Params to show a document.
	 *
	 * @since 3.16.0
	 */
	class ShowDocumentParams
	{
		/**
		 * The document uri to show.
		 */
		public URI uri;

		/**
		 * Indicates to show the resource in an external program.
		 * To show for example `https://code.visualstudio.com/`
		 * in the default WEB browser set `external` to `true`.
		 */
		public bool external;

		/**
		 * An optional property to indicate whether the editor
		 * showing the document should take focus or not.
		 * Clients might ignore this property if an external
		 * program is started.
		 */
		public bool takeFocus;

		/**
		 * An optional selection range if the document is a text
		 * document. Clients might ignore the property if an
		 * external program is started or the file is not a text
		 * file.
		 */
		public Range selection;
	}

	/**
	 * The result of an show document request.
	 *
	 * @since 3.16.0
	 */
	class ShowDocumentResult
	{
		/**
		 * A boolean indicating if the show was successful.
		 */
		public bool success;
	}	
	
}
