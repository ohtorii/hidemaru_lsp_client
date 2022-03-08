using System;
using System.Diagnostics;

namespace HidemaruLspClient_FrontEnd
{
    /// <summary>
    /// UIスレッドでデリゲートを実行するクラス
    /// </summary>
    class UIThread
    {
        class HiddenForm : System.Windows.Forms.Form {
            public HiddenForm()
            {
                this.Opacity = 0;
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
            }
            protected override bool ShowWithoutActivation => true;
        }

        static int mainThreadId_;
        static HiddenForm form_;

        public static void Initializer()
        {
            if (form_ == null)
            {
                mainThreadId_ = System.Threading.Thread.CurrentThread.ManagedThreadId;
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

        static bool IsMainThread => System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId_;

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
