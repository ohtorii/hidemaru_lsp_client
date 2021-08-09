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

		


        //const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        #region Hidemaru-API
        public delegate IntPtr Delegate_Hidemaru_GetTotalTextUnicode();
        public static Delegate_Hidemaru_GetTotalTextUnicode Hidemaru_GetTotalTextUnicode;
		
		public delegate  IntPtr Delegate_Hidemaru_GetCurrentWindowHandle();
        public static Delegate_Hidemaru_GetCurrentWindowHandle Hidemaru_GetCurrentWindowHandle;

		#endregion


		class Constant
        {
			public const int WM_USER = 0x400;
			public const int WM_HIDEMARUINFO = WM_USER + 181;			

			public const int HIDEMARUINFO_GETTABWIDTH = 0;
			public const int HIDEMARUINFO_GETSTARTCOLUMN = 1;
			public const int HIDEMARUINFO_GETSPACETAB = 2;
			public const int HIDEMARUINFO_GETMANUALTAB = 3;
			public const int HIDEMARUINFO_GETFILEFULLPATH = 4;
		}

		static public void Initialize()
        {
            if (initialized_)
            {
				return;
            }
			IntPtr hmod = Dll.GetModuleHandle(null); //hidemaru.exe自身
			
            Hidemaru_GetTotalTextUnicode   = (Delegate_Hidemaru_GetTotalTextUnicode)Marshal.GetDelegateForFunctionPointer(Dll.GetProcAddress(hmod, "Hidemaru_GetTotalTextUnicode"), typeof(Delegate_Hidemaru_GetTotalTextUnicode));			
			Hidemaru_GetCurrentWindowHandle= (Delegate_Hidemaru_GetCurrentWindowHandle)Marshal.GetDelegateForFunctionPointer(Dll.GetProcAddress(hmod, "Hidemaru_GetCurrentWindowHandle"), typeof(Delegate_Hidemaru_GetCurrentWindowHandle));

			initialized_ = true;
		}
        public static string GetTotalTextUnicode()
        {
			string result="";
			var hGlobal = Hidemaru_GetTotalTextUnicode();
			if (hGlobal != IntPtr.Zero)
			{
				var pwsz = Dll.GlobalLock(hGlobal);
				if (pwsz != IntPtr.Zero)
				{
					result = Marshal.PtrToStringUni(pwsz);
					Dll.GlobalUnlock(hGlobal);
				}
				Dll.GlobalFree(hGlobal);
			}
			return result;
		}
		public static string GetFileFullPath()
        {
			var hwndHidemaru = Hidemaru_GetCurrentWindowHandle();
			var sb = new StringBuilder(512);
			var cwch = Dll.SendMessage(hwndHidemaru, Constant.WM_HIDEMARUINFO, new IntPtr(Constant.HIDEMARUINFO_GETFILEFULLPATH), sb);
			return sb.ToString();
		}
	}
}
