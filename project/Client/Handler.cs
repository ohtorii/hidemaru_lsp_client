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
        Response response;
        Task Runner;

		public Handler(Response.InitializeParameter param, CancellationToken token)
        {
            response = new Response(param,token);
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
        public void StoreCallback(int id, Action<JObject> callback)
		{
            response.StoreJob(id, callback);
		}
        
    }
}
