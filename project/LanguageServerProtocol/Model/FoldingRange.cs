using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class FoldingRangeClientCapabilities
	{
		/**
		 * Whether implementation supports dynamic registration for folding range
		 * providers. If this is set to `true` the client supports the new
		 * `FoldingRangeRegistrationOptions` return value for the corresponding
		 * server capability as well.
		 */
		public bool dynamicRegistration;
		/**
		 * The maximum number of folding ranges that the client prefers to receive
		 * per document. The value serves as a hint, servers are free to follow the
		 * limit.
		 */
		public uint rangeLimit;
		/**
		 * If set, the client signals that it only supports folding complete lines.
		 * If set, client will ignore specified `startCharacter` and `endCharacter`
		 * properties in a FoldingRange.
		 */
		public bool lineFoldingOnly;
	}

	interface IFoldingRangeOptions : IWorkDoneProgressOptions
	{
	}
	interface IFoldingRangeRegistrationOptions :
		ITextDocumentRegistrationOptions, IFoldingRangeOptions, IStaticRegistrationOptions {
	}
	interface IFoldingRangeParams : IWorkDoneProgressParams,IPartialResultParams {
		/**
		 * The text document.
		 */
		ITextDocumentIdentifier textDocument { get; set; }
	}

#if false
	/**
	 * Enum of known range kinds
	 */
	enum FoldingRangeKind
	{
		/**
		 * Folding range for a comment
		 */
		Comment = 'comment',
		/**
		 * Folding range for a imports or includes
		 */
		Imports = 'imports',
		/**
		 * Folding range for a region (e.g. `#region`)
		 */
		Region = 'region'
	}
#endif

	/**
	 * Represents a folding range. To be valid, start and end line must be bigger
	 * than zero and smaller than the number of lines in the document. Clients
	 * are free to ignore invalid ranges.
	 */
	interface IFoldingRange
	{
		/**
		 * The zero-based start line of the range to fold. The folded area starts
		 * after the line's last character. To be valid, the end must be zero or
		 * larger and smaller than the number of lines in the document.
		 */
		uint startLine { get; set; }

		/**
		 * The zero-based character offset from where the folded range starts. If
		 * not defined, defaults to the length of the start line.
		 */
		uint startCharacter { get; set; }

		/**
		 * The zero-based end line of the range to fold. The folded area ends with
		 * the line's last character. To be valid, the end must be zero or larger
		 * and smaller than the number of lines in the document.
		 */
		uint endLine { get; set; }

		/**
		 * The zero-based character offset before the folded range ends. If not
		 * defined, defaults to the length of the end line.
		 */
		uint endCharacter { get; set; }

		/**
		 * Describes the kind of the folding range such as `comment` or `region`.
		 * The kind is used to categorize folding ranges and used by commands like
		 * 'Fold all comments'. See [FoldingRangeKind](#FoldingRangeKind) for an
		 * enumeration of standardized kinds.
		 */
		string kind { get; set; }
	}



	class FoldingRangeRegistrationOptions : IFoldingRangeRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
		public string id { get; set; }
	}
}
