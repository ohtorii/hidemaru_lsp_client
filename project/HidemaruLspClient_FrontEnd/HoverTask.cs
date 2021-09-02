using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using HidemaruLspClient_BackEndContract;
using System.Runtime.InteropServices;
using static HidemaruLspClient_FrontEnd.UnsafeNativeMethods;
using static HidemaruLspClient_FrontEnd.NativeMethods;

namespace HidemaruLspClient_FrontEnd
{
    public partial class Service
    {
        /// <summary>
        /// textDocument/hover
        /// </summary>
        class HoverTask
        {
            const int defaultMillisecondsTimeout = 500;

            Service service_;
            Task task_;
            IWorker worker_;
            ILspClientLogger logger_;
            CancellationToken cancellationToken_;

            POINT prevMousePoint_ = new POINT (0,  0 );
            Hidemaru.Position prevHidemaruPosition_ = new Hidemaru.Position(0, 0);
            TOOLINFO toolItem_ = new TOOLINFO();
            IntPtr toolTipHandle_=new IntPtr(0);
            /// <summary>
            /// 秀丸エディタのウインドウハンドル
            /// </summary>
            IntPtr hwndHidemaru_;
            bool toolTipShow_ = false;

            public Task GetTask() { return task_; }
            public HoverTask(Service service, IWorker worker, ILspClientLogger logger, CancellationToken cancellationToken)
            {
                service_ = service;
                hwndHidemaru_ = Hidemaru.Hidemaru_GetCurrentWindowHandle();

                worker_ = worker;
                logger_ = logger;
                cancellationToken_ = cancellationToken;
                task_ = Task.Run(MainLoop, cancellationToken_);
            }
            async Task MainLoop()
            {
                //Todo:ポーリングではなくイベント通知にする
                NativeMethods.MSG  msg=new NativeMethods.MSG();

                try
                {
                    while (0<UnsafeNativeMethods.GetMessage(out msg, IntPtr.Zero, 0, 0))
                    //while(true)
                    {
                        if (cancellationToken_.IsCancellationRequested)
                        {
                            return;
                        }
                        Process();
#if true
                        Thread.Sleep(defaultMillisecondsTimeout);
#else
                        //SendMessageから戻ってこなくなる
                        await Task.Delay(defaultMillisecondsTimeout,cancellationToken_);
#endif
                    }
                }
                catch (Exception e)
                {
                    //System.Threading.Tasks.TaskCanceledException
                    //System.Runtime.InteropServices.COMException
                    logger_.Error(e.ToString());
                }
            }
            void Process()
            {
                System.Diagnostics.Debug.WriteLine("==== Process ====");

                string tooltipText=null;
                {
                    bool mouseMoved;
                    bool cursorMoved;
                    var currentMousePoint = new POINT(0, 0);
                    var currentHidemaruPosition_ = new Hidemaru.Position(0, 0);


                    if (GetCursorPos(ref currentMousePoint) == false)
                    {
                        HideToolTipsWin32();
                        return;
                    }
                    mouseMoved = prevMousePoint_ != currentMousePoint;
                    prevMousePoint_ = currentMousePoint;

                    if (Hidemaru.Hidemaru_GetCursorPosUnicode(ref currentHidemaruPosition_.line, ref currentHidemaruPosition_.column) == false)
                    {
                        HideToolTipsWin32();
                        return;
                    }
                    if (!currentHidemaruPosition_.ValueIsCorrect())
                    {
                        HideToolTipsWin32();
                        return;
                    }
                    cursorMoved = prevHidemaruPosition_ != currentHidemaruPosition_;
                    prevHidemaruPosition_ = currentHidemaruPosition_;

                    System.Diagnostics.Debug.WriteLine(string.Format("mouseMoved={0} / cursorMoved={1}", mouseMoved, cursorMoved));
                    if (mouseMoved)
                    {
                        if (cursorMoved)
                        {
                            //pass
                            return;
                        }
                        else
                        {
                            int line=0, column=0;
                            if(! Hidemaru.Hidemaru_GetCursorPosUnicodeFromMousePos(ref currentMousePoint,ref line, ref column))
                            {
                                HideToolTipsWin32();
                                return;
                            }
                            tooltipText = service_.Hover(line, column);
                        }
                    }
                    else
                    {
                        if (cursorMoved)
                        {
                            HideToolTipsWin32();
                        }
                        else
                        {
                            //pass
                            return;
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine(String.Format("tooltipText={0}", tooltipText));
                if ((tooltipText == null) || (tooltipText.Length == 0))
                {
                    HideToolTipsWin32();
                }
                else
                {
                    /* 秀丸エディタはWin32-APIを利用したアプリなので、本マクロでも素直にWin32-APIを利用しています。
                         *なお、WinformsのToolTipも動作検証しましたが、意図したとおりに動かず利用していません。
                         * ShowToolTipsWinforms(prevMousePoint_.x, prevMousePoint_.y, text);
                         */
                    ShowToolTipsWin32(prevMousePoint_.x, prevMousePoint_.y, tooltipText);
                }
            }

            void HideToolTipsWin32()
            {
                System.Diagnostics.Debug.WriteLine("HideToolTipsWin32");
                if (toolTipHandle_ == IntPtr.Zero)
                {
                    return;
                }
                if (toolTipShow_)
                {
#if true
                    // Deactivate the tooltip.
                    const int False = 0;
                    SendMessageWin32(toolTipHandle_, (uint)ToolTipMessage.TTM_TRACKACTIVATE, False);
#else
                    ShowWindow(toolTipHandle_, (int)ShowState.SW_HIDE);
#endif
                    toolTipShow_ = false;
                }
            }
            /// <summary>
            /// Tooltipsを表示する(Win-API版)
            /// </summary>
            /// <param name="screenX"></param>
            /// <param name="screenY"></param>
            /// <param name="text"></param>
            void ShowToolTipsWin32(int screenX, int screenY, string text)
            {
                System.Diagnostics.Debug.WriteLine("ShowToolTipsWin32");

                if (toolTipHandle_==IntPtr.Zero)
                {
#if true
                    IntPtr hInstance = new IntPtr(0);
                    IntPtr hwnd = new IntPtr(0);
#else
                    //Memo: 秀丸エディタのウインドウが固まる
                    IntPtr hInstance = UnsafeNativeMethods.GetModuleHandle(null);
                    IntPtr hwnd = Hidemaru.Hidemaru_GetCurrentWindowHandle();
#endif
                    toolTipHandle_ = CreateTrackingToolTipWin32(hwnd, hInstance);
                    if (toolTipHandle_ == IntPtr.Zero)
                    {
                        return;
                    }
                }

                // Activate the tooltip.
                if(! toolTipShow_)
                {
                    const int True = 1;
                    SendMessageWin32(toolTipHandle_, (uint)ToolTipMessage.TTM_TRACKACTIVATE, True);
                    toolTipShow_ = true;
                }

                toolItem_.lpszText = Marshal.StringToHGlobalAuto(text);
                try
                {
                    SendMessageWin32(toolTipHandle_, (uint)ToolTipMessage.TTM_SETTOOLINFO);

                    // Position the tooltip. The coordinates are adjusted so that the tooltip does not overlap the mouse pointer.
                    {
                        var pt = new POINT(screenX, screenY);
                        SendMessage(toolTipHandle_, (uint)ToolTipMessage.TTM_TRACKPOSITION, 0, MAKELONG(pt.x + 10, pt.y - 20));
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

            IntPtr CreateTrackingToolTipWin32(IntPtr hwndParent, IntPtr hInstance)
            {
                System.Diagnostics.Debug.WriteLine("==== CreateTrackingToolTipWin32 ====");
                var NULL = IntPtr.Zero;

                // Create a tooltip.
                var hwndTT = CreateWindowEx(WindowStylesEx.WS_EX_TOPMOST,
                                            TOOLTIPS_CLASS,
                                            null,
                                            WindowStyles.WS_POPUP | WindowStyles.TTS_NOPREFIX | WindowStyles.TTS_ALWAYSTIP,
                                            CW_USEDEFAULT,
                                            CW_USEDEFAULT,
                                            CW_USEDEFAULT,
                                            CW_USEDEFAULT,
                                            hwndParent,
                                            NULL,
                                            hInstance,
                                            NULL);
                if (hwndTT== IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                // Set up the tool information. In this case, the "tool" is the entire parent window.

                //toolItem_.cbSize   = TTTOOLINFOW_V2_SIZE;// Marshal.SizeOf(typeof(Dll.TOOLINFO));
                toolItem_.uFlags   = (int)(ToolTipFlags.TTF_IDISHWND | ToolTipFlags.TTF_TRACK | ToolTipFlags.TTF_ABSOLUTE);
                toolItem_.hwnd     = hwndParent;
                toolItem_.hinst    = hInstance;
                toolItem_.uId      = hwndParent;
                //toolItem_.lParam   = NULL;
                GetClientRect(hwndParent, out toolItem_.rect);

                try
                {
                    toolItem_.lpszText = Marshal.StringToHGlobalAuto(" ");
                    this.SendMessageWin32(hwndTT, (uint)ToolTipMessage.TTM_ADDTOOL);
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
                SendMessage(hwndTT, (uint)ToolTipMessage.TTM_SETMAXTIPWIDTH, 0, 300);
                return hwndTT;
            }
            bool SendMessageWin32(IntPtr hWnd, uint Msg, int wparam)
            {
                System.Diagnostics.Debug.WriteLine("  Enter SendMessageWin32");
                var ret = (int)SendMessage(hWnd, Msg, wparam, toolItem_);
                System.Diagnostics.Debug.WriteLine(string.Format("  Leave SendMessageWin32. ret={0}",ret));
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
