using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{
    class Dll
    {
		#region kernel32.dll
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string lpFileName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GlobalLock(IntPtr hMem);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GlobalUnlock(IntPtr hMem);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GlobalFree(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr handle);
		#endregion


		#region user32.dll
		[DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

		[DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, StringBuilder wParam, StringBuilder lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int command, IntPtr lparam);
        #endregion
    }
}
