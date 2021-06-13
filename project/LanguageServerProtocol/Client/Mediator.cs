using LSP.Model;
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
	class Mediator
	{
		public Protocol Protocol
		{
			get {
                return protocol_;
            } 
        }
        Protocol protocol_;
        Task runner_;

		public Mediator(CancellationToken token)
        {
            protocol_ = new Protocol(token);
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
        public void StoreResponse(RequestId id, Action<ResponseMessage> callback)
		{
            protocol_.StoreJob(id, callback);
		}
        
    }
}
