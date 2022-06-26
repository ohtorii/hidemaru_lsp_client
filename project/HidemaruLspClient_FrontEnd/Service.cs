using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HidemaruLspClient_BackEndContract;
using HidemaruLspClient_FrontEnd.Native;
using HidemaruLspClient_FrontEnd.BackgroundTask;
using HidemaruLspClient_FrontEnd.Facility;
using HidemaruLspClient_FrontEnd.Hidemaru;
using HidemaruLspClient_FrontEnd.BackEndContractImpl;

namespace HidemaruLspClient_FrontEnd
{

    /// <summary>
    /// 秀丸エディタへ公開するクラス（同期版）
    /// </summary>
    [ComVisible(true)]
    [Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9590")]
    public sealed partial class Service : IService
    {
        static DllAssemblyResolver dasmr_= new DllAssemblyResolver();

        class Context
        {
            public IHidemaruLspBackEndServer server { get; set; }
            public IWorker worker { get; set; }
        }
        Context context_ =null;
        ILspClientLogger logger_=null;

        CancellationTokenSource tokenSource_ = null;
        Diagnostics diagnosticsTask_ = null;
        Hover hoverTask_ = null;
        SyncDocumenmt syncDocumentTask_ = null;
        IniFileService iniFile_ = null;


        bool CreateWorkerMain(string serverConfigFilename, string currentSourceCodeDirectory)
        {
            if(context_.worker != null)
            {
                return true;
            }
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
        void FinalizeContext()
        {
            if ((context_.server != null) && (context_.worker != null))
            {
                syncDocumentTask_.Finish();
                context_.server.DestroyWorker(context_.worker);
            }
            context_.worker = null;
            context_.server = null;
        }
        bool InitializeBackEndServiceMain()
        {
            lock (context_)
            {
                if (context_.server != null)
                {
                    return true;
                }

                try
                {
                    {
                        var ServerClassGuid = LspContract.Constants.ServerClassGuid;
                        object obj;
                        int hr = Ole32.CoCreateInstance(ServerClassGuid, IntPtr.Zero, Ole32.CLSCTX_LOCAL_SERVER, typeof(IHidemaruLspBackEndServer).GUID, out obj);
                        if (hr < 0)
                        {
                            Marshal.ThrowExceptionForHR(hr);
                        }
                        context_.server = (IHidemaruLspBackEndServer)obj;

                        //CrashReport送信は個人情報に関わる処理なのでサーバ起動直後に真偽値をセットする（Initializeより先に呼びだして良い）
                        context_.server.EnableSendCrashReport(Convert.ToSByte(MicrosoftAppCenter.EnableSendCrashReport));
                    }
                    var ret = Convert.ToBoolean(context_.server.Initialize());
                    if (ret)
                    {
                        logger_ = context_.server.GetLogger("FrontEnd");
                        iniFile_.SetLogger(logger_);
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
            }
            return false;
        }
        bool InitializeFrontEndServiceMain(string fileExtension, string currentSourceCodeDirectory)
        {
            if (! Directory.Exists(currentSourceCodeDirectory))
            {
                logger_.Error($"Directory not Found: currentSourceCodeDirectory={currentSourceCodeDirectory}");
                return false;
            }
            var serverConfigFilename = iniFile_.FindServerConfig(fileExtension);
            if (string.IsNullOrEmpty(serverConfigFilename))
            {
                return false;
            }
            if(!File.Exists(serverConfigFilename))
            {
                logger_.Error($"File not found. serverConfigFilename={serverConfigFilename}");
                return false;
            }
            lock (context_)
            {
                if (!CreateWorkerMain(serverConfigFilename, currentSourceCodeDirectory))
                {
                    return false;
                }
                UIThread.Invoke((MethodInvoker)delegate
                {
                    if (diagnosticsTask_ == null)
                    {
                        diagnosticsTask_ = new Diagnostics(context_.worker.PullDiagnosticsParams, logger_, tokenSource_.Token);
                    }
                    const sbyte False = 0;
                    if ((hoverTask_ == null) && (context_.worker.ServerCapabilities.HoverProvider != False))
                    {
                        hoverTask_ = new Hover(this.HoverAsync, context_.worker, logger_, tokenSource_.Token);
                    }
                    if (syncDocumentTask_ == null)
                    {
                        syncDocumentTask_ = new SyncDocumenmt(logger_, tokenSource_.Token);
                        syncDocumentTask_.OpenEvent += (sender, e) => context_.worker.DidOpen(e.FileName, e.Text, e.ContentsVersion);
                        syncDocumentTask_.ChangeEvent += (sender, e) =>context_.worker.DidChange(e.FileName, e.Text, e.ContentsVersion);
                        syncDocumentTask_.CloseEvent += (sender, e) =>context_.worker.DidClose(e.FileName);
                    }
                });
            }
            return true;
        }
        internal CancellationToken GetCancellationToken()
        {
            return tokenSource_.Token;
        }

#region Public methods
        public Service()
        {
            try
            {
                MicrosoftAppCenter.Start();
                tokenSource_ = new CancellationTokenSource();
                context_     = new Context();
                UIThread.Initializer();
                Api.Initialize();
            }catch(Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
        }

        public bool Initialize(string iniFileName)
        {
            //Memo: logger_はBackEndService起動後に取得可能
            try
            {
                iniFile_ = IniFileService.Create(iniFileName);
                if (iniFile_ == null)
                {
                    logger_?.Error(string.Format(".Ini file not found. iniFilename={0}", iniFileName));
                    return false;
                }

                //CrashReport送信は個人情報に関わる処理なので処理の早い段階で真偽値をセットする
                MicrosoftAppCenter.EnableSendCrashReport=iniFile_.ReadEnableCrashReport();
                iniFile_.OnFileChanged += IniFile__OnFileChanged;

                return true;
            }catch(Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            return false;
        }
        private void IniFile__OnFileChanged(object sender, EventArgs _)
        {
            try
            {
                if (iniFile_ == null)
                {
                    return;
                }
                var sendCrashReport = iniFile_.ReadEnableCrashReport();
                MicrosoftAppCenter.EnableSendCrashReport = sendCrashReport;
                context_?.server?.EnableSendCrashReport(Convert.ToSByte(sendCrashReport));
            }
            catch (System.Exception e)
            {
                logger_?.Error(e.ToString());
            }
        }

        /// <summary>
        /// BackEndを初期化する（非同期版）
        /// </summary>
        /// <param name="logFilename"></param>
        public void InitializeBackEndServiceAsync()
        {
            var _ = Task.Run(() => {
                try
                {
                    if (InitializeBackEndServiceMain())
                    {
                        return;
                    }
                    UIThread.Invoke((MethodInvoker)delegate
                    {
                        Finalizer();
                    });
                }catch(Exception e)
                {
                    OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                    logger_?.Error(e.ToString());
                }
            }, tokenSource_.Token);
        }
        /// <summary>
        /// back endの初期化が終了したか調べる
        /// </summary>
        /// <returns></returns>
        public bool CheckBackEndService()
        {
            if (tokenSource_.IsCancellationRequested)
            {
                return false;
            }

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
            }catch(Exception e){
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            finally{
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
        public bool InitializeBackEndService()
        {
            try
            {
                if (!InitializeBackEndServiceMain())
                {
                    Finalizer();
                    return false;
                }
                return true;
            }catch(Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            return false;
        }
        /// <summary>
        /// front endを初期化する（非同期版）
        /// </summary>
        /// <param name="serverConfigFilename"></param>
        /// <param name="currentSourceCodeDirectory"></param>
        public void InitializeFrontEndServiceAsync(string fileExtension, string currentSourceCodeDirectory)
        {
            var _ = Task.Run(async () =>
            {
                try
                {
                    var prevUpdateCount = iniFile_.UpdateCount;
                    while(InitializeFrontEndServiceMain(fileExtension, currentSourceCodeDirectory)==false)
                    {
                        //リトライする（.iniファイルの読み込み失敗、または、LSPのプロセス起動失敗）
                        while (true)
                        {
                            if (tokenSource_.IsCancellationRequested)
                            {
                                return;
                            }
                            var currentUpdateCount = iniFile_.UpdateCount;
                            if (prevUpdateCount != currentUpdateCount)
                            {
                                //.iniファイルが更新されたため再読み込みする
                                prevUpdateCount = currentUpdateCount;
                                break;
                            }
                            await Task.Delay(500);
                        }
                    }
                    //.iniファイル読み込み成功
                }catch(Exception e)
                {
                    OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(),e.ToString());
                    logger_?.Error(e.ToString());
                }
            }, tokenSource_.Token);
        }
        /// <summary>
        /// front endの初期化が終了したか調べる
        /// </summary>
        /// <returns></returns>
        public bool CheckFrontEndService()
        {
            if (tokenSource_.IsCancellationRequested)
            {
                return false;
            }
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
            }catch(Exception e){
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            finally{
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

            try
            {
                var success = InitializeFrontEndServiceMain(fileExtension, currentSourceCodeDirectory);
                if (!success)
                {
                    Finalizer();
                    return false;
                }
                return true;
            }catch(Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
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
                return context_.server.Add(x, y);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public void Finalizer(int reason = 0)
        {
            logger_?.Trace("Enter Finalizer");
            try
            {
                tokenSource_?.Cancel();
                lock (context_)
                {
                    FinalizeContext();
                }
                UIThread.Finalizer();
            }
            catch (Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }

            diagnosticsTask_ = null;
            hoverTask_ = null;
            syncDocumentTask_ = null;

            dasmr_ = null;

            /*tokenSource_ = null;
            context_ = null;*/

            iniFile_ = null;

            logger_?.Trace("Leave Finalizer");
            logger_ = null;

            GC.Collect();
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
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
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
            return syncDocumentTask_.SyncDocument();
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
                var absFileName=syncDocumentTask_.QueryFileName();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return "";
                }
                long line, character;
                Api.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                return context_.worker.Completion(absFileName, line, character);
            }
            catch(Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_.Error(e.ToString());
            }
            return "";
        }

#region Impl
        delegate ILocationContainer InvokeWorker(string absFilename, long line, long character);
        LocationContainerImpl CommonImplementationsOfGoto(long hidemaruLine, long hidemaruColumn, InvokeWorker invoke, bool useContentsOfLine)
        {
            try
            {
                var absFileName = syncDocumentTask_.QueryFileName();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return null;
                }
                long line, character;
                Api.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                var locations = invoke(absFileName, line, character);
                List<LocationContainerImpl.WithContent> contents;
                if (useContentsOfLine)
                {
                    contents = ConvertLocationsWidthContent(locations);
                }
                else
                {
                    contents = ConvertLocations(locations) ;
                }
                return new LocationContainerImpl(contents);
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            return null;
        }

        static List<LocationContainerImpl.WithContent> ConvertLocationsWidthContent(HidemaruLspClient_BackEndContract.ILocationContainer locations)
        {
            var result = new List<LocationContainerImpl.WithContent>();
            {
                var option = new TextLines.Option();
                for (long i = 0; i < locations.Length; ++i)
                {
                    var UserData = new LocationContainerImpl.WithContent(locations.Item(i));
                    result.Add(UserData);

                    var location = locations.Item(i);
                    if (location == null)
                    {
                        continue;
                    }
                    if (location.range == null)
                    {
                        continue;
                    }
                    option.Add(
                        new Uri(location.uri).AbsolutePath,
                        location.range.start.line,
                        UserData);
                }
                foreach(var fileContent in TextLines.Gather(option).Values)
                {
                    foreach (var lineContent in fileContent)
                    {
                        ((LocationContainerImpl.WithContent)lineContent.UserData).text= lineContent.Text;
                    }
                }
            }
            return result;
        }
        static List<LocationContainerImpl.WithContent> ConvertLocations(HidemaruLspClient_BackEndContract.ILocationContainer locations) {
            var result = new List<LocationContainerImpl.WithContent>();
            for(long i=0;i< locations.Length; ++i)
            {
                result.Add(new LocationContainerImpl.WithContent(locations.Item(i)));
            }
            return result;
        }
        #endregion


        public LocationContainerImpl Declaration(long hidemaruLine, long hidemaruColumn)
        {
            try {
                if (context_.worker == null){
                    return null;
                }
                return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, context_.worker.Declaration, false);
            }catch(Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            return null;
        }
        public LocationContainerImpl Definition(long hidemaruLine, long hidemaruColumn)
        {
            try
            {
                if (context_.worker == null)
                {
                    return null;
                }
                return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, context_.worker.Definition, false);
            }catch (Exception e){
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            return null;
        }
        public LocationContainerImpl TypeDefinition(long hidemaruLine, long hidemaruColumn)
        {
            try {
                if (context_.worker == null)
                {
                    return null;
                }
                return CommonImplementationsOfGoto(hidemaruLine,hidemaruColumn, context_.worker.TypeDefinition, false);
            }catch (Exception e){
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            return null;
        }
        public LocationContainerImpl Implementation(long hidemaruLine, long hidemaruColumn)
        {
            try {
                if (context_.worker == null)
                {
                    return null;
                }
                return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, context_.worker.Implementation, false);
            }catch (Exception e){
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            return null;
        }
        public LocationContainerImpl References(long hidemaruLine, long hidemaruColumn)
        {
            try
            {
                if (context_.worker == null)
                {
                    return null;
                }
                return CommonImplementationsOfGoto(hidemaruLine, hidemaruColumn, context_.worker.References, true);
            }catch (Exception e){
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_?.Error(e.ToString());
            }
            return null;
        }

#region Hover
        internal string HoverAsync(long hidemaruLine, long hidemaruColumn)
        {
            try {
                if (this.tokenSource_.IsCancellationRequested)
                {
                    return null;
                }
                if (context_.worker == null)
                {
                    return null;
                }
                var absFileName = syncDocumentTask_.QueryFileName();
                if (String.IsNullOrEmpty(absFileName))
                {
                    return null;
                }
                long line, character;
                Api.HidemaruToZeroBase(out line, out character, hidemaruLine, hidemaruColumn);
                var hover = context_.worker.Hover(absFileName, line, character);
                if (hover == null)
                {
                    return null;
                }
                return hover.contents.value;
            }
            catch (Exception e)
            {
                OutputPane.OutputW(Api.Hidemaru_GetCurrentWindowHandle(), e.ToString());
                logger_.Error(e.ToString());
            }
            return null;
        }
#endregion
#endregion
    }
}
