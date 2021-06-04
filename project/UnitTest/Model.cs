using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LSP.Model;

namespace UnitTest
{
	[TestClass]
	public class Model
	{
		[TestMethod]
		public void TestServerCapabilities_1()
		{
			//現時点では例外を出力しなければ良い
			var json_string = @"{""implementationProvider"":{""workDoneProgress"":true,""documentSelector"":null}}";
			var receiver = (JObject)JsonConvert.DeserializeObject(json_string);
			var response = receiver.ToObject<ServerCapabilities>();
		}
		[TestMethod]
		public void TestServerCapabilities_2()
		{
			var json_py =
			@"{	""textDocumentSync"": {
					""openClose"": true, 
					""change"": 2, 
					""willSave"": false, 
					""willSaveWaitUntil"": false, 
					""save"": true
				}, 
				""completionProvider"": {
					""triggerCharacters"": [""."", ""'"", ""\""""], 
					""resolveProvider"": true
				}, 
				""hoverProvider"": true, 
				""signatureHelpProvider"": {
					""triggerCharacters"": [""("", "",""]
				}, 
				""definitionProvider"": true, 
				""referencesProvider"": true, 
				""documentHighlightProvider"": true, 
				""documentSymbolProvider"": true, 
				""codeActionProvider"": {
					""codeActionKinds"": [""refactor.inline"", ""refactor.extract""]
				}, 
				""renameProvider"": true, 
				""executeCommandProvider"": {
					""commands"": []
				}, 
				""workspaceSymbolProvider"": true, 
				""workspace"": {
					""workspaceFolders"": {
						""supported"": true, 
						""changeNotifications"": true
					}
				}
			}";
			var response = ((JObject)JsonConvert.DeserializeObject(json_py)).ToObject<ServerCapabilities>();
			var textDocumentSync = response.textDocumentSync;
			Assert.AreEqual(textDocumentSync.HasKind, false);
			Assert.AreEqual(textDocumentSync.HasOptions, true);
			Assert.AreEqual(textDocumentSync.Options.change, TextDocumentSyncKind.Incremental);
		}
		[TestMethod]
		public void TestInitializeResult()
		{
			//現時点では例外を出力しなければ良い
			var json_string = 
@"{
	""jsonrpc"":""2.0"",
	""id"":1,
	""result"":{
		""capabilities"":{
			""textDocumentSync"":{
				""openClose"":true,
				""change"":1,
				""save"":{
					""includeText"":true
				}
			},
			""hoverProvider"":{},
			""completionProvider"":{
				""resolveProvider"":true,
				""triggerCharacters"":[""."","" ""]
			},
			""signatureHelpProvider"":{
				""triggerCharacters"":[""."",""?"",""[""]
			},
			""definitionProvider"":{},
			""referencesProvider"":{},
			""documentSymbolProvider"":{},
			""workspaceSymbolProvider"":{},
			""codeActionProvider"":{
				""codeActionKinds"":[""source.organizeImports"",""refactor"",""refactor.extract""]
			},
			""codeLensProvider"":{
				""resolveProvider"":true
			},
			""documentFormattingProvider"":{},
			""documentRangeFormattingProvider"":{},
			""documentOnTypeFormattingProvider"":{
				""firstTriggerCharacter"":"";"",""moreTriggerCharacter"":[""}"","")""]
			},
			""renameProvider"":{},
			""executeCommandProvider"":{
				""commands"":[""omnisharp/executeCodeAction"",""omnisharp/executeCodeAction""]
			},
			""experimental"":{},
			""implementationProvider"":{},
			""workspace"":{
				""workspaceFolders"":{
					""changeNotifications"":false
				},
				""fileOperations"":{}
			}
		},
		""serverInfo"":{
			""name"":""OmniSharp"",
			""version"":""1.37.8+Branch.tags-v1.37.8.Sha.1b2b92de97c4ae06d5d3ad52606ef734de2b6d0d.1b2b92de97c4ae06d5d3ad52606ef734de2b6d0d""
		}
	}
}";
			var receiver = (JObject)JsonConvert.DeserializeObject(json_string);
			var response = receiver.ToObject<ResponseMessage>();
			var result = ((JObject)response.result).ToObject<InitializeResult>();
			var xxxxx = result.ToString();
		}

		class SimpleServerCapabilities
		{
			public BooleanOr<HoverOptions> /*boolean | HoverOptions*/ hoverProvider;
		}
		[TestMethod]
		public void TestBooleanOr()
		{
			{
				var json_py = @"{""hoverProvider"":true}";
				var response = ((JObject)JsonConvert.DeserializeObject(json_py)).ToObject<SimpleServerCapabilities>();
				var hoverProvider = response.hoverProvider;
				Assert.AreEqual(hoverProvider.IsBool, true);
				Assert.AreEqual(hoverProvider.IsValue, false);
				Assert.AreEqual(hoverProvider.Bool, true);
				Assert.AreEqual(hoverProvider.Value, null);
			}

			{
				var json_cs = @"{""hoverProvider"":{""workDoneProgress"":true}}";
				var receiver = ((JObject)JsonConvert.DeserializeObject(json_cs));
				var response = receiver.ToObject<SimpleServerCapabilities>();
				var hoverProvider = response.hoverProvider;
				Assert.AreEqual(hoverProvider.IsBool, false);
				Assert.AreEqual(hoverProvider.IsValue, true);
				Assert.AreEqual(hoverProvider.Value.workDoneProgress, true);
			}

		}
		[TestMethod]
		public void TestWorkDoneProgressBegin()
		{
			var json = 
				@"{	""token"":1,
					""value"":{
						""cancellable"":true,
						""kind"":""begin"",
						""percentage"":12,
						""title"":""Loading workspace""
					}
				}";
			var response = ((JObject)JsonConvert.DeserializeObject(json)).ToObject<ProgressParams>();
			Assert.AreEqual(response.token.IsLong, true);
			Assert.AreEqual(response.token.Long, 1);
			var value = (WorkDoneProgressBegin)response.value;
			Assert.AreEqual(value.kind , "begin");
			Assert.AreEqual(value.cancellable, true);
			Assert.AreEqual(value.percentage,(uint)12);
			Assert.AreEqual(value.message,"");
			Assert.AreEqual(value.title, "Loading workspace");
		}
		[TestMethod]
		public void TestWorkDoneProgressReport()
		{
			var json =
				@"{	""token"":""1d-22-ddd-3"",
					""value"":{
						""cancellable"":true,
						""kind"":""report"",
						""percentage"":12,
						""message"":""FOO""
					}
				}";
			var response = ((JObject)JsonConvert.DeserializeObject(json)).ToObject<ProgressParams>();
			Assert.AreEqual(response.token.IsString, true);
			Assert.AreEqual(response.token.String, "1d-22-ddd-3");
			var value = (WorkDoneProgressReport)response.value;
			Assert.AreEqual(value.kind, "report");
			Assert.AreEqual(value.cancellable, true);
			Assert.AreEqual(value.percentage, (uint)12);
			Assert.AreEqual(value.message, "FOO");
		}
		[TestMethod]
		public void TestWorkDoneProgressEnd()
		{
			var json =
				@"{	""token"":""1d-22-ddd-3"",
					""value"":{
						""kind"":""end"",
						""message"":""BAR""
					}
				}";
			var response = ((JObject)JsonConvert.DeserializeObject(json)).ToObject<ProgressParams>();
			Assert.AreEqual(response.token.IsString, true);
			Assert.AreEqual(response.token.String, "1d-22-ddd-3");
			var value = (WorkDoneProgressEnd)response.value;
			Assert.AreEqual(value.kind, "end");
			Assert.AreEqual(value.message, "BAR");
		}
	}
	
}
