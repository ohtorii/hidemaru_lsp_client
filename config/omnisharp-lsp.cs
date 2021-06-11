using System;
using System.Text;
using LanguageServerProcess;

namespace LanguageServerProcess{
    class ServerConfiguration :Configuration
    {
        public override string GetExcutablePath(){
            return @"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\omnisharp-lsp\OmniSharp.exe";
        }
        public override string GetArguments(){       
            var sb = new StringBuilder();
            sb.Append("-lsp --encoding utf-8");
            {
                var source="";
                {
                    var sln = Environment.FindVisualStudioSolutionFileName();
                    if(sln!=""){
                        source=sln;
                    }else{
                        var repo=Environment.FindRepositoryFolderName();
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
                var pid = Environment.GetHostProcessId();            
                sb.AppendFormat(" --host_pid {0}",pid);
            }
            return sb.ToString();
        }
        public override string GetRootUri(){
            var repo=Environment.FindRepositoryFolderName();
            var rootUri=new Uri(repo);
            return rootUri.AbsoluteUri;
        }
    }
}
