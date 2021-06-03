using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface WorkDoneProgressCreateParams
	{
		/**
		 * The token to be used to report progress.
		 */
		ProgressToken token { get; set; }
	}
}
