using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class Holder
    {
        static LSP.Client.StdioClient client_ = null;

		public static void StartServer(string serverConfigFilename)
		{

			/*
			var FileName = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\omnisharp-lsp\OmniSharp.exe");
			var Arguments = string.Format(@"-lsp -v --source ""{0}"" --hostPID {1} --encoding utf-8", solutionFileName, System.Diagnostics.Process.GetCurrentProcess().Id);
			var WorkingDirectory = @"";

			client_ = new LSP.Client.StdioClient();
			client_.StartLspProcess(
				new LSP.Client.StdioClient.LspParameter
				{
					exeFileName = FileName,
					exeArguments = Arguments,
					exeWorkingDirectory = WorkingDirectory
				}
			);
			*/
		}
    }
}
