using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HidemaruLspClient_BackEndContract;

namespace HidemaruLspClient_FrontEnd.BackgroundTask
{

    /// <summary>
    /// textDocument/publishDiagnostics
    /// </summary>
    class Diagnostics
    {
        public delegate IPublishDiagnosticsParamsContainer PullDiagnosticsParamsType();
        PullDiagnosticsParamsType PullDiagnosticsParams;

        ILspClientLogger logger_;
        CancellationToken cancellationToken_;
        System.Windows.Forms.Timer timer_;

        /// <summary>
        /// 秀丸エディタのウインドウハンドル
        /// </summary>
        IntPtr hwndHidemaru_;

        /// <summary>
        /// アウトプット枠をクリアしたかどうか
        /// 繰り返しクリアしてウインドウがちらつくのを防止する。
        /// </summary>
        bool outputPaneCleard_;

        public Diagnostics(PullDiagnosticsParamsType func, ILspClientLogger logger, CancellationToken cancellationToken)
        {
            PullDiagnosticsParams = func;
            hwndHidemaru_ = Hidemaru.Hidemaru_GetCurrentWindowHandle();
            outputPaneCleard_ = false;

            logger_ = logger;
            cancellationToken_ = cancellationToken;

            timer_ = new System.Windows.Forms.Timer();
            timer_.Interval = 500;
            timer_.Tick += UpdateAsync;
            timer_.Start();
        }

        async void UpdateAsync(object sender, EventArgs e)
        {
            try
            {
                if (cancellationToken_.IsCancellationRequested)
                {
                    timer_.Stop();
                    return;
                }

                (bool pullEventOccurred, string text) result;
                try
                {
                    /*
                         * awaitでタスクの終了を待っている間に、タイマーイベントで繰り返し呼ばれタスクを生成し続けるため、タスクが終了するまでタイマーを止めている。
                         */
                    timer_.Stop();
                    result = await Task.Run(() => PullDiagnosticsText(), cancellationToken_);
                }
                finally
                {
                    timer_.Start();
                }

                if (result.pullEventOccurred == false)
                {
                    return;
                }
                if (result.text == null)
                {
                    if (outputPaneCleard_ == false)
                    {
                        HmOutputPane.Clear(hwndHidemaru_);
                        outputPaneCleard_ = true;
                    }
                }
                else
                {
                    HmOutputPane.Clear(hwndHidemaru_);
                    HmOutputPane.OutputW(hwndHidemaru_, result.text);
                    outputPaneCleard_ = false;
                }
            }
            catch (Exception exce)
            {
                logger_.Error(exce.ToString());
                timer_.Stop();
                throw;
            }
        }

        (bool pullEventOccurred, string text) PullDiagnosticsText()
        {
            var container = PullDiagnosticsParams();
            bool pullEventOccurred = container.Length == 0 ? false : true;
            if (pullEventOccurred == false)
            {
                return (pullEventOccurred, null);
            }

            var sb = new StringBuilder();
            for (long i = 0; i < container.Length; ++i)
            {
                FormatDiagnostics(sb, container.Item(i));
            }
            var text = sb.ToString();
            if (text.Length == 0)
            {
                return (pullEventOccurred, null);
            }
            return (pullEventOccurred, text);
        }

        static void FormatDiagnostics(StringBuilder sb, IPublishDiagnosticsParams diagnosticsParams)
        {
            var uri = new Uri(diagnosticsParams.uri);
            var filename = uri.LocalPath;
            for (long i = 0; i < diagnosticsParams.Length; ++i)
            {
                var diagnostic = diagnosticsParams.Item(i);
                var severity = diagnostic.severity;
                if (severity <= /*DiagnosticSeverity.Error*/ DiagnosticSeverity.Warning)
                {
                    //+1して秀丸エディタの行番号(1開始)にする
                    var line = diagnostic.range.start.line + 1;
                    var code = diagnostic.code;
                    var message = diagnostic.message;
                    var source = diagnostic.source;

                    //Memo: 秀丸エディタのアウトプット枠へ出力するには \r\n が必要。
                    sb.Append($"{filename}({line}):  {source}({code}) {message}\r\n");
                }
            }
        }
    }
}
