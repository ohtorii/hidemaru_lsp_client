using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LSP.Client
{
	class Handler
	{
        Protocol response;
        Task Runner;

		public Handler(Protocol.InitializeParameter param, CancellationToken token)
        {
            response = new Protocol(param,token);
        }               

        public void StoreBuffer(byte[] streamString)
		{
			if (response.StoreBuffer(streamString) == false)
			{
                return;
			}
			if (Runner == null)
			{
                Runner = Task.Run(() => response.Parse());
            }
        }
        public void StoreResponse(int id, Action<JToken> callback)
		{
            response.StoreJob(id, callback);
		}
        
    }
}
