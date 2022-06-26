using System;
using System.Runtime.InteropServices;
using static HidemaruLspClient_FrontEnd.Native.UnsafeNativeMethods;

namespace HidemaruLspClient_FrontEnd.Hidemaru
{
    class OutputPane
    {
        [DllImport("HmOutputPane.dll")]
        public static extern void Output(IntPtr hwndHidemaru, byte[] text);
        [DllImport("HmOutputPane.dll", CharSet = CharSet.Unicode)]
        public static extern void OutputW(IntPtr hwndHidemaru, string text);
        [DllImport("HmOutputPane.dll")]
        public static extern IntPtr GetWindowHandle(IntPtr hwndHidemaru);
        public static void Clear(IntPtr hwndHidemaru)
        {
            //0x111=WM_COMMAND
            //1009=クリア
            SendMessage(GetWindowHandle(hwndHidemaru), 0x111, 1009, new IntPtr(0));
        }
    }
}
