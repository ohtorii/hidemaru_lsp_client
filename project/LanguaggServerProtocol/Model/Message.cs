using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Model
{
    interface IMessage
	{
        string jsonrpc { get; set; }
    }
    /*class Message: IMessage
    {
        string IMessage.jsonrpc { get; set; } = "2.0";
    } */  
}
