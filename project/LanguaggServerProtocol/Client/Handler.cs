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
        Protocol protocol_;
        Task runner_;

		public Handler(Protocol.InitializeParameter param, CancellationToken token)
        {
            protocol_ = new Protocol(param,token);
        }               

        public void StoreBuffer(byte[] streamString)
		{
			if (protocol_.StoreBuffer(streamString) == false)
			{
                return;
			}
			if (runner_ == null)
			{
                runner_ = Task.Run(() => protocol_.Parse());
            }
        }
        public void StoreResponse(int id, Action<JToken> callback)
		{
            protocol_.StoreJob(id, callback);
		}
        
    }
}
