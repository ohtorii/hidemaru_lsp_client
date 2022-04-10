using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace LSP.Implementation
{
	class ServerProcess
	{
        public event EventHandler<byte[]> standardOutputReceived {
			add
			{
                lock (this.standardOutput_)
                {
                    this.standardOutput_.DataReceived += value;
                }
			}
			remove
			{
                lock (this.standardOutput_)
                {
                    this.standardOutput_.DataReceived -= value;
                }
            }
        }
        public event EventHandler<byte[]> standardErrorReceived
        {
            add
            {
                lock (this.standardError_)
                {
                    this.standardError_.DataReceived += value;
                }
            }
            remove
            {
                lock (this.standardError_)
                {
                    this.standardError_.DataReceived -= value;
                }
            }
        }
        public event EventHandler Exited
		{
            add
            {
                lock (process_)
                {
                    process_.Exited += value;
                }
            }
			remove
			{
                lock (process_)
                {
                    process_.Exited -= value;
                }
            }
		}
        public bool HasExited { get { return process_.HasExited; } }

        private ProcessStartInfo processStartInfo_ = null;
        private Process process_ = null;
        private AsyncStreamReader standardOutput_ =null;
        private AsyncStreamReader standardError_ = null;

        public ServerProcess(string filename, string arguments, string WorkingDirectory)
		{
            standardOutput_ = new AsyncStreamReader();
            standardError_ = new AsyncStreamReader();
            if ((WorkingDirectory == null)||(WorkingDirectory.Length==0))
            {
                WorkingDirectory = System.IO.Path.GetDirectoryName(filename);
            }
            processStartInfo_ = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,

                RedirectStandardInput = true,

                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,

                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.UTF8,

                WorkingDirectory= WorkingDirectory,
                CreateNoWindow  = true,
                UseShellExecute = false,
            };
        }
		public void StartProcess()
		{
			if (process_ != null)
			{
                return;
			}
            process_ = Process.Start(processStartInfo_);
            standardOutput_.SetStreamReader(process_.StandardOutput);
            standardError_.SetStreamReader(process_.StandardError);
        }
        public void StartRedirect()
        {
            standardOutput_.Start();
            standardError_.Start();
        }
        public void StartThreadLoop()
        {
            var _ = Task.Run(async () =>
            {
                while (process_.HasExited == false)
                {
                    await Task.Delay(10);
                    if (standardOutput_.Active == false)
                    {
                        standardOutput_.Start();
                    }
                    if (standardError_.Active == false)
                    {
                        standardError_.Start();
                    }
                }
            });
        }
        public void WaitForExit()
		{
            process_.WaitForExit();
		}
        public bool WaitForExit(int milliseconds)
		{
            return process_.WaitForExit(milliseconds);
        }
        public void WriteStandardInput(byte[] b)
		{
            if (process_.HasExited)
            {
                throw new Exception("LSP process has been exited.");
            }
            process_.StandardInput.BaseStream.Write(b,0,b.Length);
            process_.StandardInput.BaseStream.Flush();
        }
        public void Kill()
		{
            process_.Kill();
		}
        public int GetExitCode()
        {
            return process_.ExitCode;
        }
    }
}
