using System;
using System.Collections.Generic;
using System.Text;
using DocumentUri = System.String;



namespace LSP.Model
{
	interface IWorkspaceFoldersServerCapabilities
	{
		bool supported { get; set; }
		bool changeNotifications { get; set; }
	}

	interface IWorkspaceFolder
	{		
		DocumentUri uri { get; set; }
		string name { get; set; }
	}
	interface IPartialResultParams
	{
		ProgressToken partialResultToken { get; set; }
	}




	class WorkspaceFoldersServerCapabilities : IWorkspaceFoldersServerCapabilities
	{
		public bool supported { get; set; }
		public bool changeNotifications { get; set; }
	}
	class WorkspaceFolder : IWorkspaceFolder
	{
		//public string IWorkspaceFolder.uri { get; set; }
		//public string IWorkspaceFolder.name { get; set; }
		public DocumentUri uri { get; set; }
		public string name { get; set; }
	}
}
