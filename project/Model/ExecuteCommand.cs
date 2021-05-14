using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class ExecuteCommandClientCapabilities
	{
		public bool dynamicRegistration;
	}

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

	class ExecuteCommandParams : IWorkDoneProgressParams
	{
		/**
		 * The identifier of the actual command handler.
		 */
		public string command;
		/**
		 * Arguments that the command should be invoked with.
		 */
		public object arguments;

		public string workDoneToken { get; set; } = null;
	}
}
