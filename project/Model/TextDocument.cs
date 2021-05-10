using System;
using System.Collections.Generic;
using System.Text;
using DocumentUri = System.String;

namespace LSP.Model
{
	interface IPosition
	{
		uint line { get; set; }
		uint character { get; set; }
	}
	class Position : IPosition
	{
		public uint line { get; set; }
		public uint character { get; set; }
	}

	interface IRange
	{
		IPosition start { get; set; }
		IPosition end { get; set; } 
	}
	class Range : IRange
	{
		public IPosition start { get; set; } = new Position();
		public IPosition end { get; set; } = new Position();
	}
	interface ILocation
	{
		DocumentUri uri { get; set; }
		IRange range { get; set; } 
	}
	class Location : ILocation
	{
		public string uri { get; set; }
		public IRange range { get; set; } = new Range();
	}


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
		public bool openClose;
		/**
		 * Change notifications are sent to the server.
		 * See TextDocumentSyncKind.None, TextDocumentSyncKind.Full,
		 * and TextDocumentSyncKind.Incremental.
		 * If omitted, it defaults to TextDocumentSyncKind.None.
		 */
		public int change;
		/**
		 * If present will save notifications are sent to the server.
		 * If omitted, the notification should not be sent.
		 */
		public bool willSave;
		/**
		 * If present will save wait until requests are sent to the server.
		 * If omitted, the request should not be sent.
		 */
		public bool willSaveWaitUntil;
		/**
		 * If present save notifications are sent to the server.
		 * If omitted, the notification should not be sent.
		 */
		/*boolean |*/
		public SaveOptions save;
	}

	class SaveOptions
	{
		/**
		 * The client is supposed to include the content on save.
		 */
		public bool includeText;
	}
	interface ITextDocumentIdentifier
	{
		DocumentUri uri {get;set;}
	}
	class TextDocumentIdentifier : ITextDocumentIdentifier
	{
		public string uri { get; set; }
	}
	interface ITextDocumentPositionParams
	{
		ITextDocumentIdentifier textDocument  { get;set; }
		IPosition position  { get; set; }
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
