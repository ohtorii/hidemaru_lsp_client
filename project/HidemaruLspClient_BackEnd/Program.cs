using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using COMRegistration;
using HidemaruLspClient_BackEndContract;


namespace HidemaruLspClient
{
    partial class Program
    {
        static DllAssemblyResolver dasmr          = new DllAssemblyResolver();
        static readonly string tlbPath            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HidemaruLspClient_BackEndContract.tlb");
        static readonly bool isConsoleApplication = IsConsoleApplication();

        static int Main(string[] args)
        {
            var o = Options.Create(isConsoleApplication, args);
            if (o == null)
            {
                return 1;
            }
            return Start(o)?1:0;
        }
        static TraceListener CreateTracer(Options options)
        {
            if (options.logStreamWriter == null)
            {
                return new ConsoleTraceListener();
            }
            return new TextWriterTraceListener(options.logStreamWriter);
        }
        static bool Start(Options options) {

            using (var consoleTrace = CreateTracer(options))
            {
                Trace.Listeners.Add(consoleTrace);
                try
                {
                    if (!File.Exists(tlbPath))
                    {
                        Trace.WriteLine($"Not found {tlbPath}");
                        return false;
                    }

                    var serverClassGuid = new Guid((Attribute.GetCustomAttribute(typeof(ServerClass), typeof(GuidAttribute)) as GuidAttribute).Value);
                    var exePath = Process.GetCurrentProcess().MainModule.FileName;
#if false
                    ProgIdAttribute progId = Attribute.GetCustomAttribute(typeof(HidemaruLspBackEndServer), typeof(ProgIdAttribute)) as ProgIdAttribute;
#else
                    ProgIdAttribute progId = null;
#endif
                    switch (options.Mode)
                    {
                        case Options.RegistryMode.RegServer:
                            LocalServer.RegisterToLocalMachine(serverClassGuid, progId, exePath, tlbPath);
                            return true;
                        case Options.RegistryMode.RegServerPerUser:
                            LocalServer.RegisterToCurrentUser(serverClassGuid, progId, exePath, tlbPath);
                            return true;
                        case Options.RegistryMode.UnRegServer:
                            LocalServer.UnregisterFromLocalMachine(serverClassGuid, progId, tlbPath);
                            return true;
                        case Options.RegistryMode.UnRegServerPerUser:
                            LocalServer.UnregisterToCurrentUser(serverClassGuid, progId, tlbPath);
                            return true;
                        case Options.RegistryMode.Unknown:
                            goto default;
                        default:
                            return StartServer(serverClassGuid);
                    }
                }
                finally
                {
                    Trace.Listeners.Remove(consoleTrace);
                }
            }
        }
        static bool StartServer(Guid serverClassGuid)
        {
            Trace.WriteLine("StartServer");
            using (var server = new LocalServer())
            {
                server.RegisterClass<HidemaruLspBackEndServer>(serverClassGuid);
                if (IsConsoleApplication())
                {
                    WaitServerUsingStdio();
                }
                else
                {
                    WaitServerUsingEvent();
                }
            }
            Trace.WriteLine("[Finish]exe");
            return true;
        }
        static void WaitServerUsingEvent()
        {
            Trace.WriteLine($"================================");
            Trace.WriteLine($"Main thread is Sleeping.");
            Trace.WriteLine($"================================");
            Trace.Flush();
            var autoEvent = new AutoResetEvent(false);
            autoEvent.WaitOne();
        }
        static void WaitServerUsingStdio()
        {
            Trace.WriteLine($"================================");
            Trace.WriteLine($"Press ENTER to exit.");
            Trace.WriteLine($"================================");
            Trace.Flush();
            Console.ReadLine();
        }



        static bool IsConsoleApplication()
        {
            try
            {
                var _ = Console.BufferWidth;
            }
            catch (IOException)
            {
                //Windows application.
                return false;
            }
            //Console application.
            return true;
        }
    }
}

