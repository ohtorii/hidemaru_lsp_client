using System.Diagnostics;
using NLog;

namespace HidemaruLspClient
{
    partial class Program
    {
        class LoggingOutToOneLocation : TraceListener
        {
            TraceListener[] Listners = new TraceListener[] {new ConsoleTraceListener(), new NLogTraceListener()};

            public override void Write(string message)
            {
                foreach(var l in Listners)
                {
                    l.Write(message);
                }
            }

            public override void WriteLine(string message)
            {
                foreach (var l in Listners)
                {
                    l.WriteLine(message);
                }
            }
        }
    }
}

