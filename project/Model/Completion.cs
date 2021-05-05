using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	/**
	 * Completion options.
	 */
	class CompletionOptions : WorkDoneProgressOptions
	{
		/**
		 * Most tools trigger completion request automatically without explicitly
		 * requesting it using a keyboard shortcut (for example Ctrl+Space).
		 * Typically they do so when the user starts to type an identifier.
		 * For example, if the user types `c` in a JavaScript file, code complete
		 * will automatically display `console` along with others as a
		 * completion item.
		 * Characters that make up identifiers don't need to be listed here.
		 *
		 * If code complete should automatically be triggered on characters
		 * not being valid inside an identifier (for example `.` in JavaScript),
		 * list them in `triggerCharacters`.
		 */
		public string[]  triggerCharacters;

		/**
		 * The list of all possible characters that commit a completion.
		 * This field can be used if clients don't support individual commit
		 * characters per completion item. See `ClientCapabilities.`
		 * `textDocument.completion.completionItem.commitCharactersSupport`.
		 *
		 * If a server provides both `allCommitCharacters` and commit characters
		 * on an individual completion item, the ones on the completion item win.
		 *
		 * @since 3.2.0
		 */
		public string[] allCommitCharacters;

		/**
		 * The server provides support to resolve additional
		 * information for a completion item.
		 */
		public bool resolveProvider;
	}
}
