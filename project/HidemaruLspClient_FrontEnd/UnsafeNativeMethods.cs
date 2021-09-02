using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static HidemaruLspClient_FrontEnd.NativeMethods;

namespace HidemaruLspClient_FrontEnd
{
    class UnsafeNativeMethods
    {

		#region comctl32.dll
		[DllImport("comctl32.dll", EntryPoint = "InitCommonControls", CallingConvention = CallingConvention.StdCall)]
		static extern bool InitCommonControls();
		#endregion

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
		[DllImport("user32.dll")]
		public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,uint wMsgFilterMax);

		[DllImport("user32.dll")]
		public static extern bool TranslateMessage([In] ref MSG lpMsg);

		[DllImport("user32.dll")]
		public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyWindow(IntPtr hwnd);

		[DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Auto)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

		[DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Auto)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, StringBuilder wParam, StringBuilder lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, NativeMethods.TOOLINFO lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		[DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int command, IntPtr lparam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetCursorPos(ref POINT lpPoint);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
		#endregion
	}
}

