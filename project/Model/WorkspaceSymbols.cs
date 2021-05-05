using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IWorkspaceSymbolOptions :IWorkDoneProgressOptions
	{
	}
	interface IWorkspaceSymbolRegistrationOptions:IWorkspaceSymbolOptions
	{
	}
	class WorkspaceSymbolOptions : IWorkspaceSymbolOptions
	{
		public bool workDoneProgress { get; set; }
	}
	class WorkspaceSymbolRegistrationOptions : IWorkspaceSymbolRegistrationOptions
	{
		public bool workDoneProgress { get; set; }
	}
}
