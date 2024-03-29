﻿using HidemaruLspClient_BackEndContract;
using HidemaruLspClient_FrontEnd.Facility;
using HidemaruLspClient_FrontEnd.Hidemaru;
using System;
using System.Diagnostics;
using System.Threading;

namespace HidemaruLspClient_FrontEnd.BackgroundTask
{
    //Todo: 状態遷移のクラスを利用し処理を単純化する

    /// <summary>
    /// 秀丸エディタの状態(Oepn,Change,Save,Close)とLSPの状態を同期する。
    /// </summary>
    class SyncDocumenmt
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
            public string Text { get; }
            public int ContentsVersion { get; }
        }
        public delegate void OpenEventHandler(object sender, OpenEventArgs e);
        public event OpenEventHandler OpenEvent;
        void RaiseOpenEvent(OpenEventArgs e)
        {
            if (OpenEvent != null)
            {
                logger_?.Trace($"RaiseOpenEvent:e.FileName={e.FileName}");
                OpenEvent(this, e);
                didSaveTransition.digOpenWasCalled = true;
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
                logger_?.Trace($"RaiseChangeEvent:e.FileName={e.FileName}");
                ChangeEvent(this, e);
            }
        }

        /// <summary>
        /// CloseEvent
        /// </summary>
        public class CloseEventArgs : EventArgs
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
                logger_?.Trace($"RaiseCloseEvent:e.FileName={e.FileName}");
                CloseEvent(this, e);
            }
        }


        /// <summary>
        /// Save
        /// </summary>
        public class SaveEventArgs : EventArgs
        {
            public SaveEventArgs(string fileName, string text)
            {
                this.FileName = fileName;
                this.Text = text;
            }
            public string FileName { get; }
            public string Text { get; }
        }
        public delegate void SaveEventHandler(object sender, SaveEventArgs e);
        public event SaveEventHandler SaveEvent;
        void RaiseSaveEvent(SaveEventArgs e)
        {
            if (ChangeEvent != null)
            {
                logger_?.Trace($"RaiseSaveEvent:e.FileName={e.FileName}");
                SaveEvent(this, e);
            }
        }

        /// <summary>
        ///didOpen後にdidSaveイベントを発生可能にする
        /// </summary>
        class DidSaveTransition
        {
            public bool digOpenWasCalled = false;
            public bool registEvent = false;
            /// <summary>
            /// SaveEvventに登録する必要があるかどうか
            /// </summary>
            public bool ShouldRegisterSaveEvent { get { return digOpenWasCalled && (!registEvent); } }
        }

        DidSaveTransition didSaveTransition;
        ILspClientLogger logger_;
        CancellationToken cancellationToken_;
        System.Windows.Forms.Timer timer_;
        HidemaruEditorDocument openedFile_;


        public SyncDocumenmt(ILspClientLogger logger, CancellationToken cancellationToken)
        {
            logger_ = logger;
            cancellationToken_ = cancellationToken;
            openedFile_ = new HidemaruEditorDocument();
            didSaveTransition = new DidSaveTransition();

            timer_ = new System.Windows.Forms.Timer();
            timer_.Interval = 1000;
            timer_.Tick += Update;
            timer_.Start();
        }
        public void Finish()
        {
            if (openedFile_ == null)
            {
                return;
            }

            if (openedFile_.IsValidFileName())
            {
                DidClose();
            }
            openedFile_.Clear();
            openedFile_ = null;
        }

        /// <summary>
        /// 秀丸エディタのテキストとサーバ側のテキストを明示的に同期する（デバッグ用途）
        /// </summary>
        /// <returns></returns>
        public bool SyncDocument_Debug()
        {
            try
            {
                var _ = Workflow();
            }
            catch (Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_.Error(e.ToString());
                return false;
            }
            return true;
        }
        public string QueryFileName()
        {
            return Workflow();
        }

        /// <summary>
        /// 定期的に実行する処理を記述する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    var _ = Workflow();
                    if (didSaveTransition.ShouldRegisterSaveEvent)
                    {
                        didSaveTransition.registEvent = true;
                        openedFile_.SaveEvent += (sender_, e_) => RaiseSaveEvent(new SaveEventArgs(openedFile_.Filename, Api.GetTotalTextUnicode()));
                    }
                }
                catch (Exception exception)
                {
                    OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), exception.ToString());
                    logger_.Error(exception.ToString());
                    return;
                }
                return;
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

        DigOpenStatus TryDigOpen(string absFilename)
        {
            try
            {
                if (openedFile_.IsSameFileName(absFilename))
                {
                    return DigOpenStatus.AlreadyOpened;
                }
                var text = Api.GetTotalTextUnicode();
                const int contentsVersion = 1;
                RaiseOpenEvent(new OpenEventArgs(absFilename, text, contentsVersion));
                openedFile_.Setup(absFilename,
                                 new Uri(absFilename),
                                 Api.GetUpdateCount(),
                                 contentsVersion);
                return DigOpenStatus.Opened;
            }
            catch (Exception e)
            {
                logger_?.Error(e.ToString());
            }
            return DigOpenStatus.Failed;
        }
        DigChangeStatus TryDigChange()
        {
            try
            {
                Debug.Assert(openedFile_.IsValidFileName());

                var currentUpdateCount = Api.GetUpdateCount();
                var prevUpdateCount = openedFile_.hidemaruUpdateCount;
                if (currentUpdateCount == prevUpdateCount)
                {
                    return DigChangeStatus.NoChanged;
                }
                openedFile_.UpdateContentsVersion(currentUpdateCount);
                RaiseChangeEvent(new ChangeEventArgs(openedFile_.Filename, Api.GetTotalTextUnicode(), openedFile_.countentsVersion));
                return DigChangeStatus.Changed;
            }
            catch (Exception e)
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
            Debug.Assert(openedFile_.IsValidFileName());
            RaiseCloseEvent(new CloseEventArgs(openedFile_.Filename));
            openedFile_.Clear();
        }

        const string fileNotFound = "";
        string DigOpenProc(string currentHidemaruFilePath)
        {
            switch (TryDigOpen(currentHidemaruFilePath))
            {
                case DigOpenStatus.Opened:
                    return currentHidemaruFilePath;

                case DigOpenStatus.AlreadyOpened:
                    return currentHidemaruFilePath;

                case DigOpenStatus.Failed:
                    logger_.Warn("DigOpenStatus.Failed");
                    return fileNotFound;

                default:
                    logger_.Warn("DigOpenStatus.???");
                    break;
            }
            return fileNotFound;
        }
        /// <summary>
        /// 状態更新
        /// </summary>
        /// <returns>現在、秀丸エディタで開いているファイルの絶対パス</returns>
        string Workflow()
        {
            string currentHidemaruFilePath;
            if (openedFile_.IsValidFileName())
            {
                //2回目以降にファイルを開く場合
                currentHidemaruFilePath = Api.GetFileFullPath();
                if (string.IsNullOrEmpty(currentHidemaruFilePath))
                {
                    //秀丸エディタのファイルが閉じた場合
                    DidClose();
                    return fileNotFound;
                }

                if (openedFile_.IsSameFileName(currentHidemaruFilePath))
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
                return DigOpenProc(currentHidemaruFilePath);
            }
            else
            {
                //初めてファイルを開く場合
                currentHidemaruFilePath = Api.GetFileFullPath();
                if (String.IsNullOrEmpty(currentHidemaruFilePath))
                {
                    return fileNotFound;
                }
                return DigOpenProc(currentHidemaruFilePath);
            }
        }
    }
}
