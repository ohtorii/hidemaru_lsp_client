using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Client
{
	public class RequestId
	{
		public RequestId(int id)
        {
			id_ = id;
        }		
		public override int GetHashCode()
        {
			return id_;
        }
		public override bool Equals(object obj)
        {
			var other = obj as RequestId;
			if (other == null) return false;
			return this.id_ == other.id_;
		}

		public int intId
		{
			get
			{
				return id_;
			}
		}
		int id_;
	}

	class RequestIdGenerator
	{
		
		int id_ = 1;
		public RequestId NextId()
		{
			var ret = id_;
			id_++;
			return new RequestId(ret);
		}
	}
}
