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
                var source="";
                {
                    var sln = LSP.Environment.FindVisualStudioSolutionFileName();
                    if(sln!=""){
                        source=sln;
                    }else{
                        var repo=LSP.Environment.FindRepositoryDirectoryName();
                        if(repo!=""){
                            source=repo;
                        }else{
                            //pass
                        }
                    }
                }
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
        public override string GetRootUri(){
            var repo=LSP.Environment.FindRepositoryDirectoryName();
            if(repo.Length==0){
                repo=LSP.Environment.GetCurrentWorkingDirectoryName();
            }
            var rootUri=new Uri(repo);
            return rootUri.AbsoluteUri;
        }
    }
}
