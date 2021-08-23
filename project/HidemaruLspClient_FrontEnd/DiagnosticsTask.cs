using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HidemaruLspClient_BackEndContract;

namespace HidemaruLspClient_FrontEnd
{
    public partial class Service
    {
        /// <summary>
        /// textDocument/publishDiagnostics
        /// </summary>        
        class DiagnosticsTask
        {
            const int defaultMillisecondsTimeout = 500;
            Task task_;
            IWorker worker_;
            ILspClientLogger logger_;
            CancellationToken cancellationToken_;

            /// <summary>
            /// 秀丸エディタのウインドウハンドル
            /// </summary>
            IntPtr hwndHidemaru_;

            /// <summary>
            /// アウトプット枠をクリアしたかどうか
            /// 繰り返しクリアしてウインドウがちらつくのを防止する。
            /// </summary>
            bool outputPaneCleard_;

            public DiagnosticsTask(IWorker worker, ILspClientLogger logger, CancellationToken cancellationToken)
            {
                hwndHidemaru_     = Hidemaru.Hidemaru_GetCurrentWindowHandle();
                outputPaneCleard_ = false;

                worker_            = worker;
                logger_ = logger;
                cancellationToken_ = cancellationToken;                
                task_              =Task.Run(MainLoop, cancellationToken_);
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
                }catch(System.Runtime.InteropServices.COMException e)
                {
                    //COMサーバ(.exe)が終了したため、ログ出力してからポーリング動作を終了させる。
                    logger_.Error(e.ToString());
                }
            }
            void Process()
            {                
                var container = worker_.PullDiagnosticsParams();
                bool hasEvent = container.Length == 0 ? false : true;

                if (hasEvent == false)
                {
                    return;
                }

                var sb = new StringBuilder();
                for (long i = 0; i < container.Length; ++i)
                {
                    FormatDiagnostics(sb, container.Item(i));
                }

                if (sb.Length == 0)
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
                    HmOutputPane.OutputW(hwndHidemaru_, sb.ToString());

                    outputPaneCleard_ = false;
                }
            }

            static void FormatDiagnostics(StringBuilder sb, IPublishDiagnosticsParams diagnosticsParams)
            {
                var uri      = new Uri(diagnosticsParams.uri);
                var filename = uri.LocalPath;
                for (long i = 0; i < diagnosticsParams.Length; ++i)
                {
                    var diagnostic = diagnosticsParams.Item(i);
                    var severity   = diagnostic.severity;
                    if (severity <= /*DiagnosticSeverity.Error*/ DiagnosticSeverity.Warning)
                    {
                        //+1して秀丸エディタの行番号(1開始)にする
                        var line    = diagnostic.range.start.line + 1;
                        var code    = diagnostic.code;
                        var message = diagnostic.message;
                        var source  = diagnostic.source;

                        //Memo: 秀丸エディタのアウトプット枠へ出力するには \r\n が必要。
                        sb.Append($"{filename}({line}):  {source}({code}) {message}\r\n");
                    }
                }
            }
        }

        
                

    }
}
