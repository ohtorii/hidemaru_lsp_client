using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using HidemaruLspClient_BackEndContract;

namespace HidemaruLspClient_FrontEnd
{    
    /// <summary>
    /// 秀丸エディタへ公開するクラス
    /// </summary>
    [ComVisible(true)]
    [Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9590")]
    public class Service
    {
        static DllAssemblyResolver dasmr_ = new DllAssemblyResolver();

        IHidemaruLspBackEndServer server_ = null;
        IWorker worker_ = null;
        ILspClientLogger logger_ = null;        

        class Document
        {
            string Filename_;
            Uri Uri_;
            int ContentsVersion_;
            int ContentsHash_;

            public string Filename { get { return this.Filename_; } }
            public Uri Uri { get { return this.Uri_; } }
            public int ContentsVersion { get { return this.ContentsVersion_; } }
            public int ContentsHash { get { return this.ContentsHash_; } }

            public Document()
            {
                Initialize();
            }
            public void Setup(string filename, Uri uri, int contentsVersion, int contentsHash)
            {
                this.Filename_ = filename;
                this.Uri_ = uri;
                this.ContentsVersion_ = contentsVersion;
                this.ContentsHash_ = contentsHash;
            }
            public void Clear()
            {
                Initialize();
            }
            public void IncrementContentsVersion(){
                ++this.ContentsVersion_;
            }


            void Initialize() {
                Filename_ = "";
                Uri_ = null;
                ContentsVersion_ = 1;
                ContentsHash_ = 0;
            }
        }
        Document openedFile_ = new Document();

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

        public bool Initialize(string logFilename)
        {
            try
            {
                Hidemaru.Initialize();

                if (server_ == null)
                {
                    //事前にBackEndをspawnしたほうが良いと思う
                    var ServerClassGuid = new Guid((Attribute.GetCustomAttribute(typeof(ServerClass), typeof(GuidAttribute)) as GuidAttribute).Value);
                    object obj;
                    int hr = Ole32.CoCreateInstance(ServerClassGuid, IntPtr.Zero, Ole32.CLSCTX_LOCAL_SERVER, typeof(IHidemaruLspBackEndServer).GUID, out obj);
                    if (hr < 0)
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }
                    server_ = (IHidemaruLspBackEndServer)obj;
                    var ret = Convert.ToBoolean(server_.Initialize(logFilename));
                    if (ret)
                    {                     
                        logger_     = server_.GetLogger();
                        Configuration.Initialize(logger_);
                    }
                    else
                    {
                        server_ = null;
                    }
                    return ret;
                }
                return true;
            }
            catch(Exception e)
            {
                server_ = null;
            }
            return false;
        }
        /// <summary>
        /// テスト用のメソッド
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Add(int x, int y)
        {
            try
            {
                return server_.Add(x, y);
            }
            catch (System.Exception )
            {
                return -1;
            }
        }
        public void Finalizer(int reason)
        {
            try
            {
                if ((server_ != null) && (worker_ != null))
                {
                    if (string.IsNullOrEmpty(openedFile_.Filename) == false)
                    {
                        DidClose();
                    }
                    server_.DestroyWorker(worker_);
                }
                worker_     = null;
                server_     = null;
                dasmr_      = null;
                openedFile_ = null;
            }
            catch (Exception e)
            {
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            logger_ = null;
            return;
        }
        public bool CreateWorker(string serverConfigFilename, string currentSourceCodeDirectory)
        {
            try
            {
                logger_.Trace("CreateWorker@enter");
                Debug.Assert(worker_ == null);

                logger_.Trace("CreateWorker@1");
                var options = Configuration.Eval(serverConfigFilename, currentSourceCodeDirectory);
                if (options == null)
                {
                    return false;
                }
                logger_.Trace("CreateWorker@2");
                worker_ =server_.CreateWorker(
                            options.ServerName,
                            options.ExcutablePath, 
                            options.Arguments,
                            options.RootUri,
                            options.WorkspaceConfig);
                logger_.Trace("CreateWorker@3");
                if (worker_ == null)
                {
                    return false;
                }
                logger_.Trace("CreateWorker@4");
                return true;
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            logger_.Trace("CreateWorker@exit");
            return false;
        }
        DigOpenStatus DigOpen(string absFilename)
        {
            try
            {
                Debug.Assert(worker_ != null);

                if (openedFile_.Filename == absFilename)
                {
                    return DigOpenStatus.AlreadyOpened;
                }
                var text = Hidemaru.GetTotalTextUnicode();
                const int contentsVersion = 1;
                worker_.DidOpen(absFilename, text, contentsVersion);                
                openedFile_.Setup(absFilename,
                                 new Uri(absFilename),
                                 contentsVersion,
                                 text.GetHashCode());
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
        DigChangeStatus DigChange()
        {
            try
            {
                Debug.Assert(worker_ != null);
                Debug.Assert(string.IsNullOrEmpty(openedFile_.Filename) == false);

                var text = Hidemaru.GetTotalTextUnicode();
                {
                    var currentHash = text.GetHashCode();
                    var prevHash = openedFile_.ContentsHash;
                    if (currentHash == prevHash)
                    {
                        return DigChangeStatus.NoChanged;
                    }
                }
                openedFile_.IncrementContentsVersion();
                worker_.DidChange(openedFile_.Filename, text, openedFile_.ContentsVersion);
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
            Debug.Assert(worker_ != null);
            Debug.Assert(string.IsNullOrEmpty(openedFile_.Filename) == false);

            worker_.DidClose(openedFile_.Filename);
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
                if (DigChange() == DigChangeStatus.Failed)
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

        /// <summary>
        /// textDocument/completion
        /// </summary>
        /// <param name="absFilename"></param>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns>一時的な辞書ファイル名（ファイルはCOM側で一定時間後に削除します）</returns>
        public string Completion(long line, long column)
        {
            try
            {
                Debug.Assert(worker_ != null);

                var absFileName=FileProc();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return "";
                }
                return worker_.Completion(absFileName, line, column);
            }
            catch(Exception e)
            {
                logger_.Error(e.ToString());
            }
            return "";
        }
        /// <summary>
        /// textDocument/publishDiagnostics
        /// </summary>        
        /// <returns>秀丸エディタのアウトプット枠へ出力する文字列</returns>
        public string Diagnostics()
        {
            //Todo: COMサーバから「秀丸エディタのアウトプット枠へ出力する文字列」を返すようにする（もっと単純にする）
            try
            {
                Debug.Assert(worker_ != null);

                var sb        = new StringBuilder();
                var container = worker_.PullDiagnosticsParams();
                for(long i=0; i< container.Length; ++i)
                {
                    FormatDiagnostics(sb,container.Item(i));
                }
                return sb.ToString();
            }
            catch(Exception e)
            {
                logger_.Error(e.ToString());
            }
            return "";
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

                    //Memo: 秀丸エディタのアウトプット枠へ出力するには \r\n が必要。
                    sb.Append($"{filename}({line}):  {code} {message}\r\n");
                }
            }
        }



        private class Ole32
        {
            // https://docs.microsoft.com/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx
            public const int CLSCTX_LOCAL_SERVER = 0x4;

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance
            [DllImport(nameof(Ole32))]
            public static extern int CoCreateInstance(
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
                IntPtr pUnkOuter,
                uint dwClsContext,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
        }

    }

    
}
