using LSP.Implementation;
using LSP.Model;
using System;
using System.Diagnostics;
using System.IO;

namespace ClientExample
{
    internal abstract class ExampleBase
    {
        internal abstract Uri rootUri { get; }
        internal abstract Uri sourceUri { get; }

        //internal abstract string logFilename { get; }
        internal abstract string languageId { get; }

        internal class CompilationPosition
        {
            public uint line { get; set; }
            public uint character { get; set; }
        }

        internal abstract CompilationPosition compilationPosition { get; }

        internal abstract LanguageClient CreateClient();

        internal virtual object serverInitializationOptions { get; } = null;

        private int sourceVersion = 0;

        public void Start()
        {
            var client = CreateClient();

            Console.WriteLine("==== InitializeServer ====");
            {
                var initializeId = InitializeServer(client);
                var result = (InitializeResult)client.QueryResponse(initializeId).item;
                Debug.Assert(client.Status == LSP.Implementation.LanguageClient.Mode.ServerInitializeFinish);
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

            Console.WriteLine("==== textDocument/publishDiagnostics ====");
            publishDiagnostics(client);

            Console.WriteLine("==== textDFocument/DidClose ====");
            DigClose(client);

            Console.WriteLine("==== Shutdown ====");
            Shutdown(client);

            //Console.WriteLine("続行するには何かキーを押してください．．．");
            //Console.ReadKey();
        }

        internal virtual void publishDiagnostics(LanguageClient client)
        {
            //Todo: あとで実装
            /*var diagnostics = client.PullTextDocumentPublishDiagnostics(sourceUri.AbsoluteUri);
            if (diagnostics == null)
            {
                Console.WriteLine("diagnostics==null");
                return;
            }
            Console.WriteLine(string.Format("diagnostics.diagnostics.Length={0}", diagnostics.diagnostics.Length));
            */
        }
        internal virtual void Completion(LanguageClient client)
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

                requestId = client.Send.TextDocumentCompletion(param);
            }
            var millisecondsTimeout = 6000;
            //Console.WriteLine(string.Format("millisecondsTimeout={0}", millisecondsTimeout));
            var completion = (CompletionList)client.QueryResponse(requestId, millisecondsTimeout).item;
            if (completion == null)
            {
                Console.WriteLine("Compilation is failed or timeout.");
                //throw new Exception();
            }
            else
            {
                Console.WriteLine("[Success]completion.items.Length={0}", completion.items.Length);
            }
        }

        internal virtual void DidChange(LSP.Implementation.LanguageClient client)
        {
            var text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8);
            ++sourceVersion;//ソースを更新したので+1する

            var param = new DidChangeTextDocumentParams();
            param.contentChanges = new[] { new TextDocumentContentChangeEvent { text = text } };
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.textDocument.version = sourceVersion;
        }

        internal virtual void DidChangeConfiguration(LSP.Implementation.LanguageClient client)
        {
            var param = new DidChangeConfigurationParams();
            param.settings = new object();
            client.Send.WorkspaceDidChangeConfiguration(param);
        }

        internal virtual void DigOpen(LSP.Implementation.LanguageClient client)
        {
            sourceVersion = 1;//Openしたのでとりあえず1にする。

            var param = new DidOpenTextDocumentParams();
            param.textDocument.uri = sourceUri.AbsoluteUri;
            param.textDocument.version = sourceVersion;
            param.textDocument.text = File.ReadAllText(sourceUri.AbsolutePath, System.Text.Encoding.UTF8);
            param.textDocument.languageId = languageId;
            client.Send.TextDocumentDidOpen(param);
        }
        internal virtual void DigClose(LSP.Implementation.LanguageClient client)
        {
            var param = new DidCloseTextDocumentParams();
            param.textDocument.uri = sourceUri.AbsoluteUri;
            client.Send.TextDocumentDidClose(param);
        }
        internal virtual void InitializedClient(LSP.Implementation.LanguageClient client)
        {
            var param = new InitializedParams();
            client.Send.Initialized(param);
        }

        internal virtual RequestId InitializeServer(LSP.Implementation.LanguageClient client)
        {
            var param = UtilInitializeParams.Initialzie();
            param.rootUri = rootUri.AbsoluteUri;
            param.rootPath = rootUri.AbsolutePath;
            param.workspaceFolders = new[] { new WorkspaceFolder { uri = rootUri.AbsoluteUri, name = "VisualStudio-Solution" } };
            param.initializationOptions = serverInitializationOptions;
            return client.Send.Initialize(param);
        }

        internal virtual void Shutdown(LanguageClient client)
        {
            var requestId = client.Send.Shutdown();
            var error = client.QueryResponse(requestId).item as ResponseError;
            if (error != null)
            {
                Console.WriteLine("[Failed]Shutdown.");
                return;
            }
            client.Send.Exit();
            client.Send.LoggingResponseLeak();
            Console.WriteLine("[Success]Shutdown.");
        }
    }
}