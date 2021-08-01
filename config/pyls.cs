using System;
using System.Text;
using LSP=LanguageServerProcess;

namespace LanguageServerProcess {
    class ServerConfiguration : LSP.Configuration
    {
        public override string GetServerName(){
            return "pyls";
        }
        public override string GetExcutablePath(){
            return System.Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\pyls-all\venv\Scripts\pyls.exe");
        }
        public override string GetArguments(){
            return "";
            //return @"-v --log-file D:\temp\pyls.log";
        }
    }
}