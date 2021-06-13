using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    public class DllInterface
    {
        static readonly IntPtr True  = (IntPtr)1;   //1==True
		static readonly IntPtr False = (IntPtr)0;   //0==False

		[DllExport]
		public static IntPtr StartServer(IntPtr serverConfigFilename)
        {
            if (Holder.StartServer(Marshal.PtrToStringAuto(serverConfigFilename)))
            {
                return True;
            }
			return False;
		}
		[DllExport]
		public static IntPtr DigOpen(IntPtr absFilename)
        {
            if (Holder.DigOpen(Marshal.PtrToStringAuto(absFilename)))
            {
                return True;
            }
            return False;
        }
		[DllExport]
		public static IntPtr Completion(IntPtr absFilename, IntPtr line, IntPtr column)
		{
            var intLine = line.ToInt32();
            var intColumn = column.ToInt32();
            if (intLine < 0)
            {
                return False;
            }
            if (intColumn < 0)
            {
                return False;
            }
            Holder.Completion(Marshal.PtrToStringAuto(absFilename), (uint)intLine,(uint)intColumn);
            /*if ()
            {
                return True;
            }*/
            return False;
		}
	}
}
