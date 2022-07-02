using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient.Native
{
    internal class User32
    {
        public enum MessageBoxResult : uint
        {
            Ok = 1,
            Cancel,
            Abort,
            Retry,
            Ignore,
            Yes,
            No,
            Close,
            Help,
            TryAgain,
            Continue,
            Timeout = 32000
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern MessageBoxResult MessageBox(IntPtr hWnd, string text, string caption, int options);

    }
}
