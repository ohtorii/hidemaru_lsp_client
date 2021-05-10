using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	/**
	 * The options to register for file operations.
	 *
	 * @since 3.16.0
	 */
	class FileOperationRegistrationOptions
	{
		/**
		 * The actual filters.
		 */
		public FileOperationFilter[]  filters;
	}

	/**
	 * A pattern kind describing if a glob pattern matches a file a folder or
	 * both.
	 *
	 * @since 3.16.0
	 */
	[JsonConverter(typeof(StringEnumConverter))]
	enum FileOperationPatternKind
	{
		/**
		 * The pattern matches a file only.
		 */
		file=0,

		/**
		 * The pattern matches a folder only.
		 */
		folder=1,
	}	

	/**
	 * Matching options for the file operation pattern.
	 *
	 * @since 3.16.0
	 */
	class FileOperationPatternOptions
	{
		/**
		* The pattern should be matched ignoring casing.
		*/
		public bool ignoreCase;
	}


	/**
	 * A pattern to describe in which file operation requests or notifications
	 * the server is interested in.
	 *
	 * @since 3.16.0
	 */
	class FileOperationPattern
	{
		/**
		 * The glob pattern to match. Glob patterns can have the following syntax:
		 * - `*` to match one or more characters in a path segment
		 * - `?` to match on one character in a path segment
		 * - `**` to match any number of path segments, including none
		 * - `{}` to group sub patterns into an OR expression. (e.g. `**​/*.{ts,js}`
		 *   matches all TypeScript and JavaScript files)
		 * - `[]` to declare a range of characters to match in a path segment
		 *   (e.g., `example.[0-9]` to match on `example.0`, `example.1`, …)
		 * - `[!...]` to negate a range of characters to match in a path segment
		 *   (e.g., `example.[!0-9]` to match on `example.a`, `example.b`, but
		 *   not `example.0`)
		 */
		public string glob;

		/**
		 * Whether to match files or folders with this pattern.
		 *
		 * Matches both if undefined.
		 */
		public FileOperationPatternKind matches;

		/**
		 * Additional options used during matching.
		 */
		public FileOperationPatternOptions options;
	}


	/**
	 * A filter to describe in which file operation requests or notifications
	 * the server is interested in.
	 *
	 * @since 3.16.0
	 */
	class FileOperationFilter
	{
		/**
		 * A Uri like `file` or `untitled`.
		 */
		public string scheme;

		/**
		 * The actual file operation pattern.
		 */
		public FileOperationPattern pattern;
	}

	/**
	 * The parameters sent in notifications/requests for user-initiated creation
	 * of files.
	 *
	 * @since 3.16.0
	 */
	class CreateFilesParams
	{
		/**
		 * An array of all files/folders created in this operation.
		 */
		public FileCreate[] files;
	}

	/**
	 * Represents information on a file/folder create.
	 *
	 * @since 3.16.0
	 */
	class FileCreate
	{
		/**
		* A file:// URI for the location of the file/folder being created.
		*/
		public string uri;
	}

}
