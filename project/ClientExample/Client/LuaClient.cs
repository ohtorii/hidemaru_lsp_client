using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;

namespace ClientExample
{
    internal class LuaClient : ExampleBase
    {
        private static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\lua\");
        internal override Uri rootUri => new Uri(rootPath);
        internal override Uri sourceUri => new Uri(rootUri, @"test1.lua");
        internal override string languageId => "lua";
        internal override CompilationPosition compilationPosition => new CompilationPosition { line = 11, character = 9 };

        private static JObject workspaceConfig = (JObject)JsonConvert.DeserializeObject(
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
}");

        internal override LSP.Client.LanguageClient CreateClient()
        {
#if true
            string logFilename = @"D:\temp\LSP-Server\lsp_server_response_lua.txt";
            var sumnekoLuaLanguageServer = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\sumneko-lua-language-server\");
            var FileName = "cmd";
            var serverCmd = Path.Combine(sumnekoLuaLanguageServer, @"sumneko-lua-language-server.cmd");
            var Arguments = String.Format(@"/c ""{0}""", serverCmd);
            var WorkingDirectory = "";
#endif

            var client = new LSP.Client.LanguageClient();
            client.Start(new LSP.Client.LanguageClient.LspParameter
            {
                exeFileName = FileName,
                exeArguments = Arguments,
                exeWorkingDirectory = WorkingDirectory,
                logger = new Logger(logFilename),
                jsonWorkspaceConfiguration = workspaceConfig
            });
            return client;
        }

        internal override void DigOpen(LSP.Client.LanguageClient client)
        {
            base.DigOpen(client);
            //　$/progress　を待つ

            //サーバが準備できるまでちょっと待つ
            int time = 6000;
            Console.WriteLine("Waiting {0}ms.", time);
            Thread.Sleep(time);
        }
    }
}