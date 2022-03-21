using System;


namespace HidemaruLspClient_FrontEnd
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
