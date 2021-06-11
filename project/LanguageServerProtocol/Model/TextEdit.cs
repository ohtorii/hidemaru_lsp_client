using System;
using System.Collections.Generic;
using System.Text;
using ChangeAnnotationIdentifier = System.String;


namespace LSP.Model
{
	interface ITextEdit
	{
		/**
		 * The range of the text document to be manipulated. To insert
		 * text into a document create a range where start === end.
		 */
		Range range { get; set; }

		/**
		 * The string to be inserted. For delete operations use an
		 * empty string.
		 */
		string newText { get; set; }
	}
	class TextEdit : ITextEdit
	{
		public Range range { get; set; } = null;
		public string newText { get; set; } = null;
	}
	/**
	 * Additional information that describes document changes.
	 *
	 * @since 3.16.0
	 */
	interface IChangeAnnotation {
		/**
		 * A human-readable string describing the actual change. The string
		 * is rendered prominent in the user interface.
		 */
		string label { get; set; }

		/**
		 * A flag which indicates that user confirmation is needed
		 * before applying the change.
		 */
		bool needsConfirmation { get; set; }

		/**
		 * A human-readable string which is rendered less prominent in
		 * the user interface.
		 */
		string description { get; set; }
}



	/**
	 * A special text edit with an additional change annotation.
	 *
	 * @since 3.16.0
	 */
	interface IAnnotatedTextEdit : ITextEdit {
		/**
		 * The actual annotation identifier.
		 */
		ChangeAnnotationIdentifier annotationId { get; set; }
	}

}
