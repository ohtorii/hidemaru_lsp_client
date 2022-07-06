using System;
using System.IO;


namespace HidemaruLspClient_FrontEnd.Facility
{
    /// <summary>
    /// 秀丸エディタで編集しているドキュメント
    /// </summary>
    class HidemaruEditorDocument
    {
        string Filename_;
        Uri Uri_;
        int hidemaruUpdateCount_;
        int contentsVersion_;

        /// <summary>
        /// 秀丸エディタのファイルセーブを検出するのが目的
        /// </summary>
        FileSystemWatcher fileWatcher_;

        public delegate void SaveEventHandler(object sender, EventArgs e);
        /// <summary>
        /// ファイルセーブが発生した時に呼ばれる
        /// </summary>
        public event SaveEventHandler SaveEvent;

        /// <summary>
        /// ドキュメントのファイル名を取得を試みる
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>true ドキュメントのファイル名が格納されている, false それ以外</returns>
        public bool TryGetFileName(out string filename)
        {
            filename = this.Filename_;
            return IsValidFileName();
        }
        /// <summary>
        /// ドキュメントのファイル名が格納されているかどうか確認する
        /// </summary>
        /// <returns>true 正しいファイル / false 正しくないファイル</returns>
        public bool IsValidFileName()
        {
            return !string.IsNullOrEmpty(this.Filename_);
        }
        /// <summary>
        /// ドキュメントのファイル名が同じかどうか
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool IsSameFileName(string filename)
        {
            string currentFileName;
            if(TryGetFileName(out currentFileName))
            {
                return filename == currentFileName;
            }
            return false;
        }
        /// <summary>
        /// ドキュメントのファイル名を取得する
        /// </summary>
        public string Filename { get { return this.Filename_; } }
        public Uri Uri { get { return this.Uri_; } }
        /// <summary>
        /// 秀丸エディタのUpdateCount値（飛び飛びの値）
        /// </summary>
        public int hidemaruUpdateCount { get { return this.hidemaruUpdateCount_; } }
        /// <summary>
        /// LSPへ渡すContentsVersion（連続した値）
        /// </summary>
        public int countentsVersion { get { return this.contentsVersion_; } }
        public HidemaruEditorDocument()
        {
            Initialize();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="filename">絶対パス</param>
        /// <param name="uri">filenameのUri表記</param>
        /// <param name="hidemaruUpdateCount"></param>
        /// <param name="contentsVersion"></param>
        public void Setup(string filename, Uri uri, int hidemaruUpdateCount, int contentsVersion)
        {
            this.Filename_ = filename;
            this.Uri_ = uri;
            this.hidemaruUpdateCount_ = hidemaruUpdateCount;
            this.contentsVersion_ = contentsVersion;

            if (this.fileWatcher_ != null){
                this.fileWatcher_.Dispose();
                this.fileWatcher_ = null;
            }
            this.fileWatcher_ = new FileSystemWatcher(Path.GetDirectoryName(filename),Path.GetFileName(filename));
            this.fileWatcher_.NotifyFilter = NotifyFilters.LastWrite;
            this.fileWatcher_.SynchronizingObject = Utils.UIThread.SynchronizingObject;
            this.fileWatcher_.Changed += FileWatcher__Changed;
            this.fileWatcher_.EnableRaisingEvents = true; ;
        }

        private void FileWatcher__Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            if (SaveEvent == null) {
                return;
            }
            SaveEvent(this,new EventArgs());
        }


        public void Clear()
        {
            if (this.fileWatcher_ != null)
            {
                this.fileWatcher_.Dispose();
                this.fileWatcher_ = null;
            }
            Initialize();
        }
        public void UpdateContentsVersion(int hidemaruUpdateCount){
            hidemaruUpdateCount_= hidemaruUpdateCount;
            ++contentsVersion_;
        }

        void Initialize() {
            Filename_ = "";
            Uri_ = null;
            hidemaruUpdateCount_ = 0;
            contentsVersion_ = 0;
            fileWatcher_ = null;
        }
    }

}
