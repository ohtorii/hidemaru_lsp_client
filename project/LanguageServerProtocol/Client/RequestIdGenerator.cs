using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Client
{
	class RequestIdGenerator
	{
		int id_ = 1;
		public int NextId()
		{
			var ret = id_;
			id_++;
			return ret;
		}
	}
}
