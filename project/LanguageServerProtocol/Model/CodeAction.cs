using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class CodeActionClientCapabilities
	{
		/**
		 * Whether code action supports dynamic registration.
		 */
		public bool dynamicRegistration;

		/**
		 * The client supports code action literals as a valid
		 * response of the `textDocument/codeAction` request.
		 *
		 * @since 3.8.0
		 */
		public class _codeActionLiteralSupport
		{
			/**
			 * The code action kind is supported with the following value
			 * set.
			 */
			public class _codeActionKind
			{
				/**
				 * The code action kind values the client supports. When this
				 * property exists the client also guarantees that it will
				 * handle values outside its set gracefully and falls back
				 * to a default value when unknown.
				 */
				public /*CodeActionKind[]*/ string[] valueSet;
			}
			public _codeActionKind codeActionKind
			{
				get
				{
					if (m_codeActionKind == null)
					{
						m_codeActionKind = new _codeActionKind();
					}
					return m_codeActionKind;
				}
			}
			[JsonIgnore] _codeActionKind m_codeActionKind = null;
		}
		public _codeActionLiteralSupport codeActionLiteralSupport
		{
			get
			{
				if (m_codeActionLiteralSupport == null)
				{
					m_codeActionLiteralSupport = new _codeActionLiteralSupport();
				}
				return m_codeActionLiteralSupport;
			}
		}
		[JsonIgnore] _codeActionLiteralSupport m_codeActionLiteralSupport=null;
		/**
		 * Whether code action supports the `isPreferred` property.
		 *
		 * @since 3.15.0
		 */
		public bool isPreferredSupport ;

		/**
		 * Whether code action supports the `disabled` property.
		 *
		 * @since 3.16.0
		 */
		public bool disabledSupport;

		/**
		 * Whether code action supports the `data` property which is
		 * preserved between a `textDocument/codeAction` and a
		 * `codeAction/resolve` request.
		 *
		 * @since 3.16.0
		 */
		public bool dataSupport;


		/**
		 * Whether the client supports resolving additional code action
		 * properties via a separate `codeAction/resolve` request.
		 *
		 * @since 3.16.0
		 */
		public class _resolveSupport  {
			/**
			 * The properties that a client can resolve lazily.
			 */
			public string[] properties;
		};
		public _resolveSupport resolveSupport;
		/**
		 * Whether the client honors the change annotations in
		 * text edits and resource operations returned via the
		 * `CodeAction#edit` property by for example presenting
		 * the workspace edit in the user interface and asking
		 * for confirmation.
		 *
		 * @since 3.16.0
		 */
		public bool honorsChangeAnnotations ;
	}

	interface ICodeActionOptions : IWorkDoneProgressOptions
	{
		/*CodeActionKind[]*/ string[] codeActionKinds  { get;set; }
		/**
		 * The server provides support to resolve additional
		 * information for a code action.
		 *
		 * @since 3.16.0
		 */
		bool resolveProvider { get; set; }
	}
	interface ICodeActionRegistrationOptions : ITextDocumentRegistrationOptions, ICodeActionOptions {
	}

	class CodeActionOptions : ICodeActionOptions
	{		
		public bool workDoneProgress { get; set; }
		public /*CodeActionKind[]*/ string[] codeActionKinds { get; set; }
		public bool resolveProvider { get; set; }
	}
	class CodeActionRegistrationOptions : ICodeActionRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }		
		public bool workDoneProgress { get; set; }
		public /*CodeActionKind[]*/ string[] codeActionKinds { get; set; }
		public bool resolveProvider { get; set; }
	}
#if false
/**
 * Params for the CodeActionRequest
 */
export interface CodeActionParams extends WorkDoneProgressParams,
	PartialResultParams {
	/**
	 * The document in which the command was invoked.
	 */
	textDocument: TextDocumentIdentifier;

	/**
	 * The range for which the command was invoked.
	 */
	range: Range;

	/**
	 * Context carrying additional information.
	 */
	context: CodeActionContext;
}
#endif

	/**
	 * The kind of a code action.
	 *
	 * Kinds are a hierarchical list of identifiers separated by `.`,
	 * e.g. `"refactor.extract.function"`.
	 *
	 * The set of kinds is open and client needs to announce the kinds it supports
	 * to the server during initialization.
	 */
	//export type CodeActionKind = string;
	
	/// <summary>
	/// The kind of a code action. Kinds are a hierarchical list of identifiers separated by <c>.</c>, e.g. <c>"refactor.extract.function"</c>.
	/// </summary>
	public static class CodeActionKind
	{

		/// <summary>
		/// Empty kind.
		/// </summary>
		public const string Empty = "";

		/// <summary>
		/// Base kind for quickfix actions: "quickfix".
		/// </summary>
		public const string QuickFix = "quickfix";

		/// <summary>
		/// Base kind for refactoring actions: "refactor".
		/// </summary>
		public const string Refactor = "refactor";

		/// <summary>
		/// Base kind for refactoring extraction actions: "refactor.extract".
		///
		/// Example extract actions:
		///
		/// - Extract method
		/// - Extract function
		/// - Extract variable
		/// - Extract interface from class
		/// - ...
		/// </summary>
		public const string RefactorExtract = "refactor.extract";

		/// <summary>
		/// Base kind for refactoring inline actions: "refactor.inline".
		///
		/// Example inline actions:
		///
		/// - Inline function
		/// - Inline variable
		/// - Inline constant
		/// - ...
		/// </summary>
		public const string RefactorInline = "refactor.inline";

		/// <summary>
		/// Base kind for refactoring rewrite actions: "refactor.rewrite".
		///
		/// Example rewrite actions:
		///
		/// - Convert JavaScript function to class
		/// - Add or remove parameter
		/// - Encapsulate field
		/// - Make method static
		/// - Move method to base class
		/// - ...
		/// </summary>
		public const string RefactorRewrite = "refactor.rewrite";

		/// <summary>
		/// Base kind for source actions: `source`.
		///
		/// Source code actions apply to the entire file.
		/// </summary>
		public const string Source = "source";

		/// <summary>
		/// Base kind for an organize imports source action: `source.organizeImports`.
		/// </summary>
		public const string SourceOrganizeImports = "source.organizeImports";
	}


#if false
	/**
	 * Contains additional diagnostic information about the context in which
	 * a code action is run.
	 */
	export interface CodeActionContext {
	/**
	 * An array of diagnostics known on the client side overlapping the range
	 * provided to the `textDocument/codeAction` request. They are provided so
	 * that the server knows which errors are currently presented to the user
	 * for the given range. There is no guarantee that these accurately reflect
	 * the error state of the resource. The primary parameter
	 * to compute code actions is the provided range.
	 */
	diagnostics: Diagnostic[];

	/**
	 * Requested kind of actions to return.
	 *
	 * Actions not of this kind are filtered out by the client before being
	 * shown. So servers can omit computing them.
	 */
	only?: CodeActionKind[];
}

/**
 * A code action represents a change that can be performed in code, e.g. to fix
 * a problem or to refactor code.
 *
 * A CodeAction must set either `edit` and/or a `command`. If both are supplied,
 * the `edit` is applied first, then the `command` is executed.
 */
export interface CodeAction {

	/**
	 * A short, human-readable, title for this code action.
	 */
	title: string;

	/**
	 * The kind of the code action.
	 *
	 * Used to filter code actions.
	 */
	kind?: CodeActionKind;

	/**
	 * The diagnostics that this code action resolves.
	 */
	diagnostics?: Diagnostic[];

	/**
	 * Marks this as a preferred action. Preferred actions are used by the
	 * `auto fix` command and can be targeted by keybindings.
	 *
	 * A quick fix should be marked preferred if it properly addresses the
	 * underlying error. A refactoring should be marked preferred if it is the
	 * most reasonable choice of actions to take.
	 *
	 * @since 3.15.0
	 */
	isPreferred?: boolean;

	/**
	 * Marks that the code action cannot currently be applied.
	 *
	 * Clients should follow the following guidelines regarding disabled code
	 * actions:
	 *
	 * - Disabled code actions are not shown in automatic lightbulbs code
	 *   action menus.
	 *
	 * - Disabled actions are shown as faded out in the code action menu when
	 *   the user request a more specific type of code action, such as
	 *   refactorings.
	 *
	 * - If the user has a keybinding that auto applies a code action and only
	 *   a disabled code actions are returned, the client should show the user
	 *   an error message with `reason` in the editor.
	 *
	 * @since 3.16.0
	 */
	disabled?: {

		/**
		 * Human readable description of why the code action is currently
		 * disabled.
		 *
		 * This is displayed in the code actions UI.
		 */
		reason: string;
	};

	/**
	 * The workspace edit this code action performs.
	 */
	edit?: WorkspaceEdit;

	/**
	 * A command this code action executes. If a code action
	 * provides an edit and a command, first the edit is
	 * executed and then the command.
	 */
	command?: Command;

	/**
	 * A data entry field that is preserved on a code action between
	 * a `textDocument/codeAction` and a `codeAction/resolve` request.
	 *
	 * @since 3.16.0
	 */
	data?: any;
}

#endif
}
