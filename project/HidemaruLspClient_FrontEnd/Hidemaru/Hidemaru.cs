using System;
using System.Runtime.InteropServices;
using System.Text;
using static HidemaruLspClient_FrontEnd.UnsafeNativeMethods;

namespace HidemaruLspClient_FrontEnd
{
	/// <summary>
	/// 秀丸エディタの機能を呼び出す
	/// （重要）秀丸エディタはマルチスレッドに対応していないため、各機能はメインスレッドから呼びだすこと。
	/// </summary>
    class Hidemaru
    {
		static bool initialized_ = false;

		public struct Position
        {
			public int column;
			public int line;

			public Position(int column, int line)
            {
                this.column = column;
                this.line = line;
            }

            public override bool Equals(object obj)
            {
                return obj is Position position &&
                       column == position.column &&
                       line == position.line;
            }

            public override int GetHashCode()
            {
                int hashCode = 74363238;
                hashCode = hashCode * -1521134295 + column.GetHashCode();
                hashCode = hashCode * -1521134295 + line.GetHashCode();
                return hashCode;
            }

            public static bool operator ==(Position left, Position right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Position left, Position right)
            {
                return !(left == right);
            }
			public bool ValueIsCorrect()
            {
				if((this.column<=0) || (this.line <= 0))
                {
					return false;
                }
				return true;
            }
        }


        //const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        #region Hidemaru-API
        delegate IntPtr Delegate_Hidemaru_GetTotalTextUnicode();
        static Delegate_Hidemaru_GetTotalTextUnicode Hidemaru_GetTotalTextUnicode;

		public delegate  IntPtr Delegate_Hidemaru_GetCurrentWindowHandle();
        public static Delegate_Hidemaru_GetCurrentWindowHandle Hidemaru_GetCurrentWindowHandle;

		public delegate bool Delegate_Hidemaru_GetCursorPosUnicodeFromMousePos(ref NativeMethods.POINT ppt, ref int pnLineNo, ref int pnColumn);
		public static Delegate_Hidemaru_GetCursorPosUnicodeFromMousePos Hidemaru_GetCursorPosUnicodeFromMousePos;

		public delegate bool Delegate_Hidemaru_GetCursorPosUnicode(ref int pnLineNo, ref int pnColumn);
		public static Delegate_Hidemaru_GetCursorPosUnicode Hidemaru_GetCursorPosUnicode;


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
			IntPtr hmod = UnsafeNativeMethods.GetModuleHandle(null); //hidemaru.exe自身

            Hidemaru_GetTotalTextUnicode             = (Delegate_Hidemaru_GetTotalTextUnicode)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hmod, "Hidemaru_GetTotalTextUnicode"), typeof(Delegate_Hidemaru_GetTotalTextUnicode));
			Hidemaru_GetCurrentWindowHandle          = (Delegate_Hidemaru_GetCurrentWindowHandle)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hmod, "Hidemaru_GetCurrentWindowHandle"), typeof(Delegate_Hidemaru_GetCurrentWindowHandle));
			Hidemaru_GetCursorPosUnicodeFromMousePos = (Delegate_Hidemaru_GetCursorPosUnicodeFromMousePos)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hmod, "Hidemaru_GetCursorPosUnicodeFromMousePos"), typeof(Delegate_Hidemaru_GetCursorPosUnicodeFromMousePos));
			Hidemaru_GetCursorPosUnicode             = (Delegate_Hidemaru_GetCursorPosUnicode)Marshal.GetDelegateForFunctionPointer(GetProcAddress(hmod, "Hidemaru_GetCursorPosUnicode"), typeof(Delegate_Hidemaru_GetCursorPosUnicode));
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
		public static string GetFileFullPath()
        {
			var hwndHidemaru = Hidemaru_GetCurrentWindowHandle();
			var sb = new StringBuilder(512);
			var cwch = SendMessage(hwndHidemaru, Constant.WM_HIDEMARUINFO, new IntPtr(Constant.HIDEMARUINFO_GETFILEFULLPATH), sb);
			return sb.ToString();
		}
		public static void HidemaruToZeroBase(out long zerobaseLine, out long zerobaseCharacter, long hidemaruLine,  long hidemaruColumn)
        {
			zerobaseLine      = hidemaruLine	- 1;
			zerobaseCharacter = hidemaruColumn	/*- 1*/;
        }
		public static void ZeroBaseToHidemaru(out long hidemaruLine, out long hidemaruColumn, long zerobaseLine,long zerobaseCharacter)
		{
			hidemaruLine   =zerobaseLine		+ 1;
			hidemaruColumn =zerobaseCharacter	/*+ 1*/;
		}
	}
}
