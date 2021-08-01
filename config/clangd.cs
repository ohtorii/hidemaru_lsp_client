using System;
using System.Text;
using LSP=LanguageServerProcess;

namespace LanguageServerProcess {
    class ServerConfiguration : LSP.Configuration
    {
        public override string GetServerName(){
            return "clangd";
        }
        public override string GetExcutablePath(){
            return @"C:\Program Files\LLVM\bin\clangd.exe";
            //return Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\clangd\clangd.exe");
        }
        public override string GetArguments(){
            return "";
            //return @"--log=verbose";
        }
    }
}