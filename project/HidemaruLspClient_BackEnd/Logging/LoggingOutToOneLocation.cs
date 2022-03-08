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

            public override void Write(string? message)
            {
                foreach(var l in Listners)
                {
                    l.Write(message);
                }
            }

            public override void WriteLine(string? message)
            {
                foreach (var l in Listners)
                {
                    l.WriteLine(message);
                }
            }

            public override void Close()
            {
                foreach (var l in Listners)
                {
                    l.Close();
                }
            }
            public override void Fail(string? message)
            {
                foreach (var l in Listners)
                {
                    l.Fail(message);
                }
            }
            public override void Fail(string? message, string? detailMessage)
            {
                foreach (var l in Listners)
                {
                    l.Fail(message, detailMessage);
                }
            }
            public override void Flush()
            {
                foreach (var l in Listners)
                {
                    l.Flush();
                }
            }
            public override void TraceData(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, object? data)
            {
                foreach (var l in Listners)
                {
                    l.TraceData(eventCache, source, eventType, id, data);
                }
            }
            public override void TraceData(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, params object?[]? data)
            {
                foreach (var l in Listners)
                {
                    l.TraceData(eventCache, source, eventType, id, data);
                }
            }
            public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id)
            {
                foreach (var l in Listners)
                {
                    l.TraceEvent(eventCache, source, eventType, id);
                }
            }
            public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? message)
            {
                foreach (var l in Listners)
                {
                    l.TraceEvent(eventCache, source, eventType,id, message);
                }
            }
            public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string format, params object?[]? args)
            {
                foreach (var l in Listners)
                {
                    l.TraceEvent(eventCache, source,eventType,id,format, args);
                }
            }
            public override void TraceTransfer(TraceEventCache? eventCache, string source, int id, string? message, System.Guid relatedActivityId)
            {
                foreach (var l in Listners)
                {
                    l.TraceTransfer(eventCache, source, id,message, relatedActivityId);
                }
            }
            public override void Write(object? o)
            {
                foreach (var l in Listners)
                {
                    l.Write(o);
                }
            }
            public override void Write(string? message, string? category)
            {
                foreach (var l in Listners)
                {
                    l.Write(message, category);
                }
            }
            public override void Write(object? o, string? category)
            {
                foreach (var l in Listners)
                {
                    l.Write(o,category);
                }
            }
            public override void WriteLine(string? message, string? category)
            {
                foreach (var l in Listners)
                {
                    l.WriteLine(message,category);
                }
            }

            public override void WriteLine(object? o, string? category)
            {
                foreach (var l in Listners)
                {
                    l.WriteLine(o, category);
                }
            }
            public override void WriteLine(object? o)
            {
                foreach (var l in Listners)
                {
                    l.WriteLine(o);
                }
            }
            /*protected override void Dispose(bool disposing)
            {
                foreach (var l in Listners)
                {
                    l.Dispose(disposing);
                }
            }*/

            /*protected override string[]? GetSupportedAttributes()
            {
            foreach (var l in Listners)
{
    l.;
}
            }*/
            /*protected override void WriteIndent()
            {
                foreach (var l in Listners)
                {
                    l.WriteIndent();
                }
            }*/
        }
    }
}

