
using System;
using System.IO;
using System.Threading;


namespace ClientExample
{

    class Program
	{

        static void Main(string[] args)
		{
			//var o = new CSharpClient();
			//o.Start();

			var o = new PythonClient();
			o.Start();

			//CppClient.Start();
			//VimScriptClient.Start();
			//LuaClient.Start();
		}

    }    
}
