using HidemaruLspClient_FrontEnd.BackEndContractImpl;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace HidemaruLspClient_FrontEnd
{
    /// <summary>
    /// 秀丸エディタへ公開するクラス（非同期版）
    /// </summary>
    [ComVisible(true)]
    [Guid("0B0A4550-4B16-456C-B7C7-9EE172234251")]
    public sealed class ServiceAsync : IService
    {

        Service service_ = null;
        Timer timer_ = null;
        string fileType_ = "";
        string sourceCodeDirectory_ = "";
        CancellationToken cancellationToken_;

        enum InitializeStatus
        {
            InitializeBackend,
            CheckBackend,
            InitializeFrontEnd,
            CheckFrontEnd,
            Done,
        };
        InitializeStatus initializeStatus_ = InitializeStatus.InitializeBackend;

        private void MainLoop(object sender, EventArgs e)
        {
            if (cancellationToken_.IsCancellationRequested)
            {
                if (timer_ != null) {
                    timer_.Stop();
                }
                return;
            }
            switch (initializeStatus_)
            {
                case InitializeStatus.InitializeBackend:
                    service_.InitializeBackEndServiceAsync();
                    initializeStatus_++;
                    goto case InitializeStatus.CheckBackend;

                case InitializeStatus.CheckBackend:
                    if (service_.CheckBackEndService() == false)
                    {
                        return;
                    }
                    initializeStatus_++;
                    goto case InitializeStatus.InitializeFrontEnd;

                case InitializeStatus.InitializeFrontEnd:
                    service_.InitializeFrontEndServiceAsync(fileType_, sourceCodeDirectory_);
                    initializeStatus_++;
                    goto case InitializeStatus.CheckFrontEnd;

                case InitializeStatus.CheckFrontEnd:
                    if (service_.CheckFrontEndService() == false)
                    {
                        return;
                    }
                    initializeStatus_++;
                    goto case InitializeStatus.Done;

                case InitializeStatus.Done:
                    return;
            }
        }

        #region Public Interface
        /*Memo:コンストラクタを記述するとCOM生成に失敗委する
        public ServiceASync()
        {            
        }
        */
        public bool Initialize()
        {
            try
            {
                if (service_ == null)
                {
                    service_ = new Service();
                    if (!service_.Initialize())
                    {
                        service_ = null;
                        return false;
                    }
                } 
                if (cancellationToken_ == CancellationToken.None)
                {
                    cancellationToken_ = service_.GetCancellationToken();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InitializeIni(string iniFileName) { 
            return service_.InitializeIni(iniFileName);
        }
        public bool InitializeServiceAsync(string fileExtension, string currentSourceCodeDirectory)
        {
            try
            {
                fileType_ = fileExtension;
                sourceCodeDirectory_ = currentSourceCodeDirectory;

                if (timer_ == null)
                {
                    timer_ = new Timer();
                }
                timer_.Interval = 200;
                timer_.Tick += MainLoop;
                timer_.Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckService()
        {
            if (initializeStatus_ == InitializeStatus.Done)
            {
                return true;
            }
            return false;
        }
        public void Finalizer(int reason = 0)
        {
            if (timer_ != null)
            {
                timer_.Stop();
                timer_ = null;
            }
            if (service_ != null)
            {
                service_.Finalizer(reason);
                service_ = null;
            }
        }

        public int Add(int x, int y)
        {
            return service_.Add(x, y);
        }

        public string Completion(long hidemaruLine, long hidemaruColumn)
        {
            return service_.Completion(hidemaruLine, hidemaruColumn);
        }

        public LocationContainerImpl Declaration(long hidemaruLine, long hidemaruColumn)
        {
            return service_.Declaration(hidemaruLine, hidemaruColumn);
        }

        public LocationContainerImpl Definition(long hidemaruLine, long hidemaruColumn)
        {
            return service_.Definition(hidemaruLine, hidemaruColumn);
        }
        public LocationContainerImpl Implementation(long hidemaruLine, long hidemaruColumn)
        {
            return service_.Implementation(hidemaruLine, hidemaruColumn);
        }

        public LocationContainerImpl References(long hidemaruLine, long hidemaruColumn)
        {
            return service_.References(hidemaruLine, hidemaruColumn);
        }

        public HidemaruLspClient_BackEndContract.IServerCapabilities ServerCapabilities()
        {
            return service_.ServerCapabilities();
        }

        public bool SyncDocument()
        {
            return service_.SyncDocument();
        }

        public LocationContainerImpl TypeDefinition(long hidemaruLine, long hidemaruColumn)
        {
            return service_.TypeDefinition(hidemaruLine, hidemaruColumn);
        }

        #endregion
    }
}
