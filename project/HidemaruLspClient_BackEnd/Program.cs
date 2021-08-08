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
using HidemaruLspClient_BackEndContract;


namespace HidemaruLspClient
{
    class Program
    {
        static DllAssemblyResolver dasmr = new DllAssemblyResolver();

#if EMBEDDED_TYPE_LIBRARY
        //private static readonly string tlbPath = exePath;
        private static readonly string tlbPath="";
#else
        private static readonly string tlbPath_x86 = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\HidemaruLspClient_Contract\bin\x86\HidemaruLspClient_BackEndContract.tlb");
        private static readonly string tlbPath_x64 = Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\GitHub\hidemaru_lsp_client\project\HidemaruLspClient_Contract\bin\x64\HidemaruLspClient_BackEndContract.tlb");
#endif

        static int Main(string[] args)
        {
            using (var consoleTrace = new ConsoleTraceListener())
            {
                Trace.Listeners.Add(consoleTrace);
                Trace.WriteLine("[Create]exe");
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
                    Trace.WriteLine($"Not found {tlbPath}");
                    return 1;
                }

                var ServerClassGuid = new Guid((Attribute.GetCustomAttribute(typeof(ServerClass), typeof(GuidAttribute)) as GuidAttribute).Value);
                if (args.Length == 1)
                {                    
                    string regCommandMaybe = args[0];
                    var exePath = Process.GetCurrentProcess().MainModule.FileName;
#if false
                    ProgIdAttribute progId = Attribute.GetCustomAttribute(typeof(HidemaruLspBackEndServer), typeof(ProgIdAttribute)) as ProgIdAttribute;
#else
                    ProgIdAttribute progId = null;
#endif
                    switch (regCommandMaybe.ToLower()) {
                        case "/regserver":
                        case "-regserver":
                            LocalServer.RegisterToLocalMachine(ServerClassGuid, progId, exePath, tlbPath);
                            return 0;

                        case "/unregserver":
                        case "-unregserver":                    
                            LocalServer.UnregisterFromLocalMachine(ServerClassGuid, progId, tlbPath);
                            return 0;

                        case "/regserverperuser":
                        case "-regserverperuser":
                            LocalServer.RegisterToCurrentUser(ServerClassGuid, progId, exePath, tlbPath);
                            return 0;

                        case "/unregserverperuser":
                        case "-unregserverperuser":                    
                            LocalServer.UnregisterToCurrentUser(ServerClassGuid, progId, tlbPath);
                            return 0;
                        
                        case "/?":
                        case "/h":
                        case "/help":
                        case "-h":
                        case "--help":
                            Usage();
                            return 0;
#if true
                        default:
                            Console.WriteLine(string.Format("Unknown argument. {0}", regCommandMaybe));
                            //Trace.WriteLine("Unknown argument. {0}", );
                            //return 1;
                            break;
#endif
                    }
                }

                using (var server = new LocalServer())
                {
                    server.RegisterClass<HidemaruLspBackEndServer>(ServerClassGuid);
                    server.Run();
                }
                Trace.WriteLine("[Finish]exe");
                return 0;
            }
        }
        static void Usage()
        {
            //Todo: Usage を書く
            Console.WriteLine(
@"Usage:
/regserver -regserver
/unregserver -unregserver
/regserverperuser -regserverperuser
/unregserverperuser -unregserverperuser
/? /help -h --help
"
); ; 
        }
    }
}

