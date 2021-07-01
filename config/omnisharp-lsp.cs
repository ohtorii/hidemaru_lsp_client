using System;
using System.Text;
using LSP=LanguageServerProcess;

namespace LanguageServerProcess {
    class ServerConfiguration : LSP.Configuration
    {
        public override string GetExcutablePath(){
            return System.Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\omnisharp-lsp\OmniSharp.exe");
        }
        public override string GetArguments(){
            var sb = new StringBuilder();
            sb.Append("-lsp --encoding utf-8");
            {
                var source=FindSource();
                if(source!=""){
                    sb.AppendFormat(" --source {0}",source);
                }
            }

            {
                var pid = LSP.Environment.GetHostProcessId();
                sb.AppendFormat(" --host_pid {0}",pid);
            }
            return sb.ToString();
        }
        static string FindSource(){
            var sln = LSP.Environment.FindVisualStudioSolutionFileName();
            if(sln!=""){
                return sln;
            }
            var repo=LSP.Environment.FindRepositoryDirectoryName();
            if(repo!=""){
                return repo;
            }
            return LSP.Environment.GetCurrentWorkingDirectoryName();
        }
        public override string GetRootUri(){
            return LSP.Environment.FindRootUriAsAdhoc();
        }
    }
}
