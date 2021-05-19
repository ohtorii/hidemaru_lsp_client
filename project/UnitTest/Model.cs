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
		public void TestMethod1()
		{
			//現時点では例外を出力しなければ良い
			var json_string = @"{""implementationProvider"":{""workDoneProgress"":true,""documentSelector"":null}}";
			var receiver = (JObject)JsonConvert.DeserializeObject(json_string);
			var response = receiver.ToObject<ServerCapabilities>();
		}
		[TestMethod]
		public void TestMethod2()
		{
			//現時点では例外を出力しなければ良い
			var json_string = @"{""jsonrpc"":""2.0"",""id"":1,""result"":{""capabilities"":{""textDocumentSync"":{""openClose"":true,""change"":1,""save"":{""includeText"":true}},""hoverProvider"":{},""completionProvider"":{""resolveProvider"":true,""triggerCharacters"":[""."","" ""]},""signatureHelpProvider"":{""triggerCharacters"":[""."",""?"",""[""]},""definitionProvider"":{},""referencesProvider"":{},""documentSymbolProvider"":{},""workspaceSymbolProvider"":{},""codeActionProvider"":{""codeActionKinds"":[""source.organizeImports"",""refactor"",""refactor.extract""]},""codeLensProvider"":{""resolveProvider"":true},""documentFormattingProvider"":{},""documentRangeFormattingProvider"":{},""documentOnTypeFormattingProvider"":{""firstTriggerCharacter"":"";"",""moreTriggerCharacter"":[""}"","")""]},""renameProvider"":{},""executeCommandProvider"":{""commands"":[""omnisharp/executeCodeAction"",""omnisharp/executeCodeAction""]},""experimental"":{},""implementationProvider"":{},""workspace"":{""workspaceFolders"":{""changeNotifications"":false},""fileOperations"":{}}},""serverInfo"":{""name"":""OmniSharp"",""version"":""1.37.8+Branch.tags-v1.37.8.Sha.1b2b92de97c4ae06d5d3ad52606ef734de2b6d0d.1b2b92de97c4ae06d5d3ad52606ef734de2b6d0d""}}}";
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
		public void TestMethod3()
		{
			{
				var json_py = @"{""hoverProvider"":true}";				
				var response = ((JObject)JsonConvert.DeserializeObject(json_py)).ToObject<SimpleServerCapabilities>();
				Assert.AreEqual(response.hoverProvider.IsBool, true);
				Assert.AreEqual(response.hoverProvider.IsValue, false);
				Assert.AreEqual(response.hoverProvider.Bool, true);
				Assert.AreEqual(response.hoverProvider.Value, null);
			}
			
			{
				var json_cs = @"{""hoverProvider"":{""workDoneProgress"":true}}";
				var receiver = ((JObject)JsonConvert.DeserializeObject(json_cs));
				var response = receiver.ToObject<SimpleServerCapabilities>();
				Assert.AreEqual(response.hoverProvider.IsBool, false);
				Assert.AreEqual(response.hoverProvider.IsValue, true);
				Assert.AreEqual(response.hoverProvider.Value.workDoneProgress, true);
			}
			
		}
	}
}
