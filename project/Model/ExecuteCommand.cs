using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IExecuteCommandOptions :IWorkDoneProgressOptions
	{
		string[] commands { get; set; }
	}
	interface IExecuteCommandRegistrationOptions:IExecuteCommandOptions
	{
	}
	class ExecuteCommandOptions : IExecuteCommandOptions
	{
		public string[] commands { get; set; }
		public bool workDoneProgress { get; set; }
	}
	class ExecuteCommandRegistrationOptions : IExecuteCommandRegistrationOptions
	{
		public string[] commands { get; set; }
		public bool workDoneProgress { get; set; }
	}
}
