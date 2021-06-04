using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IWorkDoneProgressCreateParams
	{
		/**
		 * The token to be used to report progress.
		 */
		ProgressToken token { get; set; }
	}
	class WorkDoneProgressCreateParams : IWorkDoneProgressCreateParams
	{
		public ProgressToken token { get; set; }
	}
}
