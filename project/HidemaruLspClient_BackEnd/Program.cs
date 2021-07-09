using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using COMRegistration;


namespace HidemaruLspClient
{
    class Program
    {
#if EMBEDDED_TYPE_LIBRARY
        //private static readonly string tlbPath = exePath;
        private static readonly string tlbPath="";
#else        
        
        private static readonly string tlbPath_x86 = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\HidemaruLspClient_IDL\Debug\HidemaruLspClient_BackEnd.tlb");
        private static readonly string tlbPath_x64 = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\HidemaruLspClient_IDL\x64\Debug\HidemaruLspClient_BackEnd.tlb");
#endif

        static int Main(string[] args)
        {
            using (var consoleTrace = new ConsoleTraceListener())
            {
                Trace.Listeners.Add(consoleTrace);
                Trace.WriteLine("[Start]exe");
                string tlbPath;
                if (Environment.Is64BitProcess)
                {
                    tlbPath = tlbPath_x64;
                }
                else
                {
                    tlbPath = tlbPath_x86;
                }
                
                if (!File.Exists(tlbPath))
                {
                    Trace.WriteLine("Not found .tlb");
                    return 1;
                }

                if (args.Length == 1)
                {
                    string regCommandMaybe = args[0];
                    var exePath = Process.GetCurrentProcess().MainModule.FileName;
                    var progId = Attribute.GetCustomAttribute(typeof(HidemaruLspBackEndServer), typeof(ProgIdAttribute)) as ProgIdAttribute;
                    
                    switch (regCommandMaybe.ToLower()) {
                        case "/regserver":
                        case "-regserver":
                            LocalServer.RegisterToLocalMachine(LspContract.Constants.ServerClassGuid, progId, exePath, tlbPath);
                            return 0;

                        case "/unregserver":
                        case "-unregserver":                    
                            LocalServer.UnregisterFromLocalMachine(LspContract.Constants.ServerClassGuid, progId, tlbPath);
                            return 0;

                        case "/regserverperuser":
                        case "-regserverperuser":
                            LocalServer.RegisterToCurrentUser(LspContract.Constants.ServerClassGuid, progId, exePath, tlbPath);
                            return 0;

                        case "/unregserverperuser":
                        case "-unregserverperuser":                    
                            LocalServer.UnregisterToCurrentUser(LspContract.Constants.ServerClassGuid, progId, tlbPath);
                            return 0;
                        
                        case "/?":
                        case "/h":
                        case "/help":
                        case "-h":
                        case "--help":
                            Usage();
                            return 0;
                        /*default:
                            Trace.WriteLine("Unknown argument.");
                            return 1;*/
                    }
                }

                using (var server = new LocalServer())
                {
                    server.RegisterClass<HidemaruLspBackEndServer>(LspContract.Constants.ServerClassGuid);
                    server.Run();
                }
                Trace.WriteLine("[Finish]exe");
                return 0;
            }
        }
        static void Usage()
        {
            //Todo: Usage を書く
            Console.WriteLine("Usage");
        }
    }
}

