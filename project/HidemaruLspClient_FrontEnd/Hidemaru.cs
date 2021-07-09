using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{
    class Hidemaru
    {
		static bool initialized_ = false;
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string lpFileName);
		
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalLock(IntPtr hMem);
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GlobalUnlock(IntPtr hMem);
		[DllImport("kernel32.dll")]
		static extern IntPtr GlobalFree(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr handle);

		const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
		delegate IntPtr Delegate_Hidemaru_GetTotalTextUnicode();
		static Delegate_Hidemaru_GetTotalTextUnicode Hidemaru_GetTotalTextUnicode;

		static public void Initialize()
        {
            if (initialized_)
            {
				return;
            }
			IntPtr hmod = GetModuleHandle(null); //hidemaru.exe自身
			IntPtr pfnHidemaru_GetCurrentWindowHandle = GetProcAddress(hmod, "Hidemaru_GetTotalTextUnicode");
			//Debug.Assert(pfnHidemaru_GetCurrentWindowHandle != IntPtr.Zero);
            Hidemaru_GetTotalTextUnicode = (Delegate_Hidemaru_GetTotalTextUnicode)Marshal.GetDelegateForFunctionPointer(pfnHidemaru_GetCurrentWindowHandle, typeof(Delegate_Hidemaru_GetTotalTextUnicode));
			initialized_ = true;
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
