using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Model
{
    interface IRequestMessage : IMessage
    {
        int id { get; set; }
        string method { get; set; }
        object @params { get; set; }
    }
	class RequestMessage : IRequestMessage
	{
		public string jsonrpc { get; set; } = "2.0";
		public int id { get; set; }
		public string method { get; set; }
		public object @params { get; set; }
	}
}
