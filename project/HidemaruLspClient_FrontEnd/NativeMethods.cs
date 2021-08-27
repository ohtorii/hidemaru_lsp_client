using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{
    class NativeMethods
    {
        /*
         * NOTE: All Message Numbers below 0x0400 are RESERVED.
         *
         * Private Window Messages Start Here:
         */
        const uint WM_USER = 0x0400;


        public const string TOOLTIPS_CLASS = "tooltips_class32";
        public const int TTTOOLINFOW_V2_SIZE = 0x0000002c;
        public const int TTTOOLINFOW_V3_SIZE = 0x00000030;

        [Flags]
        public enum ToolTipFlags : uint
        {
            TTF_IDISHWND = 0x0001,
            TTF_CENTERTIP = 0x0002,
            TTF_RTLREADING = 0x0004,
            TTF_SUBCLASS = 0x0010,
            TTF_TRACK = 0x0020,
            TTF_ABSOLUTE = 0x0080,
            TTF_TRANSPARENT = 0x0100,
            TTF_PARSELINKS = 0x1000,
            TTF_DI_SETITEM = 0x8000,       // valid only on the TTN_NEEDTEXT callback
        }

        enum TTM : uint
        {
            TTM_ACTIVATE = (WM_USER + 1),
            TTM_SETDELAYTIME = (WM_USER + 3),
            TTM_ADDTOOLA = (WM_USER + 4),
            TTM_ADDTOOLW = (WM_USER + 50),
            TTM_DELTOOLA = (WM_USER + 5),
            TTM_DELTOOLW = (WM_USER + 51),
            TTM_NEWTOOLRECTA = (WM_USER + 6),
            TTM_NEWTOOLRECTW = (WM_USER + 52),
            TTM_RELAYEVENT = (WM_USER + 7), // Win7: wParam = GetMessageExtraInfo() when relaying WM_MOUSEMOVE
            TTM_GETTOOLINFOA = (WM_USER + 8),
            TTM_GETTOOLINFOW = (WM_USER + 53),
            TTM_SETTOOLINFOA = (WM_USER + 9),
            TTM_SETTOOLINFOW = (WM_USER + 54),
            TTM_HITTESTA = (WM_USER + 10),
            TTM_HITTESTW = (WM_USER + 55),
            TTM_GETTEXTA = (WM_USER + 11),
            TTM_GETTEXTW = (WM_USER + 56),
            TTM_UPDATETIPTEXTA = (WM_USER + 12),
            TTM_UPDATETIPTEXTW = (WM_USER + 57),
            TTM_GETTOOLCOUNT = (WM_USER + 13),
            TTM_ENUMTOOLSA = (WM_USER + 14),
            TTM_ENUMTOOLSW = (WM_USER + 58),
            TTM_GETCURRENTTOOLA = (WM_USER + 15),
            TTM_GETCURRENTTOOLW = (WM_USER + 59),
            TTM_WINDOWFROMPOINT = (WM_USER + 16),
            TTM_TRACKACTIVATE = (WM_USER + 17),  // wParam = TRUE/FALSE start end  lparam = LPTOOLINFO
            TTM_TRACKPOSITION = (WM_USER + 18),  // lParam = dwPos
            TTM_SETTIPBKCOLOR = (WM_USER + 19),
            TTM_SETTIPTEXTCOLOR = (WM_USER + 20),
            TTM_GETDELAYTIME = (WM_USER + 21),
            TTM_GETTIPBKCOLOR = (WM_USER + 22),
            TTM_GETTIPTEXTCOLOR = (WM_USER + 23),
            TTM_SETMAXTIPWIDTH = (WM_USER + 24),
            TTM_GETMAXTIPWIDTH = (WM_USER + 25),
            TTM_SETMARGIN = (WM_USER + 26),  // lParam = lprc
            TTM_GETMARGIN = (WM_USER + 27),  // lParam = lprc
            TTM_POP = (WM_USER + 28),
            TTM_UPDATE = (WM_USER + 29),
            TTM_GETBUBBLESIZE = (WM_USER + 30),
            TTM_ADJUSTRECT = (WM_USER + 31),
            TTM_SETTITLEA = (WM_USER + 32),  // wParam = TTI_*, lParam = char* szTitle
            TTM_SETTITLEW = (WM_USER + 33),  // wParam = TTI_*, lParam = wchar* szTitle
        }


        public enum ToolTipMessage : uint
        {
            TTM_ADDTOOL = TTM.TTM_ADDTOOLW,
            TTM_DELTOOL = TTM.TTM_DELTOOLW,
            TTM_NEWTOOLRECT = TTM.TTM_NEWTOOLRECTW,
            TTM_GETTOOLINFO = TTM.TTM_GETTOOLINFOW,
            TTM_SETTOOLINFO = TTM.TTM_SETTOOLINFOW,
            TTM_HITTEST = TTM.TTM_HITTESTW,
            TTM_GETTEXT = TTM.TTM_GETTEXTW,
            TTM_UPDATETIPTEXT = TTM.TTM_UPDATETIPTEXTW,
            TTM_ENUMTOOLS = TTM.TTM_ENUMTOOLSW,
            TTM_GETCURRENTTOOL = TTM.TTM_GETCURRENTTOOLW,
            TTM_SETTITLE = TTM.TTM_SETTITLEW,
            TTM_TRACKPOSITION = TTM.TTM_TRACKPOSITION,
            TTM_TRACKACTIVATE = TTM.TTM_TRACKACTIVATE,
        }
        //		public const uint WS_POPUP		=	0x80000000;
        public const int CW_USEDEFAULT = -1; //0x80000000;

        [Flags]
        public enum WindowStyles : uint
        {
            WS_BORDER = 0x800000,
            WS_CAPTION = 0xc00000,
            WS_CHILD = 0x40000000,
            WS_CLIPCHILDREN = 0x2000000,
            WS_CLIPSIBLINGS = 0x4000000,
            WS_DISABLED = 0x8000000,
            WS_DLGFRAME = 0x400000,
            WS_GROUP = 0x20000,
            WS_HSCROLL = 0x100000,
            WS_MAXIMIZE = 0x1000000,
            WS_MAXIMIZEBOX = 0x10000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x20000,
            WS_OVERLAPPED = 0x0,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP = 0x80000000u,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEFRAME = 0x40000,
            WS_SYSMENU = 0x80000,
            WS_TABSTOP = 0x10000,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x200000,

            TTS_ALWAYSTIP = 0x01,
            TTS_NOPREFIX = 0x02,
            TTS_NOANIMATE = 0x10,
            TTS_NOFADE = 0x20,
            TTS_BALLOON = 0x40,
            TTS_CLOSE = 0x80,

        }
        [Flags]
        public enum WindowStylesEx : uint
        {
            /// <summary>Specifies a window that accepts drag-drop files.</summary>
            WS_EX_ACCEPTFILES = 0x00000010,

            /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
            WS_EX_APPWINDOW = 0x00040000,

            /// <summary>Specifies a window that has a border with a sunken edge.</summary>
            WS_EX_CLIENTEDGE = 0x00000200,

            /// <summary>
            /// Specifies a window that paints all descendants in bottom-to-top painting order using double-buffering.
            /// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. This style is not supported in Windows 2000.
            /// </summary>
            /// <remarks>
            /// With WS_EX_COMPOSITED set, all descendants of a window get bottom-to-top painting order using double-buffering.
            /// Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects,
            /// but only if the descendent window also has the WS_EX_TRANSPARENT bit set.
            /// Double-buffering allows the window and its descendents to be painted without flicker.
            /// </remarks>
            WS_EX_COMPOSITED = 0x02000000,

            /// <summary>
            /// Specifies a window that includes a question mark in the title bar. When the user clicks the question mark,
            /// the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message.
            /// The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command.
            /// The Help application displays a pop-up window that typically contains help for the child window.
            /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
            /// </summary>
            WS_EX_CONTEXTHELP = 0x00000400,

            /// <summary>
            /// Specifies a window which contains child windows that should take part in dialog box navigation.
            /// If this style is specified, the dialog manager recurses into children of this window when performing navigation operations
            /// such as handling the TAB key, an arrow key, or a keyboard mnemonic.
            /// </summary>
            WS_EX_CONTROLPARENT = 0x00010000,

            /// <summary>Specifies a window that has a double border.</summary>
            WS_EX_DLGMODALFRAME = 0x00000001,

            /// <summary>
            /// Specifies a window that is a layered window.
            /// This cannot be used for child windows or if the window has a class style of either CS_OWNDC or CS_CLASSDC.
            /// </summary>
            WS_EX_LAYERED = 0x00080000,

            /// <summary>
            /// Specifies a window with the horizontal origin on the right edge. Increasing horizontal values advance to the left.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_LAYOUTRTL = 0x00400000,

            /// <summary>Specifies a window that has generic left-aligned properties. This is the default.</summary>
            WS_EX_LEFT = 0x00000000,

            /// <summary>
            /// Specifies a window with the vertical scroll bar (if present) to the left of the client area.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_LEFTSCROLLBAR = 0x00004000,

            /// <summary>
            /// Specifies a window that displays text using left-to-right reading-order properties. This is the default.
            /// </summary>
            WS_EX_LTRREADING = 0x00000000,

            /// <summary>
            /// Specifies a multiple-document interface (MDI) child window.
            /// </summary>
            WS_EX_MDICHILD = 0x00000040,

            /// <summary>
            /// Specifies a top-level window created with this style does not become the foreground window when the user clicks it.
            /// The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
            /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
            /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
            /// </summary>
            WS_EX_NOACTIVATE = 0x08000000,

            /// <summary>
            /// Specifies a window which does not pass its window layout to its child windows.
            /// </summary>
            WS_EX_NOINHERITLAYOUT = 0x00100000,

            /// <summary>
            /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
            /// </summary>
            WS_EX_NOPARENTNOTIFY = 0x00000004,

            /// <summary>
            /// The window does not render to a redirection surface.
            /// This is for windows that do not have visible content or that use mechanisms other than surfaces to provide their visual.
            /// </summary>
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000,

            /// <summary>Specifies an overlapped window.</summary>
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

            /// <summary>Specifies a palette window, which is a modeless dialog box that presents an array of commands.</summary>
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

            /// <summary>
            /// Specifies a window that has generic "right-aligned" properties. This depends on the window class.
            /// The shell language must support reading-order alignment for this to take effect.
            /// Using the WS_EX_RIGHT style has the same effect as using the SS_RIGHT (static), ES_RIGHT (edit), and BS_RIGHT/BS_RIGHTBUTTON (button) control styles.
            /// </summary>
            WS_EX_RIGHT = 0x00001000,

            /// <summary>Specifies a window with the vertical scroll bar (if present) to the right of the client area. This is the default.</summary>
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            /// <summary>
            /// Specifies a window that displays text using right-to-left reading-order properties.
            /// The shell language must support reading-order alignment for this to take effect.
            /// </summary>
            WS_EX_RTLREADING = 0x00002000,

            /// <summary>Specifies a window with a three-dimensional border style intended to be used for items that do not accept user input.</summary>
            WS_EX_STATICEDGE = 0x00020000,

            /// <summary>
            /// Specifies a window that is intended to be used as a floating toolbar.
            /// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
            /// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
            /// If a tool window has a system menu, its icon is not displayed on the title bar.
            /// However, you can display the system menu by right-clicking or by typing ALT+SPACE.
            /// </summary>
            WS_EX_TOOLWINDOW = 0x00000080,

            /// <summary>
            /// Specifies a window that should be placed above all non-topmost windows and should stay above them, even when the window is deactivated.
            /// To add or remove this style, use the SetWindowPos function.
            /// </summary>
            WS_EX_TOPMOST = 0x00000008,

            /// <summary>
            /// Specifies a window that should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
            /// The window appears transparent because the bits of underlying sibling windows have already been painted.
            /// To achieve transparency without these restrictions, use the SetWindowRgn function.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020,

            /// <summary>Specifies a window that has a border with a raised edge.</summary>
            WS_EX_WINDOWEDGE = 0x00000100
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override bool Equals(object obj)
            {
                return obj is POINT pOINT &&
                       x == pOINT.x &&
                       y == pOINT.y;
            }

            public override int GetHashCode()
            {
                int hashCode = 1502939027;
                hashCode = hashCode * -1521134295 + x.GetHashCode();
                hashCode = hashCode * -1521134295 + y.GetHashCode();
                return hashCode;
            }

            public static bool operator ==(POINT left, POINT right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(POINT left, POINT right)
            {
                return !(left == right);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r)
            {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }

            public System.Drawing.Size Size
            {
                get
                {
                    return new System.Drawing.Size(this.right - this.left, this.bottom - this.top);
                }
            }
        }
        //DotNet48ZDP2\ndp\fx\src\winforms\Managed\System\WinForms\NativeMethods.cs
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class TOOLINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(TOOLINFO));
            public int uFlags;
            public IntPtr hwnd;
            public IntPtr uId;
            public RECT rect;
            public IntPtr hinst = IntPtr.Zero;
            public IntPtr lpszText;
            //public IntPtr lParam = IntPtr.Zero;
        }

        public static int MAKELONG(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }
    }
}
