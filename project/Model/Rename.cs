using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	interface IRenameOptions :IWorkDoneProgressOptions
	{
		bool prepareProvider { get; set;  }
	}
	interface IRenameRegistrationOptions:ITextDocumentRegistrationOptions, IRenameOptions {
	}

	class RenameOptions : IRenameOptions
	{
		public bool prepareProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
	class RenameRegistrationOptions : IRenameRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool prepareProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
	class WorkspaceSpecificServerCapabilities_
	{
		public WorkspaceFoldersServerCapabilities workspaceFolders { get; set; }
	}
}
