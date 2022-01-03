using System.Collections.Generic;
using System.Diagnostics;

namespace HidemaruLspClient
{
    partial class Program
    {
        /// <summary>
        /// ログを一カ所へ出力する
        /// </summary>
        class LoggingOutToOneLocation : TraceListener
        {
            List<TraceListener> Listners;// = new TraceListener[] {new ConsoleTraceListener(), new NLogTraceListener()};
            public LoggingOutToOneLocation() {
                Listners = new List<TraceListener>();
            }
            public LoggingOutToOneLocation(TraceListener lister0)
            {
                Listners = new List<TraceListener>();
                Listners.Add(lister0);
            }
            public LoggingOutToOneLocation(TraceListener listner0, TraceListener listner1)
            {
                Listners = new List<TraceListener>();
                Listners.Add(listner0);
                Listners.Add(listner1);
            }
            public LoggingOutToOneLocation(TraceListener listner0, TraceListener listner1, TraceListener listner2)
            {
                Listners = new List<TraceListener>();
                Listners.Add(listner0);
                Listners.Add(listner1);
                Listners.Add(listner2);
            }
            public void AddListner(TraceListener listner)
            {
                Listners.Add(listner);
            }
            public void Remove(TraceListener listner)
            {
                Listners.Remove(listner);
            }

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

