using LSP.Model;
using System;
using System.Threading;
using System.IO;
using LSP.Client;
using System.Diagnostics;

namespace ClientExample
{
    internal abstract class ExampleBase
    {
        internal abstract Uri rootUri{get;}
        internal abstract Uri sourceUri { get; }
        //internal abstract string logFilename { get; }
        internal abstract string languageId { get; }
        internal class CompilationPosition
        {
            public uint line { get; set; }
            public uint character { get; set; }
        }
        internal abstract CompilationPosition compilationPosition { get;  }
        internal abstract StdioClient CreateClient();



        int sourceVersion = 0;

        public void Start()
        {
            var client = CreateClient();

            Console.WriteLine("==== InitializeServer ====");
            {
                var initializeId=InitializeServer(client);
                var result = (InitializeResult)client.QueryResponse(initializeId);
                Debug.Assert(client.Status == LSP.Client.StdioClient.Mode.ServerInitializeFinish);
            }

            Console.WriteLine("==== InitializedClient ====");
            InitializedClient(client);
#if false
            Console.WriteLine("==== didChangeConfiguration ====");
            DidChangeConfiguration(client);
#endif
            Console.WriteLine("==== OpenTextDocument ====");
            DigOpen(client);

#if false
            Console.WriteLine("==== DidChangen ====");
            DidChange(client);
#endif
            Console.WriteLine("==== Completion ====");
            Completion(client);

            Console.WriteLine("==== Shutdown ====");
            Shutdown(client);

            Console.WriteLine("続行するには何かキーを押してください．．．");
            Console.ReadKey();
        }
        internal virtual void Completion(StdioClient client)
        {
            RequestId requestId;
            {
                var param = new CompletionParams();
#if false
            param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
            param.context.triggerCharacter = ".";
#else
                param.context.triggerKind = CompletionTriggerKind.TriggerCharacter;
                param.context.triggerCharacter = ".";
#endif
                param.textDocument.uri = sourceUri.AbsoluteUri;
                param.position.line = compilationPosition.line;
                param.position.character = compilationPosition.character;

                requestId = client.SendTextDocumentCompletion(param);
            }
            int timeout = 3*1000;
            var completion = (CompletionList)client.QueryResponse(requestId, timeout);
            if (completion == null)
            {
                Console.WriteLine("[Failed]Compilation");
            }
            else
            {
                Console.WriteLine("[Success]completion.items.Length={0}", completion.items.Length);
            }
        }
        internal virtual void DidChange(LSP.Client.StdioClient client)
        {
            var text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8);
            ++sourceVersion;//ソースを更新したので+1する

            var param = new DidChangeTextDocumentParams();
            param.contentChanges = new[] { new TextDocumentContentChangeEvent { text = text } };
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.textDocument.version = sourceVersion;

        }

        internal virtual void DidChangeConfiguration(LSP.Client.StdioClient client)
        {
            var param = new DidChangeConfigurationParams();
            param.settings = new object();
            client.SendWorkspaceDidChangeConfiguration(param);
        }
        internal virtual void DigOpen(LSP.Client.StdioClient client)
        {
            sourceVersion = 1;//Openしたのでとりあえず1にする。

            var param = new DidOpenTextDocumentParams();
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.textDocument.version = sourceVersion;
            param.textDocument.text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8);
            param.textDocument.languageId = languageId;
            client.SendTextDocumentDigOpen(param);
        }
        internal virtual void InitializedClient(LSP.Client.StdioClient client)
        {
            var param = new InitializedParams();
            client.SendInitialized(param);
        }

        internal virtual RequestId InitializeServer(LSP.Client.StdioClient client)
        {
            var param = UtilInitializeParams.Initialzie();
            param.rootUri = rootUri.AbsoluteUri;
            param.rootPath = rootUri.AbsolutePath;
            param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "VisualStudio-Solution" } };
            return client.SendInitialize(param);
        }
        internal virtual void Shutdown(StdioClient client)
        {
            var requestId = client.SendShutdown();
            var error = (ResponseError)client.QueryResponse(requestId);
            if (error != null)
            {
                Console.WriteLine("[Failed]Shutdown.");
                return;
            }
            client.SendExit();
            Console.WriteLine("[Success]Shutdown.");
        }

    }
}