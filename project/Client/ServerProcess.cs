using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LSP.Client
{
	class ServerProcess
	{
        public event EventHandler<byte[]> standardOutputReceived {
			add
			{
                lock (this.standardOutput)
                {
                    this.standardOutput.DataReceived += value;
                }
			}
			remove
			{
                lock (this.standardOutput)
                {
                    this.standardOutput.DataReceived -= value;
                }
            }
        }
        public event EventHandler<byte[]> standardErrorReceived
        {
            add
            {
                lock (this.standardError)
                {
                    this.standardError.DataReceived += value;
                }
            }
            remove
            {
                lock (this.standardError)
                {
                    this.standardError.DataReceived -= value;
                }
            }
        }
        public event EventHandler Exited
		{
            add 
            {
                lock (process)
                {
                    process.Exited += value;
                }
            }
			remove
			{
                lock (process)
                {
                    process.Exited -= value;
                }
            }
		}
        public bool HasExited { get { return process.HasExited; } }
        
        ProcessStartInfo processStartInfo;
        Process process;
        AsyncStreamReader standardOutput;
        AsyncStreamReader standardError;

        public ServerProcess(string filename, string arguments)
		{
            standardOutput = new AsyncStreamReader();
            standardError = new AsyncStreamReader();

            processStartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,

                RedirectStandardInput = true,

                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,

                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.UTF8,

                UseShellExecute = false,
            };
        }
		public void StartProcess()
		{
			if (process != null)
			{
                return;
			}
            process = Process.Start(processStartInfo);
            standardOutput.SetStreamReader(process.StandardOutput);
            standardError.SetStreamReader(process.StandardError);            
        }
        public void StartRedirect()
        {
            standardOutput.Start();
            standardError.Start();
        }
        public void StartThreadLoop() 
        {
            Task.Run(() =>
            {
                while (process.HasExited == false)
                {
                    Thread.Sleep(10);
                    if (standardOutput.Active == false)
                    {
                        standardOutput.Start();
                    }
                    if (standardError.Active == false)
                    {
                        standardError.Start();
                    }
                }
            });
        }
        public void WaitForExit()
		{
            process.WaitForExit();
		}
        public bool WaitForExit(int milliseconds)
		{
            return process.WaitForExit(milliseconds);
        }
        public void WriteStandardInput(string s)
		{
            process.StandardInput.Write(s);
		}
        public void WriteLineStandardInput(string s)
        {
            process.StandardInput.WriteLine(s);
        }
        public void Kill()
		{
            process.Kill();
		}
    }
}
