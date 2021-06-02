using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Client
{
	interface ILogger
	{
		void Trace(string message);
		void Debug(string message);
		void Info(string message);
		void Warn(string message);
		void Error(string message);
		void Fatal(string message);
	}

	class NullLogger : ILogger
	{
		public void Debug(string message)
		{			
		}

		public void Error(string message)
		{		
		}

		public void Fatal(string message)
		{		
		}

		public void Info(string message)
		{		
		}

		public void Trace(string message)
		{		
		}

		public void Warn(string message)
		{		
		}
	}
}
