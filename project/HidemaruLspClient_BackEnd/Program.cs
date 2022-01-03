using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using COMRegistration;
using NLog;
using HidemaruLspClient_BackEndContract;


namespace HidemaruLspClient
{
    partial class Program
    {
        static string applicationName             = System.AppDomain.CurrentDomain.FriendlyName;
        static DllAssemblyResolver dasmr          = new DllAssemblyResolver();
        static readonly string tlbPath            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HidemaruLspClient_BackEndContract.tlb");
        static readonly bool isConsoleApplication = IsConsoleApplication();

        static int Main(string[] args)
        {
            const int success = 0;
            const int error   = 1;
            using (var tracer = new LoggingOutToOneLocation(new ConsoleTraceListener(), new NLogTraceListener {Name=applicationName }))
            {
                Trace.AutoFlush = true;
                Trace.Listeners.Add(tracer);
                try
                {
                    Trace.WriteLine("======== Main ========");
                    DumpArgs(args);
                    tracer.WriteLine(string.Format("isConsoleApplication={0}", isConsoleApplication));
                    Options options;
                    if (LaunchedViaCoCreateInstance(args))
                    {
                        options = Options.Default;
                    }
                    else
                    {
                        options = Options.Create(isConsoleApplication, args);
                        if (options == null)
                        {
                            //ヘルプが表示された。
                            return success;
                        }
                    }
                    return Start(options) ? success : error;
                }
                catch(Exception e)
                {
                    Trace.TraceError(e.ToString());
                }
                finally
                {
                    Trace.Listeners.Remove(tracer);
                }
            }
            return error;
        }
        /// <summary>
        /// 引数をダンプする
        /// </summary>
        /// <param name="args"></param>
        static void DumpArgs(string[] args)
        {
            if (args.Any())
            {
                Trace.WriteLine(string.Format("args[{0}]={{{1}}}", args.Count(), string.Join(',', args)));
            }
            else
            {
                Trace.WriteLine(string.Format("args[{0}]={{}}", 0));
            }
        }
        /// <summary>
        /// このプロセスがCoCreateInstance経由で起動されたかどうか
        /// </summary>
        /// <param name="args">Mainに渡されたコマンドライン引数</param>
        /// <returns></returns>
        static bool LaunchedViaCoCreateInstance(string[] args){
            if((args.Count() == 1) && (args[0] == "-Embedding")){
                return true;
            }
            return false;
        }
        static bool Start(Options options) {
            if (!File.Exists(tlbPath))
            {
                Trace.WriteLine($"Not found {tlbPath}");
                return false;
            }

            var serverClassGuid = new Guid((Attribute.GetCustomAttribute(typeof(ServerClass), typeof(GuidAttribute)) as GuidAttribute).Value);
            var exePath         = Process.GetCurrentProcess().MainModule.FileName;
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
        static bool StartServer(Guid serverClassGuid)
        {
            Trace.WriteLine("[Enter]StartServer");
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
            Trace.WriteLine("[Leave]StartServer");
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

