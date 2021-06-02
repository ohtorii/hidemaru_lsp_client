using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientExample
{
	class LuaClient
	{

		static string rootPath = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\TestData\lua\");
		static Uri rootUri = new Uri(rootPath);
		static Uri sourceUri = new Uri(rootUri, @"test1.lua");
		static int sourceVersion = 0;
		static JObject workspaceConfig = (JObject)JsonConvert.DeserializeObject(
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


		/*
		static dynamic workspaceConfig=new {
				Lua = new
				{
					color = new {
						mode = "Semantic"
					},
					completion = new {
						callSnippet = "Disable",
						enable = true,
						keywordSnippet = "Replace"
					},
					develop = new
					{
						debuggerPort = 11412,
						debuggerWait = false,
						enable = false
					},
					diagnostics = new
					{
						enable = true,
						globals = "",
						severity = new
						{
						}
					},
					hover = new
					{
						enable = true,
						viewNumber = true,
						viewString = true,
						viewStringMax = 1000
					},
					runtime = new
					{
						path = new[] { "?.lua", "?/init.lua", "?/?.lua" },
						version = "Lua 5.3",
					},
					signatureHelp = new
					{
						enable = true
					},
					workspace = new
					{
						ignoreDir = new string[] { },
						maxPreload = 1000,
						preloadFileSize = 100,
						useGitIgnore = true
					}
				}
			};
		*/
		public static void Start()
		{
#if true
			string logFilename = @"D:\temp\LSP-Server\lsp_server_response_lua.txt";
			var sumnekoLuaLanguageServer = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\AppData\Local\vim-lsp-settings\servers\sumneko-lua-language-server\");
			var FileName = "cmd";
			var serverCmd = Path.Combine(sumnekoLuaLanguageServer, @"sumneko-lua-language-server.cmd");
			var Arguments = String.Format(@"/c ""{0}""", serverCmd);
			var WorkingDirectory = rootPath;
#endif

			var client = new LSP.Client.StdioClient();
			client.StartLspProcess(new LSP.Client.StdioClient.LspParameter 
			{ 
				exeFileName = FileName, 
				exeArguments = Arguments, 
				exeWorkingDirectory = WorkingDirectory,
				logger = new Logger(logFilename),
				jsonWorkspaceConfiguration =workspaceConfig
			});

			Console.WriteLine("==== InitializeServer ====");
			InitializeServer(client);
			while (client.Status != LSP.Client.StdioClient.Mode.ServerInitializeFinish)
			{
				Thread.Sleep(100);
			}

			Console.WriteLine("==== InitializedClient ====");
			InitializedClient(client);

			//Console.WriteLine("==== DidChangeConfiguration ====");
			//DidChangeConfiguration(client);

			Console.WriteLine("==== OpenTextDocument ====");
			DigOpen(client);

			//少し待つ必要がある
			Thread.Sleep(2000);
			Console.WriteLine("==== Completion ====");
			Completion(client);

			Console.WriteLine("続行するには何かキーを押してください．．．");
			Console.ReadKey();
		}
		static void InitializeServer(LSP.Client.StdioClient client)
		{
			var param = UtilInitializeParams.Initialzie();
			param.rootUri = rootUri.AbsoluteUri;
			param.rootPath = rootUri.AbsolutePath;
			client.SendInitialize(param);
		}
		static void InitializedClient(LSP.Client.StdioClient client)
		{
			var param = new InitializedParams();
			client.SendInitialized(param);
		}
		static void DidChangeConfiguration(LSP.Client.StdioClient client)
		{
			var param = new DidChangeConfigurationParams();
			/*(Memo) Based on 
			 * vim-lsp-settings\settings\sumneko-lua-language-server.vim
			 * 
			 */
			param.settings = workspaceConfig;
			client.SendWorkspaceDidChangeConfiguration(param);
		}

		static void DigOpen(LSP.Client.StdioClient client)
		{
			sourceVersion = 1;//Openしたのでとりあえず1にする。

			var param = new DidOpenTextDocumentParams();
			param.textDocument.uri = sourceUri.AbsoluteUri;
			param.textDocument.version = sourceVersion;
			param.textDocument.text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8);
			param.textDocument.languageId = "lua";
			client.SendTextDocumentDigOpen(param);
		}
		static void Completion(LSP.Client.StdioClient client)
		{
			var param = new CompletionParams();
#if false
            param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
            param.context.triggerCharacter = ".";
#else
			//param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
			//param.context.triggerCharacter = ".";
#endif
			param.textDocument.uri = sourceUri.AbsoluteUri;
			param.position.line = 11;
			param.position.character = 9;
			client.SendTextDocumentCompletion(param);
		}
	}
}
