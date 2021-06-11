using System;
using System.Collections.Generic;
using System.Text;
using DocumentUri = System.String;

namespace LSP.Model
{
	class PublishDiagnosticsClientCapabilities
	{
		/**
		 * Whether the clients accepts diagnostics with related information.
		 */
		public bool relatedInformation;

		/**
		 * Client supports the tag property to provide meta data about a diagnostic.
		 * Clients supporting tags have to handle unknown tags gracefully.
		 *
		 * @since 3.15.0
		 */
		public class _tagSupport {
			/**
			 * The tags supported by the client.
			 */
			public DiagnosticTag[]  valueSet;
		};
		public _tagSupport tagSupport;
		/**
		 * Whether the client interprets the version property of the
		 * `textDocument/publishDiagnostics` notification's parameter.
		 *
		 * @since 3.15.0
		 */
		public bool versionSupport;

		/**
		 * Client supports a codeDescription property
		 *
		 * @since 3.16.0
		 */
		public bool codeDescriptionSupport;

		/**
		 * Whether code action supports the `data` property which is
		 * preserved between a `textDocument/publishDiagnostics` and
		 * `textDocument/codeAction` request.
		 *
		 * @since 3.16.0
		 */
		public bool dataSupport;
	}

	class PublishDiagnosticsParams
	{
		/**
		 * The URI for which diagnostic information is reported.
		 */
		public DocumentUri uri;

		/**
		 * Optional the version number of the document the diagnostics are published
		 * for.
		 *
		 * @since 3.15.0
		 */
		public uint version;

		/**
		 * An array of diagnostic information items.
		 */
		public Diagnostic[]  diagnostics;
	}

}
