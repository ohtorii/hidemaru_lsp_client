using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace HidemaruLspClient_FrontEnd
{
    /// <summary>
    /// UIスレッドでデリゲートを実行するクラス
    /// </summary>
    class UIThread
    {
        class HiddenForm : Form {
            public HiddenForm()
            {
                this.Text = "HidemaruLspClient";
                this.Load += HiddenForm_Load;
            }

            private void HiddenForm_Load(object sender, EventArgs e)
            {
                this.ShowIcon = false;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Visible = false;
                this.FormBorderStyle = FormBorderStyle.None;
            }

            protected override bool ShowWithoutActivation => true;
        }

        static int mainThreadId_;
        static HiddenForm form_;

        public static void Initializer()
        {
            if (form_ == null)
            {
                mainThreadId_ = Thread.CurrentThread.ManagedThreadId;
                form_ = new HiddenForm();
                //Memo: Formを表示するとInvokeを呼べる
#if false
                Application.Run(hideenForm_);
#else
                form_.Show();
#endif
            }
        }
        public static void Finalizer()
        {
            if (form_ != null)
            {
                form_.Close();
                form_.Dispose();
                form_ = null;
            }
        }

        static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == mainThreadId_;

        /// <summary>
        /// デリゲートをUIスレッドで実行する
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static object Invoke(Delegate method)
        {
            Debug.Assert(form_!=null);
            if (IsMainThread)
            {
                return method.DynamicInvoke();
            }
            else
            {
                return form_.Invoke(method);
            }
        }

    }
}
