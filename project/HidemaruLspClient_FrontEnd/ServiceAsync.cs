using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HidemaruLspClient_FrontEnd
{
    /// <summary>
    /// 秀丸エディタへ公開するクラス（非同期版）
    /// </summary>
    [ComVisible(true)]
    [Guid("d46f5c09-4b16-456c-b7c7-9ee172234251")]
    sealed class ServiceAsync /*:IService*/
    {
#if false
        Service service_=null;
        Timer timer_ = null;
        string fileType_ = "";
        string logFilename_="";
        string sourceCodeDirectory_ = "";

        enum InitializeStatus
        {
            InitializeBackend,
            CheckBackend,
            InitializeFrontEnd,
            CheckFrontEnd,
            Done,
        };
        InitializeStatus initializeStatus_=InitializeStatus.InitializeBackend;


        public ServiceAsync()
        {
            service_ = new Service();
        }
        private void MainLoop(object sender, EventArgs e)
        {
            switch (initializeStatus_)
            {
                case InitializeStatus.InitializeBackend:
                    service_.InitializeBackEndServiceAsync(logFilename_);
                    initializeStatus_++;
                    goto case InitializeStatus.CheckBackend;

                case InitializeStatus.CheckBackend:
                    if (service_.CheckBackEndService()==false)
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
        public bool Initialize(string iniFileName)
        {
            return service_.Initialize(iniFileName);
        }
        public void SetFileType(string fileType)
        {
            fileType_ = fileType;
        }
        public void SetSourceCodeDirectory(string sourceCodeDirectory)
        {
            sourceCodeDirectory_ = sourceCodeDirectory;
        }
        public void InitializeServiceAsync(string logFilename, string fileExtension, string currentSourceCodeDirectory)
        {
            if (timer_ == null)
            {
                timer_ = new Timer();
                timer_.Interval = 200;
                timer_.Tick += MainLoop;
                timer_.Start();
            }
            logFilename_ = logFilename;
            fileType_ = fileExtension;
            sourceCodeDirectory_ = currentSourceCodeDirectory;
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
            timer_.Stop();
            service_.Finalizer(reason);

            timer_ = null;
            service_ = null;
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

        public ServerCapabilitiesImpl ServerCapabilities()
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
#endif
    }
}
