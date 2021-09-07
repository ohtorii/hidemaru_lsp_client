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
        static HiddenForm form_;
        public static void Initializer()
        {
            if (form_ == null)
            {
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
        /// <summary>
        /// デリゲートをUIスレッドで実行する
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static object Invoke(Delegate method)
        {
            Debug.Assert(form_!=null);
            return form_.Invoke(method);
        }

    }
}
