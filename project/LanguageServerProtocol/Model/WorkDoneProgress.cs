using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Model
{
	[JsonConverter(typeof(Serialization.Converter.WorkDoneProgressBaseConverter))]
	class WorkDoneProgressBase
	{
		public string kind { get; set; }
	}
	class WorkDoneProgressBegin: WorkDoneProgressBase
	{
		//public string kind /*'begin'*/{ get; set; } 

		/**
		 * Mandatory title of the progress operation. Used to briefly inform about
		 * the kind of operation being performed.
		 *
		 * Examples: "Indexing" or "Linking dependencies".
		 */
		public string title { get; set; }

		/**
		 * Controls if a cancel button should show to allow the user to cancel the
		 * long running operation. Clients that don't support cancellation are
		 * allowed to ignore the setting.
		 */
		public bool cancellable { get; set; }

		/**
		 * Optional, more detailed associated progress message. Contains
		 * complementary information to the `title`.
		 *
		 * Examples: "3/25 files", "project/src/module2", "node_modules/some_dep".
		 * If unset, the previous progress message (if any) is still valid.
		 */
		public string message { get; set; }

		/**
		 * Optional progress percentage to display (value 100 is considered 100%).
		 * If not provided infinite progress is assumed and clients are allowed
		 * to ignore the `percentage` value in subsequent in report notifications.
		 *
		 * The value should be steadily rising. Clients are free to ignore values
		 * that are not following this rule. The value range is [0, 100]
		 */
		public uint percentage { get; set; }
	}
	class WorkDoneProgressReport: WorkDoneProgressBase
	{
		//public string kind/* 'report'*/ { get; set; }

		/**
		 * Controls enablement state of a cancel button. This property is only valid
		 * if a cancel button got requested in the `WorkDoneProgressBegin` payload.
		 *
		 * Clients that don't support cancellation or don't support control the
		 * button's enablement state are allowed to ignore the setting.
		 */
		public bool cancellable { get; set; }

		/**
		 * Optional, more detailed associated progress message. Contains
		 * complementary information to the `title`.
		 *
		 * Examples: "3/25 files", "project/src/module2", "node_modules/some_dep".
		 * If unset, the previous progress message (if any) is still valid.
		 */
		public string message{ get; set; }

		/**
		 * Optional progress percentage to display (value 100 is considered 100%).
		 * If not provided infinite progress is assumed and clients are allowed
		 * to ignore the `percentage` value in subsequent in report notifications.
		 *
		 * The value should be steadily rising. Clients are free to ignore values
		 * that are not following this rule. The value range is [0, 100]
		 */
		public uint percentage { get; set; }
	}
	class WorkDoneProgressEnd: WorkDoneProgressBase
	{

		//public string kind/* 'end'*/{ get; set; }

		/**
		 * Optional, a final message indicating to for example indicate the outcome
		 * of the operation.
		 */
		public string message { get; set; }
	}


	interface IWorkDoneProgressOptions
	{
        bool workDoneProgress { get; set; }
    }
    class WorkDoneProgressOptions: IWorkDoneProgressOptions
    {
        public bool workDoneProgress { get; set; } = false;
    }
    interface IWorkDoneProgressParams
    {
        /**
         * An optional token that a server can use to report work done progress.
         */
        ProgressToken workDoneToken { get; set; }
    }
}
