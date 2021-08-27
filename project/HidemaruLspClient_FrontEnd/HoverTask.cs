using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using HidemaruLspClient_BackEndContract;
using System.Runtime.InteropServices;
using static HidemaruLspClient_FrontEnd.UnsafeNativeMethods;

namespace HidemaruLspClient_FrontEnd
{
    public partial class Service
    {
        class HoverTask
        {
            const int defaultMillisecondsTimeout = 500;

            Service service_;
            Task task_;
            IWorker worker_;
            ILspClientLogger logger_;
            CancellationToken cancellationToken_;

            NativeMethods.POINT prevMousePoint_ = new NativeMethods.POINT (0,  0 );
            Hidemaru.Position prevHidemaruPosition_ = new Hidemaru.Position(0, 0);
            NativeMethods.TOOLINFO toolItem_ = new NativeMethods.TOOLINFO();
            IntPtr toolTipHandle_=new IntPtr(0);
            /// <summary>
            /// 秀丸エディタのウインドウハンドル
            /// </summary>
            IntPtr hwndHidemaru_;

            public HoverTask(Service service, IWorker worker, ILspClientLogger logger, CancellationToken cancellationToken)
            {                
                service_ = service;
                hwndHidemaru_ = Hidemaru.Hidemaru_GetCurrentWindowHandle();

                worker_ = worker;
                logger_ = logger;
                cancellationToken_ = cancellationToken;
                task_ = Task.Run(MainLoop, cancellationToken_);
            }
            void MainLoop()
            {
                //Todoポーリングではなくイベント通知にする
                try
                {
                    while (true)
                    {
                        if (cancellationToken_.IsCancellationRequested)
                        {
                            return;
                        }
                        Process();
                        Thread.Sleep(defaultMillisecondsTimeout);
                    }
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    //COMサーバ(.exe)が終了したため、ログ出力してからポーリング動作を終了させる。
                    logger_.Error(e.ToString());
                }
            }
            void Process()
            {
                var currentMousePoint = new NativeMethods.POINT (0,0);
                if (UnsafeNativeMethods.GetCursorPos(ref currentMousePoint)==false)
                {
                    return;
                }
                if (currentMousePoint == prevMousePoint_)
                {
                    return;
                }

                var currentHidemaruPosition_ = new Hidemaru.Position(0, 0);
                if (Hidemaru.Hidemaru_GetCursorPosUnicodeFromMousePos(ref currentMousePoint,ref currentHidemaruPosition_.line, ref currentHidemaruPosition_.column) == false)
                {
                    return;
                }
                if (! currentHidemaruPosition_.ValueIsCorrect())
                {
                    return;
                }
                prevMousePoint_ = currentMousePoint;
                prevHidemaruPosition_ = currentHidemaruPosition_;
//                System.Diagnostics.Debug.WriteLine(string.Format("column/Line={0},{1}", prevHidemaruPosition_.column, prevHidemaruPosition_.line));

                var text = service_.Hover(currentHidemaruPosition_.line, currentHidemaruPosition_.column);
                //ShowToolTips(prevMousePoint_.x, prevMousePoint_.y, text);
                ShowToolTipsWin32(prevMousePoint_.x, prevMousePoint_.y, text);
            }

            void ShowToolTipsWin32(int screenX, int screenY, string text)
            {
                if (toolTipHandle_==IntPtr.Zero)
                {
#if true
                    IntPtr hInstance = new IntPtr(0);
                    IntPtr hwnd = new IntPtr(0); 
#else
                    //Memo: 秀丸エディタのウインドウが固まる
                    IntPtr hInstance = Dll.GetModuleHandle(null);
                    IntPtr hwnd = Hidemaru.Hidemaru_GetCurrentWindowHandle();
#endif
                    toolTipHandle_ = CreateTrackingToolTipWin32(hwnd, hInstance, " ");
                    if (toolTipHandle_ == IntPtr.Zero)
                    {
                        return;
                    }
                }

                // Activate the tooltip.
                {
                    const int True = 1;
                    SendMessageWin32(toolTipHandle_, (uint)NativeMethods.ToolTipMessage.TTM_TRACKACTIVATE, True);
                }

                toolItem_.lpszText = Marshal.StringToHGlobalAuto(text); 
                try
                {
                    SendMessageWin32(toolTipHandle_, (uint)NativeMethods.ToolTipMessage.TTM_SETTOOLINFO);

                    // Position the tooltip. The coordinates are adjusted so that the tooltip does not overlap the mouse pointer.
                    {
                        var pt = new NativeMethods.POINT(screenX, screenY);
                        SendMessage(toolTipHandle_, (uint)NativeMethods.ToolTipMessage.TTM_TRACKPOSITION, 0, NativeMethods.MAKELONG(pt.x + 10, pt.y - 20));
                    }
                }
                finally
                {
                    if (toolItem_.lpszText != IntPtr.Zero)
                    {
                        var ptr = toolItem_.lpszText;
                        toolItem_.lpszText = IntPtr.Zero;
                        Marshal.FreeHGlobal(ptr);
                    }
                }
            }

            IntPtr CreateTrackingToolTipWin32(IntPtr hwndParent, IntPtr hInstance, string  text)
            {
                var NULL = IntPtr.Zero;

                // Create a tooltip.
                var hwndTT = UnsafeNativeMethods.CreateWindowEx(NativeMethods.WindowStylesEx.WS_EX_TOPMOST, NativeMethods.TOOLTIPS_CLASS, null,
                    NativeMethods.WindowStyles.WS_POPUP | NativeMethods.WindowStyles.TTS_NOPREFIX | NativeMethods.WindowStyles.TTS_ALWAYSTIP,
                    NativeMethods.CW_USEDEFAULT, NativeMethods.CW_USEDEFAULT, NativeMethods.CW_USEDEFAULT, NativeMethods.CW_USEDEFAULT,
                    hwndParent, NULL, hInstance, NULL);
                if (hwndTT== IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                // Set up the tool information. In this case, the "tool" is the entire parent window.

                //toolItem_.cbSize   = NativeMethods.TTTOOLINFOW_V2_SIZE;// Marshal.SizeOf(typeof(Dll.TOOLINFO));
                toolItem_.uFlags   = (int)(NativeMethods.ToolTipFlags.TTF_IDISHWND | NativeMethods.ToolTipFlags.TTF_TRACK | NativeMethods.ToolTipFlags.TTF_ABSOLUTE);
                toolItem_.hwnd     = hwndParent;
                toolItem_.hinst    = hInstance;
                toolItem_.uId      = hwndParent;
                //toolItem_.lParam   = NULL;
                UnsafeNativeMethods.GetClientRect(hwndParent, out toolItem_.rect);
                
                try
                {
                    toolItem_.lpszText = Marshal.StringToHGlobalAuto(text);
                    this.SendMessageWin32(hwndTT, (uint)NativeMethods.ToolTipMessage.TTM_ADDTOOL);
                }
                finally
                {
                    if (toolItem_.lpszText != IntPtr.Zero)
                    {
                        var ptr = toolItem_.lpszText;
                        toolItem_.lpszText = IntPtr.Zero;
                        Marshal.FreeHGlobal(ptr);
                    }
                }
                return hwndTT;
            }
            bool SendMessageWin32(IntPtr hWnd, uint Msg, int wparam)
            {
                var ret = (int)SendMessage(hWnd, Msg, wparam, toolItem_);
                if (ret == 0)
                {
                    return false;
                }
                return true;
            }
            bool SendMessageWin32(IntPtr hWnd, uint Msg)
            {
                return this.SendMessageWin32(hWnd,Msg,0);
            }
        }

        
        

    }
}
