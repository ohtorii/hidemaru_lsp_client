using Lsp.Model.Serialization.Converters;
using Newtonsoft.Json;
using System;
using URI = System.String;

namespace LSP.Model
{
    [Serializable()]
    class Diagnostic
    {
        /**
		 * The range at which the message applies.
		 */
        public Range range;

        /**
		 * The diagnostic's severity. Can be omitted. If omitted it is up to the
		 * client to interpret diagnostics as error, warning, info or hint.
		 */
        public DiagnosticSeverity severity;

        /**
		 * The diagnostic's code, which might appear in the user interface.
		 */
        public /*integer | string*/ object code;

        /**
		 * An optional property to describe the error code.
		 *
		 * @since 3.16.0
		 */
        public CodeDescription codeDescription;

        /**
		 * A human-readable string describing the source of this
		 * diagnostic, e.g. 'typescript' or 'super lint'.
		 */
        public string source;

        /**
		 * The diagnostic's message.
		 */
        public string message;

        /**
		 * Additional metadata about the diagnostic.
		 *
		 * @since 3.15.0
		 */
        public DiagnosticTag[] tags;

        /**
		 * An array of related diagnostic information, e.g. when symbol-names within
		 * a scope collide all definitions can be marked via this property.
		 */
        public DiagnosticRelatedInformation[] relatedInformation;

        /**
		 * A data entry field that is preserved between a
		 * `textDocument/publishDiagnostics` notification and
		 * `textDocument/codeAction` request.
		 *
		 * @since 3.16.0
		 */
        public /*unknown*/ object data;
    }
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum DiagnosticSeverity
    {
        Error = 1,
        Warning = 2,
        Information = 3,
        Hint = 4
    }


    /**
	 * The diagnostic tags.
	 *
	 * @since 3.15.0
	 */
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum DiagnosticTag
    {
        Unnecessary = 1,
        Deprecated = 2
    }

    /**
	 * Represents a related message and source code location for a diagnostic.
	 * This should be used to point to code locations that cause or are related to
	 * a diagnostics, e.g when duplicating a symbol in a scope.
	 */
    class DiagnosticRelatedInformation
    {
        /**
		 * The location of this related diagnostic information.
		 */
        public Location location;

        /**
		 * The message of this related diagnostic information.
		 */
        public string message;
    }

    /**
	 * Structure to capture a description for an error code.
	 *
	 * @since 3.16.0
	 */
    class CodeDescription
    {
        /**
		 * An URI to open with more information about the diagnostic error.
		 */
        public URI href;
    }




}
