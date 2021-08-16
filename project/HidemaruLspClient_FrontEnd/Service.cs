using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            public static int CalcTextHash(string text)
            {
                return text.GetHashCode();
            }
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
            public void SetContentsHash(int hash)
            {
                this.ContentsHash_ = hash;
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
                                 Document.CalcTextHash(text));
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
                Debug.Assert(worker_ != null);
                Debug.Assert(string.IsNullOrEmpty(openedFile_.Filename) == false);

                var text        = Hidemaru.GetTotalTextUnicode();                
                var currentHash = Document.CalcTextHash(text);
                var prevHash    = openedFile_.ContentsHash;
                if (currentHash == prevHash)
                {
                    return DigChangeStatus.NoChanged;
                }
                worker_.DidChange(openedFile_.Filename, text, openedFile_.ContentsVersion);

                openedFile_.SetContentsHash(currentHash);
                openedFile_.IncrementContentsVersion();
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
        
        bool CreateComServer(string logFilename)
        {
            if (server_ != null)
            {
                return true;
            }

            try
            {            
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
                    logger_ = server_.GetLogger();
                    Configuration.Initialize(logger_);
                }
                else
                {
                    server_ = null;
                }
                return ret;
            }
            catch (Exception e)
            {
                server_ = null;
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            return false;
        }
        bool CreateWorker(string serverConfigFilename, string currentSourceCodeDirectory)
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
                var hidemaruProcess = System.Diagnostics.Process.GetCurrentProcess();
                logger_.Trace("CreateWorker@2");
                worker_ = server_.CreateWorker(
                            options.ServerName,
                            options.ExcutablePath,
                            options.Arguments,
                            options.RootUri,
                            options.WorkspaceConfig,
                            hidemaruProcess.Id);
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

        
        CancellationTokenSource tokenSource_=null;
        Diagnostics diagnostics_ = null;

        /// <summary>
        /// 定期的な処理
        /// </summary>
        void RoutineTask(CancellationToken  cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// textDocument/publishDiagnostics
        /// </summary>        
        class Diagnostics
        {
            Task task_;
            IWorker worker_;
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

            public Diagnostics(IWorker worker, CancellationToken cancellationToken)
            {
                hwndHidemaru_     = Hidemaru.Hidemaru_GetCurrentWindowHandle();
                outputPaneCleard_ = false;

                worker_            = worker;
                cancellationToken_ = cancellationToken;                
                task_              =Task.Run(MainLoop, cancellationToken_);
            }
            void MainLoop()
            {
                //Todoポーリングではなくイベント通知にする
                while (true)
                {
                    if (cancellationToken_.IsCancellationRequested)
                    {
                        return;
                    }
                    Process();
                    Thread.Sleep(500);
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

        #region Public methods
        public bool Initialize(string logFilename, string serverConfigFilename, string currentSourceCodeDirectory)
        {
            Hidemaru.Initialize();

            if (!CreateComServer(logFilename))
            {
                Finalizer();
                return false;
            }
            if (!CreateWorker(serverConfigFilename, currentSourceCodeDirectory))
            {
                Finalizer();
                return false;
            }
            if(tokenSource_==null)
            {
                tokenSource_ = new CancellationTokenSource();
            }

            if (diagnostics_ == null)
            {
                diagnostics_ = new Diagnostics(worker_, tokenSource_.Token);
            }
            
            return true;
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
            catch (System.Exception)
            {
                return -1;
            }
        }
        public void Finalizer(int reason=0)
        {
            try
            {
                if (tokenSource_ != null)
                {
                    tokenSource_.Cancel();
                }

                if ((server_ != null) && (worker_ != null))
                {
                    if (string.IsNullOrEmpty(openedFile_.Filename) == false)
                    {
                        DidClose();
                    }
                    server_.DestroyWorker(worker_);
                }
            }
            catch (Exception e)
            {
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            logger_      = null;

            tokenSource_ = null;
            diagnostics_ = null;
            worker_      = null;
            server_      = null;
            dasmr_       = null;
            openedFile_  = null;

            return;
        }

        /// <summary>
        /// 秀丸エディタのテキストとサーバ側のテキストを同期する（デバッグ用途）
        /// </summary>
        /// <returns></returns>
        public bool SyncDocument()
        {
            try
            {
                Debug.Assert(worker_ != null);
                var _ = FileProc();
            }catch(Exception e)
            {
                logger_.Error(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// textDocument/completion
        /// </summary>
        /// <param name="absFilename"></param>
        /// <param name="hidemaruLine"></param>
        /// <param name="hidemaruColumn"></param>
        /// <returns>一時的な辞書ファイル名（ファイルはCOM側で一定時間後に削除します）</returns>
        public string Completion(long hidemaruLine, long hidemaruColumn)
        {
            try
            {
                Debug.Assert(worker_ != null);

                var absFileName=FileProc();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return "";
                }
                long line, character;
                Hidemaru.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                return worker_.Completion(absFileName, line, character);
            }
            catch(Exception e)
            {
                logger_.Error(e.ToString());
            }
            return "";
        }
        //Todo: 後で実装
        public int Declaration(long hidemaruLine, long hidemaruColumn)
        {
            try
            {
                Debug.Assert(worker_ != null);

                var absFileName = FileProc();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return 0;
                }
                long line, character;
                Hidemaru.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                worker_.Declaration(absFileName, line, character);
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            return 0;
        }
        #region Definition
        public LocationContainerImpl Definition(long hidemaruLine, long hidemaruColumn)
        {
            try
            {
                Debug.Assert(worker_ != null);

                var absFileName = FileProc();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return null;
                }
                long line, character;
                Hidemaru.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                var locations = worker_.Definition(absFileName, line, character);
                return new LocationContainerImpl(locations);
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            return null;
        }
        public sealed class PositionImpl
        {
            public PositionImpl(HidemaruLspClient_BackEndContract.IPosition position)
            {
                if (position == null)
                {
                    hidemaruCharacter_ = -1;
                    hidemaruLine_ = -1;
                    return;
                }
                Hidemaru.ZeroBaseToHidemaru(out hidemaruLine_,
                                            out hidemaruCharacter_,
                                            position.line,
                                            position.character);
            }
            public long character => hidemaruCharacter_;
            public long line => hidemaruLine_;
            
            readonly long hidemaruCharacter_;
            readonly long hidemaruLine_;
        }
        public sealed class RangeImpl
        {
            public RangeImpl(HidemaruLspClient_BackEndContract.IRange range)
            {
                range_ = range;
            }
            public PositionImpl start { 
                get {
                    if (range_ == null)
                    {
                        return null;
                    }
                    return new PositionImpl(range_.start);
                } 
            }
            public PositionImpl end { 
                get {
                    if (range_ == null)
                    {
                        return null;
                    }
                    return new PositionImpl(range_.end);
                } 
            }
            readonly HidemaruLspClient_BackEndContract.IRange range_;
        }
        public sealed class LocationImpl
        {
            public LocationImpl(HidemaruLspClient_BackEndContract.ILocation location)
            {
                location_ = location;
            }
            public string AbsFilename { 
                get {
                    if (location_ == null)
                    {
                        return "";
                    }
                    var uri = new Uri(location_.uri);
                    return uri.AbsolutePath;
                } 
            }
            public RangeImpl range { 
                get {
                    if (location_ == null)
                    {
                        return null;
                    }
                    return new RangeImpl(location_.range);
                } 
            }
            readonly HidemaruLspClient_BackEndContract.ILocation location_;
        }
    
        public sealed class LocationContainerImpl 
        {
            public LocationContainerImpl(HidemaruLspClient_BackEndContract.ILocationContainer locations)
            {
                locations_ = locations;
            }
            public LocationImpl Item(long index)
            {
                if (locations_ == null)
                {
                    return null;
                }
                return new LocationImpl(locations_.Item(index));
            }

            public long Length
            {
                get
                {
                    if (locations_ == null)
                    {
                        return 0;
                    }
                    return locations_.Length;
                }
            }

            readonly HidemaruLspClient_BackEndContract.ILocationContainer locations_;
        }
        #endregion

        #endregion

    }   
}
