using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class SignatureHelpOptions : WorkDoneProgressOptions
	{
		/**
		 * The characters that trigger signature help
		 * automatically.
		 */
		public string[] triggerCharacters;

		/**
		 * List of characters that re-trigger signature help.
		 *
		 * These trigger characters are only active when signature help is already
		 * showing.
		 * All trigger characters are also counted as re-trigger characters.
		 *
		 * @since 3.15.0
		 */
		public string[] retriggerCharacters;
	}
}
