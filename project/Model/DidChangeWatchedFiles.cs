using Lsp.Model.Serialization.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using DocumentUri = System.String;

namespace LSP.Model
{
	class DidChangeWatchedFilesClientCapabilities
	{
		public bool dynamicRegistration;
	}	
	class DidChangeWatchedFilesRegistrationOptions
	{		
		public FileSystemWatcher[] watchers;
	}

	class FileSystemWatcher
	{
		public string globPattern;		
		public WatchKind kind;
	}

	[Flags]
	[JsonConverter(typeof(NumberEnumConverter))]
	public enum WatchKind
	{
		Create = 1,
		Change = 2,
		Delete = 4
	}

	class DidChangeWatchedFilesParams
	{
		/**
		 * The actual file events.
		 */
		public FileEvent[]  changes;
	}


	/**
 * An event describing a file change.
 */
	class FileEvent
	{
		public DocumentUri uri;
		public FileChangeType type;
	}

	/**
	 * The file event type.
	 */
	[JsonConverter(typeof(NumberEnumConverter))]
	enum FileChangeType
	{		
		Created = 1,	
		Changed = 2,		
		Deleted = 3,
	}
}
