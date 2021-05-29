using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Client
{
	class RequestIdGenerator
	{
		int id = 1;
		public int NextId()
		{
			var ret = id;
			id++;
			return ret;
		}
	}
}
