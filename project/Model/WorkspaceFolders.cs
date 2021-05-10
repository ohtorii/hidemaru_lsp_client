﻿using System;
using System.Collections.Generic;
using System.Text;
using DocumentUri = System.String;
using ProgressToken = System.String; //integer | string;


namespace LSP.Model
{
	interface IWorkspaceFoldersServerCapabilities
	{
		bool supported { get; set; }
		//仮でstringで実装した
		/*boolean | */string changeNotifications { get; set; }
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
		public string changeNotifications { get; set; }
	}
	class WorkspaceFolder : IWorkspaceFolder
	{
		string IWorkspaceFolder.uri { get; set; }
		string IWorkspaceFolder.name { get; set; }
	}
}
