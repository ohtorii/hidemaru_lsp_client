using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
#if false
	export interface TextDocumentSyncClientCapabilities
	{
		/**
		 * Whether text document synchronization supports dynamic registration.
		 */
		dynamicRegistration?: boolean;

	/**
	 * The client supports sending will save notifications.
	 */
	willSave?: boolean;

	/**
	 * The client supports sending a will save request and
	 * waits for a response providing text edits which will
	 * be applied to the document before it is saved.
	 */
	willSaveWaitUntil?: boolean;

	/**
	 * The client supports did save notifications.
	 */
	didSave?: boolean;
}
#endif

	/**
	 * Defines how the host (editor) should sync document changes
	 * to the language server.
	 */
	enum TextDocumentSyncKind
	{
		/**
		 * Documents should not be synced at all.
		 */
		None = 0,

		/**
		 * Documents are synced by always sending the full content
		 * of the document.
		 */
		Full = 1,

		/**
		 * Documents are synced by sending the full content on open.
		 * After that only incremental updates to the document are
		 * send.
		 */
		Incremental = 2,
	}


	class TextDocumentSyncOptions
	{
		/**
		 * Open and close notifications are sent to the server.
		 * If omitted, open close notification should not be sent.
		 */
		bool openClose;
		/**
		 * Change notifications are sent to the server.
		 * See TextDocumentSyncKind.None, TextDocumentSyncKind.Full,
		 * and TextDocumentSyncKind.Incremental.
		 * If omitted, it defaults to TextDocumentSyncKind.None.
		 */
		int change;
		/**
		 * If present will save notifications are sent to the server.
		 * If omitted, the notification should not be sent.
		 */
		bool willSave;
		/**
		 * If present will save wait until requests are sent to the server.
		 * If omitted, the request should not be sent.
		 */
		bool willSaveWaitUntil;
		/**
		 * If present save notifications are sent to the server.
		 * If omitted, the notification should not be sent.
		 */
		/*boolean |*/
		SaveOptions save;
	}

	class SaveOptions
	{
		/**
		 * The client is supposed to include the content on save.
		 */
		bool includeText;
	}

	class DocumentFilter
	{
		public string language;
		public string scheme;
		public string pattern;
	}


	interface IStaticRegistrationOptions
	{
		string id { get; set; }
	}


	interface ITextDocumentRegistrationOptions
	{
		/*DocumentSelector*/ DocumentFilter[] documentSelector { get; set; }
	}
	class TextDocumentRegistrationOptions : ITextDocumentRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
	}

}
