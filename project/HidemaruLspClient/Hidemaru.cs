using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class Hidemaru
    {
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string lpFileName);
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalLock(IntPtr hMem);
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GlobalUnlock(IntPtr hMem);
		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalFree(IntPtr hMem);


		delegate IntPtr Delegate_Hidemaru_GetTotalTextUnicode();
		static Delegate_Hidemaru_GetTotalTextUnicode Hidemaru_GetTotalTextUnicode;

		static public void Initialize()
        {
            IntPtr hmod = GetModuleHandle(null); //hidemaru.exe自身

			IntPtr pfnHidemaru_GetCurrentWindowHandle = GetProcAddress(hmod, "Hidemaru_GetTotalTextUnicode");
            Hidemaru_GetTotalTextUnicode = (Delegate_Hidemaru_GetTotalTextUnicode)Marshal.GetDelegateForFunctionPointer(pfnHidemaru_GetCurrentWindowHandle, typeof(Delegate_Hidemaru_GetTotalTextUnicode));

		}
        public static string GetTotalTextUnicode()
        {
			string result="";
			var hGlobal = Hidemaru_GetTotalTextUnicode();
			if (hGlobal != IntPtr.Zero)
			{
				var pwsz = GlobalLock(hGlobal);
				if (pwsz != IntPtr.Zero)
				{
					result = Marshal.PtrToStringUni(pwsz);
					GlobalUnlock(hGlobal);
				}
				GlobalFree(hGlobal);
			}
			return result;
		}
	}
}
