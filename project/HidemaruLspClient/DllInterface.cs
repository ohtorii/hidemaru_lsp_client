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
		[DllExport]
		public static void StartServer(IntPtr serverConfigFilename)
        {
			Holder.StartServer(Marshal.PtrToStringAuto(serverConfigFilename));
		}
		[DllExport]
		public static void FileOpen(IntPtr absFilename)
        {

        }
		[DllExport]
		public static IntPtr Completion(IntPtr column, IntPtr line)
		{
			return new IntPtr(1);
		}
	}
}
