using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
#if false
	export namespace PrepareSupportDefaultBehavior
	{
		/**
		 * The client's default behavior is to select the identifier
		 * according the to language's syntax rule.
		 */
		export const Identifier: 1 = 1;
	}
#endif

	class RenameClientCapabilities
	{
		/**
		 * Whether rename supports dynamic registration.
		 */
		public bool dynamicRegistration;

		/**
		 * Client supports testing for validity of rename operations
		 * before execution.
		 *
		 * @since 3.12.0
		 */
		public bool prepareSupport;
#if false
		/**
		 * Client supports the default behavior result
		 * (`{ defaultBehavior: boolean }`).
		 *
		 * The value indicates the default behavior used by the
		 * client.
		 *
		 * @since 3.16.0
		 */
		public PrepareSupportDefaultBehavior prepareSupportDefaultBehavior;
#endif
		/**
		 * Whether th client honors the change annotations in
		 * text edits and resource operations returned via the
		 * rename request's workspace edit by for example presenting
		 * the workspace edit in the user interface and asking
		 * for confirmation.
		 *
		 * @since 3.16.0
		 */
		public bool honorsChangeAnnotations;
	}

	interface IRenameOptions :IWorkDoneProgressOptions
	{
		bool prepareProvider { get; set;  }
	}
	interface IRenameRegistrationOptions:ITextDocumentRegistrationOptions, IRenameOptions {
	}

	class RenameOptions : IRenameOptions
	{
		public bool prepareProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
	class RenameRegistrationOptions : IRenameRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool prepareProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
	interface RenameParams : ITextDocumentPositionParams,IWorkDoneProgressParams {
		/**
		 * The new name of the symbol. If the given name is not valid the
		 * request must return a [ResponseError](#ResponseError) with an
		 * appropriate message set.
		 */
		string newName { get; set; }
	}

	/*class WorkspaceSpecificServerCapabilities_
	{
		public WorkspaceFoldersServerCapabilities workspaceFolders { get; set; }
	}*/
}
