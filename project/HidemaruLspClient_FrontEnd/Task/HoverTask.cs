using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using HidemaruLspClient_BackEndContract;
using static HidemaruLspClient_FrontEnd.UnsafeNativeMethods;
using static HidemaruLspClient_FrontEnd.NativeMethods;

namespace HidemaruLspClient_FrontEnd
{
    /// <summary>
    /// textDocument/hover
    /// </summary>
    class HoverTask
    {
        Tooltipform tooltipWinforms_;

        public HoverTask(Service service, IWorker worker, ILspClientLogger logger, CancellationToken cancellationToken)
        {
            tooltipWinforms_ = new Tooltipform(service, logger, cancellationToken);
        }

        #region Tooltipform
        //以下コードを改変利用しています。
        //https://github.com/komiyamma/hidemaru-net/tree/master/win-hidemaru-app/hm_ts_intellisense/HmTSHintPopup.src/HmTSHintPopup
        //
        //(License)
        //https://github.com/komiyamma/hidemaru-net/blob/master/win-hidemaru-app/hm_ts_intellisense/LICENSE.txt
        //

        class Tooltipform : Form
        {
            Service service_;
            ILspClientLogger logger_;
            CancellationToken cancellationToken_;

            POINT prevMousePoint_ = new POINT(0, 0);
            Hidemaru.Position prevHidemaruPosition_ = new Hidemaru.Position(0, 0);

            /// <summary>
            /// 秀丸エディタのウインドウハンドル
            /// </summary>
            IntPtr hwndHidemaru_;


            #region フォームの要素
            Label label_ = new Label();
            System.Windows.Forms.Timer timer_;
            #endregion


            public Tooltipform(Service service, ILspClientLogger logger, CancellationToken cancellationToken)
            {
                service_ = service;
                hwndHidemaru_ = Hidemaru.Hidemaru_GetCurrentWindowHandle();

                logger_ = logger;
                cancellationToken_ = cancellationToken;

                SetFormAttr();
                SetLabelAttr();
                SetTimerAttr();
            }

            #region Attr
            void SetFormAttr()
            {
                this.Width = 816;
                this.Height = 208;
                this.BackColor = Color.White;
                this.ShowInTaskbar = false;

                //タイトルバーを消す
                this.ControlBox = false;
                this.Text = "";
                // ボーダーを消す
                this.FormBorderStyle = FormBorderStyle.None;

            }
            protected override bool ShowWithoutActivation => true;

            protected override void WndProc(ref Message m)
            {
                const int WM_MOUSEACTIVATE = 0x0021;
                const int MA_NOACTIVATE = 3;

                if (m.Msg == WM_MOUSEACTIVATE)
                {
                    m.Result = new IntPtr(MA_NOACTIVATE);
                    return;
                }

                base.WndProc(ref m);
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    // 常に最前面に表示させる(topMostプロパティを使うと
                    // ShowWithoutActivationが効かないため
                    const int WS_EX_TOPMOST = 0x00000008;

                    CreateParams p = base.CreateParams;
                    p.ExStyle |= WS_EX_TOPMOST;
                    return p;
                }
            }

            #endregion

            void SetLabelAttr()
            {
                const int padding = 8;

                label_.Left = padding;
                label_.Top = padding;
                label_.Width = 800;
                label_.Height = 140;
                this.Controls.Add(label_);
            }

            #region Timer
            void SetTimerAttr()
            {
                if (timer_ == null)
                {
                    timer_ = new System.Windows.Forms.Timer();
                }
                timer_.Tick += MainLoop;
                timer_.Enabled = true;
                timer_.Interval = 100;
                timer_.Start();
            }

            async void MainLoop(object sender, EventArgs e)
            {
                if (cancellationToken_.IsCancellationRequested)
                {
                    timer_.Stop();
                    this.Close();
                    return;
                }
                string tooltipText = null;
                {
                    bool mouseMoved;
                    bool cursorMoved;
                    var currentMousePoint = new POINT(0, 0);
                    var currentHidemaruPosition_ = new Hidemaru.Position(0, 0);


                    if (GetCursorPos(ref currentMousePoint) == false)
                    {
                        HideToolTips();
                        return;
                    }
                    mouseMoved = prevMousePoint_ != currentMousePoint;
                    prevMousePoint_ = currentMousePoint;

                    if (Hidemaru.Hidemaru_GetCursorPosUnicode(ref currentHidemaruPosition_.line, ref currentHidemaruPosition_.column) == false)
                    {
                        HideToolTips();
                        return;
                    }
                    if (!currentHidemaruPosition_.ValueIsCorrect())
                    {
                        HideToolTips();
                        return;
                    }
                    cursorMoved = prevHidemaruPosition_ != currentHidemaruPosition_;
                    prevHidemaruPosition_ = currentHidemaruPosition_;

                    if (mouseMoved)
                    {
                        if (cursorMoved)
                        {
                            //pass
                            return;
                        }
                        else
                        {
                            int line = 0, column = 0;
                            if (!Hidemaru.Hidemaru_GetCursorPosUnicodeFromMousePos(ref currentMousePoint, ref line, ref column))
                            {
                                HideToolTips();
                                return;
                            }
                            try
                            {
                                timer_.Stop();
                                tooltipText = await Task.Run(() => service_.Hover(line, column), cancellationToken_);
                            }
                            finally
                            {
                                timer_.Start();
                            }
                        }
                    }
                    else
                    {
                        if (cursorMoved)
                        {
                            HideToolTips();
                        }
                        else
                        {
                            //pass
                            return;
                        }
                    }
                }

                if ((tooltipText == null) || (tooltipText.Length == 0))
                {
                    HideToolTips();
                }
                else
                {
                    ShowToolTips(prevMousePoint_.x, prevMousePoint_.y, tooltipText);
                }
            }
            #endregion

            void HideToolTips()
            {
                this.Hide();
            }
            void ShowToolTips(int screenX, int screenY, string text)
            {
                this.Left = screenX;
                this.Top = screenY;
                this.label_.Text = text;
                this.Show();
            }
        }
        #endregion /*Tooltipform*/
    }

}

