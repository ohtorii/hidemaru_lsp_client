using System;


namespace HidemaruLspClient_FrontEnd.Facility
{
    /// <summary>
    /// 秀丸エディタで編集しているドキュメントの情報
    /// </summary>
    class HidemaruEditorDocument
    {
        string Filename_;
        Uri Uri_;
        int hidemaruUpdateCount_;
        int contentsVersion_;
        /// <summary>
        /// ドキュメントのファイル名を取得を試みる
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>true ドキュメントのファイル名が格納されている, false それ以外</returns>
        public bool TryGetFileName(out string filename)
        {
            filename= this.Filename_;
            return IsValidFileName();
        }
        /// <summary>
        /// ドキュメントのファイル名が格納されているかどうか確認する
        /// </summary>
        /// <returns>true 正しいファイル / false 正しくないファイル</returns>
        public bool IsValidFileName()
        {
            return string.IsNullOrEmpty(this.Filename_)!=false;
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
        public void Setup(string filename, Uri uri, int hidemaruUpdateCount, int contentsVersion)
        {
            this.Filename_ = filename;
            this.Uri_ = uri;
            this.hidemaruUpdateCount_ = hidemaruUpdateCount;
            this.contentsVersion_ = contentsVersion;
        }
        public void Clear()
        {
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
        }
    }

}
