using System;
using System.Text;
using LSP=LanguageServerProcess;

namespace LanguageServerProcess {
    class ServerConfiguration : LSP.Configuration
    {
        public override string GetExcutablePath(){
            return "cmd.exe";
        }
        public override string GetArguments(){
            var serverCmd = System.Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\sumneko-lua-language-server\sumneko-lua-language-server.cmd");
            return String.Format(@"/c ""{0}""", serverCmd);
        }
        public override string GetRootUri(){
            var repo=LSP.Environment.FindRepositoryDirectoryName();
            if(repo.Length==0){
                repo=LSP.Environment.GetCurrentWorkingDirectoryName();
            }
            var rootUri=new Uri(repo);
            return rootUri.AbsoluteUri;
        }
        public override string GetWorkspaceConfig() { 
            return 
@" {
    ""Lua"": {
        ""color"": {
            ""mode"": ""Semantic""
        },
        ""completion"": {
            ""callSnippet"": ""Disable"",
            ""enable"": true,
            ""keywordSnippet"": ""Replace""
        },
        ""develop"": {
            ""debuggerPort"": 11412,
            ""debuggerWait"": false,
            ""enable"": false
        },
        ""diagnostics"": {
            ""enable"": true,
            ""globals"": """",
            ""severity"": {}
        },
        ""hover"": {
            ""enable"": true,
            ""viewNumber"": true,
            ""viewString"": true,
            ""viewStringMax"": 1000
        },
        ""runtime"": {
            ""path"": [""?.lua"", ""?/init.lua"", ""?/?.lua""],
            ""version"": ""Lua 5.3""
        },
        ""signatureHelp"": {
            ""enable"": true
        },
        ""workspace"": {
            ""ignoreDir"": [],
            ""maxPreload"": 1000,
            ""preloadFileSize"": 100,
            ""useGitIgnore"": true
        }
    }
}";
        }
    }
}
