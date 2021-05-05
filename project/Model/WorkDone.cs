using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Model
{
    using ProgressToken = System.Int16;
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
