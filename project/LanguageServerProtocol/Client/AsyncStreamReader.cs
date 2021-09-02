using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Implementation
{

    /*
     * https://stackoverflow.com/questions/4501511/c-sharp-realtime-console-output-redirection
     */

    /// <summary>
    /// Stream reader for StandardOutput and StandardError stream readers
    /// Runs an eternal BeginRead loop on the underlaying stream bypassing the stream reader.
    ///
    /// The TextReceived sends data received on the stream in non delimited chunks. Event subscriber can
    /// then split on newline characters etc as desired.
    /// </summary>
    class AsyncStreamReader
    {
        public event EventHandler<byte[]> DataReceived;
        protected readonly byte[] buffer = new byte[4096];
        private StreamReader reader;

        public AsyncStreamReader()
        {
            this.reader = null;
            this.Active = false;
        }
        public void SetStreamReader(StreamReader readerToBypass)
		{
            this.reader = readerToBypass;
        }
        /// <summary>
        ///  If AsyncStreamReader is active
        /// </summary>
        public bool Active { get; private set; }
        public void Start()
        {
            if (Active == false)
            {
                Active = true;
                BeginReadAsync();
            }
        }
        public void Stop()
        {
            if (Active)
            {
                Active = false;
                reader.DiscardBufferedData();
            }
        }
        protected void BeginReadAsync()
        {
            if (this.Active)
            {
                reader.BaseStream.BeginRead(this.buffer, 0, this.buffer.Length, new AsyncCallback(ReadCallback), null);
            }
        }
        private void ReadCallback(IAsyncResult asyncResult)
        {
            var bytesRead = reader.BaseStream.EndRead(asyncResult);
			if (bytesRead <= 0)
			{
                //callback without data - stop async
                Stop();
                return;
            }
            //Send data to event subscriber - null if no longer active
            if (this.DataReceived != null)
            {
                var dst = new byte[bytesRead];
                Array.Copy(this.buffer,dst,bytesRead);
                this.DataReceived.Invoke(this, dst);
            }

            //Wait for more data from stream
            this.BeginReadAsync();
        }
    }
}

