﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HidemaruLspClient_BackEndContract;

namespace HidemaruLspClient_FrontEnd
{

    /// <summary>
    /// 秀丸エディタへ公開するクラス（同期版）
    /// </summary>
    [ComVisible(true)]
    [Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9590")]
    public sealed class Service : IService
    {
        static DllAssemblyResolver dasmr_ = new DllAssemblyResolver();

        class Context
        {
            public IHidemaruLspBackEndServer server { get; set; }
            public IWorker worker { get; set; }
        }
        Context context_ =null;
        ILspClientLogger logger_=null;



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
        Document openedFile_ = null;

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
                context_.worker.DidOpen(absFilename, text, contentsVersion);
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
                Debug.Assert(string.IsNullOrEmpty(openedFile_.Filename) == false);

                var text        = Hidemaru.GetTotalTextUnicode();
                var currentHash = Document.CalcTextHash(text);
                var prevHash    = openedFile_.ContentsHash;
                if (currentHash == prevHash)
                {
                    return DigChangeStatus.NoChanged;
                }
                context_.worker.DidChange(openedFile_.Filename, text, openedFile_.ContentsVersion);

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
            Debug.Assert(string.IsNullOrEmpty(openedFile_.Filename) == false);
            if (context_.worker == null)
            {
                return;
            }
            context_.worker.DidClose(openedFile_.Filename);
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
            if (context_.server != null)
            {
                return true;
            }

            try
            {
                Debug.Assert(true);
                var ServerClassGuid = LspContract.Constants.ServerClassGuid;    //new Guid((Attribute.GetCustomAttribute(typeof(ServerClass), typeof(GuidAttribute)) as GuidAttribute).Value);
                object obj;
                int hr = Ole32.CoCreateInstance(ServerClassGuid, IntPtr.Zero, Ole32.CLSCTX_LOCAL_SERVER, typeof(IHidemaruLspBackEndServer).GUID, out obj);
                if (hr < 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
                context_.server = (IHidemaruLspBackEndServer)obj;
                var ret = Convert.ToBoolean(context_.server.Initialize(logFilename));
                if (ret)
                {
                    logger_ = context_.server.GetLogger();
                    Configuration.Initialize(logger_);
                }
                else
                {
                    context_.server = null;
                }
                return ret;
            }
            catch (Exception e)
            {
                context_.server = null;
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            return false;
        }
        bool CreateWorkerMain(string serverConfigFilename, string currentSourceCodeDirectory)
        {
            if(context_.worker != null)
            {
                return true;
            }

            try
            {
                var options = Configuration.Eval(serverConfigFilename, currentSourceCodeDirectory);
                if (options == null)
                {
                    return false;
                }
                var hidemaruProcess = System.Diagnostics.Process.GetCurrentProcess();
                context_.worker = context_.server.CreateWorker(
                            options.ServerName,
                            options.ExcutablePath,
                            options.Arguments,
                            options.RootUri,
                            options.WorkspaceConfig,
                            hidemaruProcess.Id);
                if (context_.worker == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            context_.worker = null;
            return false;
        }
        void FinalizeContext()
        {
            if ((context_.server != null) && (context_.worker != null))
            {
                if (string.IsNullOrEmpty(openedFile_.Filename) == false)
                {
                    DidClose();
                }
                context_.server.DestroyWorker(context_.worker);
            }
            context_.worker = null;
            context_.server = null;
        }

        /// <summary>
        /// iniファイルからサーバ設定ファイルを見付ける
        /// </summary>
        /// <param name="fileExtension">ファイル拡張子(".c", ".cpp" ...)</param>
        /// <returns>サーバ設定ファイル（絶対パス）、または空文字</returns>
        string FindServerConfigFile(string fileExtension)
        {
            try
            {
                var path = iniReader_.Read(fileExtension, "ServerConfig");
                if (path == "")
                {
                    return "";
                }
                if (Path.IsPathRooted(path))
                {
                    return path;
                }
                //iniFileからの相対パス→絶対パス
                var absFileName = Path.Combine(iniFileDirectory_, path);
                return absFileName;
            }
            catch (Exception e)
            {
                logger_?.Error(e.ToString());
            }
            return "";
        }

        CancellationTokenSource tokenSource_=null;
        DiagnosticsTask diagnosticsTask_ = null;
        HoverTask hoverTask_ = null;
        DidChangeTask didChangeTask_ = null;

        IniFile iniReader_=null;
        string iniFileDirectory_ = null;

#region Public methods
        public Service()
        {
            tokenSource_ = new CancellationTokenSource();
            context_ = new Context();
            openedFile_ = new Document();
            UIThread.Initializer();
            Hidemaru.Initialize();
        }

        public bool Initialize(string iniFileName)
        {
            try
            {
                if (! File.Exists(iniFileName))
                {
                    return false;
                }
                iniReader_ = new IniFile(iniFileName);
                iniFileDirectory_= Path.GetDirectoryName(iniReader_.Filename);
                return true;
            }catch(Exception e)
            {
                logger_?.Error(e.ToString());
            }
            return false;
        }
        /// <summary>
        /// BackEndを初期化する（非同期版）
        /// </summary>
        /// <param name="logFilename"></param>
        public void InitializeBackEndServiceAsync(string logFilename)
        {
            var _ = Task.Run(() => {
                lock (context_)
                {
                    InitializeBackEndService(logFilename);
                }
            }, tokenSource_.Token);
        }
        /// <summary>
        /// back endの初期化が終了したか調べる
        /// </summary>
        /// <returns></returns>
        public bool CheckBackEndService()
        {
            bool token = false;
            try
            {
                Monitor.TryEnter(context_, ref token);
                if (token)
                {
                    if (context_.server == null)
                    {
                        return false;
                    }
                    return true;
                }
            }finally{
                if (token)
                {
                    Monitor.Exit(context_);
                }
            }
            return false;
        }
        /// <summary>
        /// BackEndを初期化する（同期版）
        /// </summary>
        /// <param name="logFilename"></param>
        /// <returns></returns>
        public bool InitializeBackEndService(string logFilename)
        {
            if (!CreateComServer(logFilename))
            {
                Finalizer();
                return false;
            }
            return true;
        }
        /// <summary>
        /// front endを初期化する（非同期版）
        /// </summary>
        /// <param name="serverConfigFilename"></param>
        /// <param name="currentSourceCodeDirectory"></param>
        public void InitializeFrontEndServiceAsync(string fileExtension, string currentSourceCodeDirectory)
        {
            var _ = Task.Run(() =>
            {
                lock (context_)
                {
                    InitializeFrontEndService(fileExtension,currentSourceCodeDirectory);
                }
            }, tokenSource_.Token);
        }
        /// <summary>
        /// front endの初期化が終了したか調べる
        /// </summary>
        /// <returns></returns>
        public bool CheckFrontEndService()
        {
            bool token = false;
            try
            {
                Monitor.TryEnter(context_, ref token);
                if (token)
                {
                    if (context_.worker == null)
                    {
                        return false;
                    }
                    return true;
                }
            }
            finally
            {
                if (token)
                {
                    Monitor.Exit(context_);
                }
            }
            return false;
        }
        /// <summary>
        /// front endを初期化する（同期版）
        /// </summary>
        /// <param name="serverConfigFilename"></param>
        /// <param name="currentSourceCodeDirectory"></param>
        /// <returns></returns>
        public bool InitializeFrontEndService(string fileExtension, string currentSourceCodeDirectory) {
            var serverConfigFilename=FindServerConfigFile(fileExtension);
            if (serverConfigFilename == "")
            {
                Finalizer();
                return false;
            }
            if (!CreateWorkerMain(serverConfigFilename, currentSourceCodeDirectory))
            {
                Finalizer();
                return false;
            }
            UIThread.Invoke((MethodInvoker)delegate
            {
                if (diagnosticsTask_ == null)
                {
                    diagnosticsTask_ = new DiagnosticsTask(context_.worker, logger_, tokenSource_.Token);
                }
                if (hoverTask_ == null)
                {
                    hoverTask_ = new HoverTask(this, context_.worker, logger_, tokenSource_.Token);
                }
                if (didChangeTask_ == null)
                {
                    didChangeTask_ = new DidChangeTask(this, logger_, tokenSource_.Token);
                }
            });
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
                return context_.server.Add(x, y);
            }
            catch (System.Exception)
            {
                return -1;
            }
        }
        public void Finalizer(int reason=0)
        {
            logger_?.Trace("Enter Finalizer");
            try
            {
                if (tokenSource_ != null)
                {
                    tokenSource_.Cancel();
                }
                lock (context_)
                {
                    FinalizeContext();
                }
                UIThread.Finalizer();
            }
            catch (Exception e)
            {
                logger_?.Error(e.ToString());
            }

            diagnosticsTask_ = null;
            hoverTask_ = null;
            didChangeTask_ = null;

            tokenSource_ = null;
            dasmr_       = null;
            openedFile_  = null;
            context_ = null;

            iniReader_ = null;
            iniFileDirectory_ = null;

            logger_?.Trace("Leave Finalizer");
            logger_ = null;
        }

        public ServerCapabilitiesImpl ServerCapabilities()
        {
            if (context_.worker == null)
            {
                return null;
            }
            try
            {
                return new ServerCapabilitiesImpl(context_.worker.ServerCapabilities);
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            return null;
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
            if (context_.worker == null)
            {
                return "";
            }
            try
            {
                var absFileName=FileProc();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return "";
                }
                long line, character;
                Hidemaru.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                return context_.worker.Completion(absFileName, line, character);
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

#endregion


        public LocationContainerImpl Declaration(long hidemaruLine, long hidemaruColumn)
        {
            if (context_.worker == null){
                return null;
            }
            return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, context_.worker.Declaration);
        }
        public LocationContainerImpl Definition(long hidemaruLine, long hidemaruColumn)
        {
            if (context_.worker == null)
            {
                return null;
            }
            return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, context_.worker.Definition);
        }
        public LocationContainerImpl TypeDefinition(long hidemaruLine, long hidemaruColumn)
        {
            if (context_.worker == null)
            {
                return null;
            }
            return CommonImplementationsOfGoto(hidemaruLine,hidemaruColumn, context_.worker.TypeDefinition);
        }
        public LocationContainerImpl Implementation(long hidemaruLine, long hidemaruColumn)
        {
            if (context_.worker == null)
            {
                return null;
            }
            return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, context_.worker.Implementation);
        }
        public LocationContainerImpl References(long hidemaruLine, long hidemaruColumn)
        {
            if (context_.worker == null)
            {
                return null;
            }
            return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, context_.worker.References);
        }

#region Hover
        internal string Hover(long hidemaruLine, long hidemaruColumn)
        {
            if (context_.worker == null)
            {
                return "";
            }

            try
            {
                var absFileName = FileProc();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return "";
                }
                long line, character;
                Hidemaru.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                var hover = context_.worker.Hover(absFileName, line, character);
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
