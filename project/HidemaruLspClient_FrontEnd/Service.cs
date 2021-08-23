﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
    public partial class Service
    {
        static DllAssemblyResolver dasmr_ = new DllAssemblyResolver();

        IHidemaruLspBackEndServer server_ = null;
        IWorker worker_                   = null;
        ILspClientLogger logger_          = null;        

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
        DiagnosticsTask diagnosticsTask_ = null;
        HoverTask hoverTask_ = null;
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

            if (diagnosticsTask_ == null)
            {
                diagnosticsTask_ = new DiagnosticsTask(worker_, logger_, tokenSource_.Token);
            }
            if (hoverTask_ == null)
            {
                hoverTask_=new HoverTask(this, worker_, logger_, tokenSource_.Token);
            }
            return true;
        }
        #region HoverTask
        class HoverTask
        {
            const int defaultMillisecondsTimeout = 500;

            Service service_;
            Task task_;
            IWorker worker_;
            ILspClientLogger logger_;
            CancellationToken cancellationToken_;

            Dll.POINT prevMousePoint_ = new Dll.POINT (0,  0 );
            Hidemaru.Position prevHidemaruPosition_ = new Hidemaru.Position(0, 0);
            Dll.TOOLINFO toolItem_ = new Dll.TOOLINFO();
            IntPtr toolTipHandle_=new IntPtr(0);
            /// <summary>
            /// 秀丸エディタのウインドウハンドル
            /// </summary>
            IntPtr hwndHidemaru_;

            public HoverTask(Service service, IWorker worker, ILspClientLogger logger, CancellationToken cancellationToken)
            {
                service_ = service;
                hwndHidemaru_ = Hidemaru.Hidemaru_GetCurrentWindowHandle();

                worker_ = worker;
                logger_ = logger;
                cancellationToken_ = cancellationToken;
                task_ = Task.Run(MainLoop, cancellationToken_);
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
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    //COMサーバ(.exe)が終了したため、ログ出力してからポーリング動作を終了させる。
                    logger_.Error(e.ToString());
                }
            }
            void Process()
            {
                var currentMousePoint = new Dll.POINT (0,0);
                if (Dll.GetCursorPos(ref currentMousePoint)==false)
                {
                    return;
                }
                if (currentMousePoint == prevMousePoint_)
                {
                    return;
                }

                var currentHidemaruPosition_ = new Hidemaru.Position(0, 0);
                if (Hidemaru.Hidemaru_GetCursorPosUnicodeFromMousePos(ref currentMousePoint,ref currentHidemaruPosition_.line, ref currentHidemaruPosition_.column) == false)
                {
                    return;
                }
                if (! currentHidemaruPosition_.ValueIsCorrect())
                {
                    return;
                }
                prevMousePoint_ = currentMousePoint;
                prevHidemaruPosition_ = currentHidemaruPosition_;
//                System.Diagnostics.Debug.WriteLine(string.Format("column/Line={0},{1}", prevHidemaruPosition_.column, prevHidemaruPosition_.line));

                var text = service_.Hover(currentHidemaruPosition_.line, currentHidemaruPosition_.column);
                ShowToolTips(prevMousePoint_.x, prevMousePoint_.y, text);
            }
            void ShowToolTips(int screenX, int screenY, string text)
            {
                if (toolTipHandle_.ToInt32()==0)
                {
#if true
                    IntPtr hInstance = new IntPtr(0);
                    IntPtr hwnd = new IntPtr(0); 
#else
                    //Memo: 秀丸エディタのウインドウが固まる
                    IntPtr hInstance = Dll.GetModuleHandle(null);
                    IntPtr hwnd = Hidemaru.Hidemaru_GetCurrentWindowHandle();
#endif
                    toolTipHandle_ = CreateTrackingToolTip(hwnd, hInstance, " ");
                    if (toolTipHandle_.ToInt32() == 0)
                    {
                        return;
                    }
                }

                // Activate the tooltip.
                {
                    const int True = 1;
                    SendMessage(toolTipHandle_, (uint)Dll.ToolTipMessage.TTM_TRACKACTIVATE, new IntPtr(True));
                }

                toolItem_.lpszText = text;
                SendMessage(toolTipHandle_, (uint)Dll.ToolTipMessage.TTM_SETTOOLINFO);

                // Position the tooltip. The coordinates are adjusted so that the tooltip does not overlap the mouse pointer.
                {
                    var pt = new Dll.POINT(screenX, screenY);
                    var lparam = new IntPtr(Dll.MAKELONG(pt.x + 10, pt.y - 20));
                    //Dll.ClientToScreen(hWnd, &pt);
                    Dll.SendMessage(toolTipHandle_, (uint)Dll.ToolTipMessage.TTM_TRACKPOSITION, 0, lparam);
                }
            }

            IntPtr CreateTrackingToolTip(IntPtr hwndParent, IntPtr hInstance, string  pText)
            {
                var NULL = new IntPtr(0);
                // Create a tooltip.
                var hwndTT = Dll.CreateWindowEx(Dll.WindowStylesEx.WS_EX_TOPMOST, Dll.TOOLTIPS_CLASS, null,
                    Dll.WindowStyles.WS_POPUP | Dll.WindowStyles.TTS_NOPREFIX | Dll.WindowStyles.TTS_ALWAYSTIP,
                    Dll.CW_USEDEFAULT, Dll.CW_USEDEFAULT, Dll.CW_USEDEFAULT, Dll.CW_USEDEFAULT,
                    hwndParent, NULL, hInstance, NULL);
                if (hwndTT==NULL)
                {
                    return NULL;
                }

                // Set up the tool information. In this case, the "tool" is the entire parent window.

                toolItem_.cbSize   = Dll.TTTOOLINFOW_V2_SIZE;// Marshal.SizeOf(typeof(Dll.TOOLINFO));
                toolItem_.uFlags   = (int)(Dll.ToolTipFlags.TTF_IDISHWND | Dll.ToolTipFlags.TTF_TRACK | Dll.ToolTipFlags.TTF_ABSOLUTE);
                toolItem_.hwnd     = hwndParent;
                toolItem_.hinst    = hInstance;
                toolItem_.lpszText = pText;
                toolItem_.uId      = hwndParent;
                toolItem_.lParam   = NULL;
                Dll.GetClientRect(hwndParent, out toolItem_.rect);

                this.SendMessage(hwndTT, (uint)Dll.ToolTipMessage.TTM_ADDTOOL);
                return hwndTT;
            }
            bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wparam)
            {
                unsafe
                {
                    // Associate the tooltip with the tool window.
                    fixed (int* lp = &toolItem_.cbSize)
                    {
                        var ppp = new IntPtr(lp);
                        if (!Dll.SendMessage(hWnd, Msg, wparam, ppp))
                        {
                            System.Diagnostics.Debug.WriteLine("Failed: TTM_ADDTOOL");
                            return false;
                        }
                    }
                }
                return true;
            }
            bool SendMessage(IntPtr hWnd, uint Msg)
            {
                return this.SendMessage(hWnd,Msg,IntPtr.Zero);
            }
        }

#endregion
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
            diagnosticsTask_ = null;
            worker_      = null;
            server_      = null;
            dasmr_       = null;
            openedFile_  = null;

            return;
        }
#region ServerCapabilities
        public ServerCapabilitiesImpl ServerCapabilities()
        {
            try
            {
                Debug.Assert(worker_ != null);
                return new ServerCapabilitiesImpl(worker_.ServerCapabilities);
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            return null;
        }
        public sealed class ServerCapabilitiesImpl : HidemaruLspClient_BackEndContract.IServerCapabilities
        {
            HidemaruLspClient_BackEndContract.IServerCapabilities serverCapabilities_;
            public ServerCapabilitiesImpl(HidemaruLspClient_BackEndContract.IServerCapabilities serverCapabilities)
            {
                serverCapabilities_ = serverCapabilities;
            }
            public sbyte CompletionProvider => serverCapabilities_.CompletionProvider;

            public sbyte HoverProvider => serverCapabilities_.HoverProvider;

            public sbyte SignatureHelpProvider => serverCapabilities_.SignatureHelpProvider;

            public sbyte DeclarationProvider => serverCapabilities_.DeclarationProvider;

            public sbyte DefinitionProvider => serverCapabilities_.DefinitionProvider;

            public sbyte TypeDefinitionProvider => serverCapabilities_.TypeDefinitionProvider;

            public sbyte ImplementationProvider => serverCapabilities_.ImplementationProvider;

            public sbyte ReferencesProvider => serverCapabilities_.ReferencesProvider;

            public sbyte DocumentHighlightProvider => serverCapabilities_.DocumentHighlightProvider;

            public sbyte DocumentSymbolProvider => serverCapabilities_.DocumentSymbolProvider;

            public sbyte CodeActionProvider => serverCapabilities_.CodeActionProvider;

            public sbyte CodeLensProvider => serverCapabilities_.CodeLensProvider;

            public sbyte DocumentLinkProvider => serverCapabilities_.DocumentLinkProvider;

            public sbyte ColorProvider => serverCapabilities_.ColorProvider;

            public sbyte DocumentFormattingProvider => serverCapabilities_.DocumentFormattingProvider;

            public sbyte DocumentRangeFormattingProvider => serverCapabilities_.DocumentRangeFormattingProvider;

            public sbyte DocumentOnTypeFormattingProvider => serverCapabilities_.DocumentOnTypeFormattingProvider;

            public sbyte RenameProvider => serverCapabilities_.RenameProvider;

            public sbyte FoldingRangeProvider => serverCapabilities_.FoldingRangeProvider;

            public sbyte ExecuteCommandProvider => serverCapabilities_.ExecuteCommandProvider;

            public sbyte SelectionRangeProvider => serverCapabilities_.SelectionRangeProvider;

            public sbyte LinkedEditingRangeProvider => serverCapabilities_.LinkedEditingRangeProvider;

            public sbyte CallHierarchyProvider => serverCapabilities_.CallHierarchyProvider;

            public sbyte SemanticTokensProvider => serverCapabilities_.SemanticTokensProvider;

            public sbyte MonikerProvider => serverCapabilities_.MonikerProvider;

            public sbyte WorkspaceSymbolProvider => serverCapabilities_.WorkspaceSymbolProvider;
        }
#endregion

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

#region Impl
        delegate ILocationContainer InvokeWorker(string absFilename, long line, long character);
        LocationContainerImpl CommonImplementationsOfGoto(long hidemaruLine, long hidemaruColumn, InvokeWorker invoke)
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
                var locations = invoke(absFileName, line, character);
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


        public LocationContainerImpl Declaration(long hidemaruLine, long hidemaruColumn)
        {
            return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, worker_.Declaration);
        }
        public LocationContainerImpl Definition(long hidemaruLine, long hidemaruColumn)
        {
            return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, worker_.Definition);
        }
        public LocationContainerImpl TypeDefinition(long hidemaruLine, long hidemaruColumn)
        {
            return CommonImplementationsOfGoto(hidemaruLine,hidemaruColumn, worker_.TypeDefinition);
        }
        public LocationContainerImpl Implementation(long hidemaruLine, long hidemaruColumn)
        {
            return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, worker_.Implementation);
        }
        public LocationContainerImpl References(long hidemaruLine, long hidemaruColumn)
        {
            return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, worker_.References);
        }

#region Hover
        public string Hover(long hidemaruLine, long hidemaruColumn)
        {
            try
            {
                Debug.Assert(worker_ != null);

                var absFileName = FileProc();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return "";
                }
                long line, character;
                Hidemaru.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                var hover = worker_.Hover(absFileName, line, character);
                if (hover == null)
                {
                    return "";
                }
                return hover.contents.value;
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            return "";
        }

        
        
#endregion


#endregion

    }
}
