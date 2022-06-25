using HidemaruLspClient_BackEndContract;
using System;
using System.Diagnostics;
using System.Threading;

namespace HidemaruLspClient_FrontEnd
{
    class SyncDocumenmtTask
    {
        /// <summary>
        /// OpenEvent
        /// </summary>
        public class OpenEventArgs : EventArgs
        {
            public OpenEventArgs(string fileName, string text, int version)
            {
                this.FileName = fileName;
                this.Text = text;
                this.ContentsVersion = version;
            }
            public string FileName { get; }
            public string Text{ get; }
            public int ContentsVersion { get; }
        }
        public delegate void OpenEventHandler(object sender, OpenEventArgs e);
        public event OpenEventHandler OpenEvent;
        void RaiseOpenEvent(OpenEventArgs e)
        {
            if (OpenEvent != null)
            {
                OpenEvent(this, e);
            }
        }
        /// <summary>
        /// Change
        /// </summary>
        public class ChangeEventArgs : EventArgs
        {
            public ChangeEventArgs(string fileName, string text, int version)
            {
                this.FileName = fileName;
                this.Text = text;
                this.ContentsVersion = version;
            }
            public string FileName { get; }
            public string Text { get; }
            public int ContentsVersion { get; }
        }
        public delegate void ChangeEventHandler(object sender, ChangeEventArgs e);
        public event ChangeEventHandler ChangeEvent;
        void RaiseChangeEvent(ChangeEventArgs e)
        {
            if (ChangeEvent != null)
            {
                ChangeEvent(this, e);
            }
        }

        /// <summary>
        /// CloseEvent
        /// </summary>
        public class CloseEventArgs:EventArgs
        {
            public CloseEventArgs(string filename)
            {
                this.FileName = filename;
            }
            public string FileName { get; }
        }
        public delegate void CloseEventHandler(object sender, CloseEventArgs e);
        public event CloseEventHandler CloseEvent;
        void RaiseCloseEvent(CloseEventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent(this, e);
            }
        }


        ILspClientLogger logger_;
        CancellationToken cancellationToken_;
        System.Windows.Forms.Timer timer_;
        HidemaruEditorDocument openedFile_;

        public void Finish()
        {
            if (string.IsNullOrEmpty(openedFile_.Filename) == false)
            {
                DidClose();
            }
        }

        public SyncDocumenmtTask(ILspClientLogger logger, CancellationToken cancellationToken)
        {
            logger_ = logger;
            cancellationToken_ = cancellationToken;
            openedFile_  = new HidemaruEditorDocument();

            timer_ = new System.Windows.Forms.Timer();
            timer_.Interval = 1000;
            timer_.Tick += Update;
            timer_.Start();
        }

        /// <summary>
        /// 秀丸エディタのテキストとサーバ側のテキストを明示的に同期する（デバッグ用途）
        /// </summary>
        /// <returns></returns>
        public bool SyncDocument()
        {
            try
            {
                var _ = FileProc();
            }
            catch (Exception e)
            {
                HmOutputPane.OutputW(Hidemaru.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_.Error(e.ToString());
                return false;
            }
            return true;
        }
        public string QueryFileName()
        {
            return FileProc();
        }
        void Update(object sender, EventArgs e)
        {
            try
            {
                if (cancellationToken_.IsCancellationRequested)
                {
                    timer_.Stop();
                    return;
                }
                try
                {
                    var _ = FileProc();
                }
                catch (Exception exception)
                {
                    HmOutputPane.OutputW(Hidemaru.Hidemaru_GetCurrentWindowHandle(), exception.ToString());
                    logger_.Error(exception.ToString());
                    return ;
                }
                return ;
            }
            catch (Exception exce)
            {
                logger_.Error(exce.ToString());
                timer_.Stop();
                throw;
            }
        }


        enum DigOpenStatus
        {
            /// <summary>
            /// ファイルを開いた
            /// </summary>
            Opened,
            /// <summary>
            /// ファイールはオープン済み
            /// </summary>
            AlreadyOpened,
            /// <summary>
            /// 失敗
            /// </summary>
            Failed,
        }
        enum DigChangeStatus
        {
            /// <summary>
            /// 変更あり
            /// </summary>
            Changed,
            /// <summary>
            /// 変更無し
            /// </summary>
            NoChanged,
            /// <summary>
            /// 失敗
            /// </summary>
            Failed,
        }

        DigOpenStatus DigOpen(string absFilename)
        {
            try
            {
                if (openedFile_.Filename == absFilename)
                {
                    return DigOpenStatus.AlreadyOpened;
                }
                var text = Hidemaru.GetTotalTextUnicode();
                const int contentsVersion = 1;
                RaiseOpenEvent(new OpenEventArgs(absFilename, text, contentsVersion));
                openedFile_.Setup(absFilename,
                                 new Uri(absFilename),
                                 Hidemaru.GetUpdateCount(),
                                 contentsVersion);
                return DigOpenStatus.Opened;
            }
            catch (Exception e)
            {
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            return DigOpenStatus.Failed;
        }
        DigChangeStatus TryDigChange()
        {
            try
            {
                Debug.Assert(string.IsNullOrEmpty(openedFile_.Filename) == false);

                var currentUpdateCount = Hidemaru.GetUpdateCount();
                var prevUpdateCount    = openedFile_.hidemaruUpdateCount;
                if (currentUpdateCount == prevUpdateCount)
                {
                    return DigChangeStatus.NoChanged;
                }
                openedFile_.UpdateContentsVersion(currentUpdateCount);
                RaiseChangeEvent(new ChangeEventArgs(openedFile_.Filename, Hidemaru.GetTotalTextUnicode(), openedFile_.countentsVersion));
                return DigChangeStatus.Changed;
            }catch(Exception e)
            {
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            return DigChangeStatus.Failed;
        }
        void DidClose()
        {
            Debug.Assert(string.IsNullOrEmpty(openedFile_.Filename) == false);
            RaiseCloseEvent(new CloseEventArgs(openedFile_.Filename));
            openedFile_.Clear();
        }

        /// <summary>
        /// ファイルの処理
        /// </summary>
        /// <returns>現在、秀丸エディタで開いているファイルの絶対パス</returns>
        string FileProc()
        {
            const string fileNotFound = "";
            Func<string, string> fncDidOpen = (absFileName) =>
            {
                switch (DigOpen(absFileName))
                {
                    case DigOpenStatus.Opened:
                        return absFileName;

                    case DigOpenStatus.AlreadyOpened:
                        return absFileName;

                    case DigOpenStatus.Failed:
                        logger_.Warn("DigOpenStatus.Failed");
                        return fileNotFound;

                    default:
                        logger_.Warn("DigOpenStatus.???");
                        break;
                }
                return fileNotFound;
            };

            string currentHidemaruFilePath;
            if (String.IsNullOrEmpty(openedFile_.Filename))
            {
                //初めてファイルを開く場合
                currentHidemaruFilePath = Hidemaru.GetFileFullPath();
                if (String.IsNullOrEmpty(currentHidemaruFilePath))
                {
                    return fileNotFound;
                }
                return fncDidOpen(currentHidemaruFilePath);
            }

            //
            //2回目以降にファイルを開く場合
            //
            currentHidemaruFilePath = Hidemaru.GetFileFullPath();
            if (string.IsNullOrEmpty(currentHidemaruFilePath))
            {
                //秀丸エディタのファイルが閉じた場合
                DidClose();
                return fileNotFound;
            }

            if (openedFile_.Filename == currentHidemaruFilePath)
            {
                //秀丸エディタで前回と同じファイルを開いている場合
                if (TryDigChange() == DigChangeStatus.Failed)
                {
                    logger_.Warn("DigChangeStatus.Failed");
                    return fileNotFound;
                }
                return currentHidemaruFilePath;
            }

            //秀丸エディタで前回と異なるファイルを開いた場合
            DidClose();
            return fncDidOpen(currentHidemaruFilePath);
        }
    }
}
