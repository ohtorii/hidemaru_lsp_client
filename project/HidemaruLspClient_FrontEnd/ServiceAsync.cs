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
    public class ServiceAsync:IService
    {
        Service service_;
        Timer timer_;
        public ServiceAsync()
        {
            timer_ = new Timer();
            timer_.Interval = 100;
            timer_.Tick += MainLoop;
            timer_.Start();
        }
        private void MainLoop(object sender, EventArgs e)
        {
            service_ = new Service();
        }

        #region Public Interface
        public int Add(int x, int y)
        {
            throw new NotImplementedException();
        }

        public string Completion(long hidemaruLine, long hidemaruColumn)
        {
            throw new NotImplementedException();
        }

        public LocationContainerImpl Declaration(long hidemaruLine, long hidemaruColumn)
        {
            throw new NotImplementedException();
        }

        public LocationContainerImpl Definition(long hidemaruLine, long hidemaruColumn)
        {
            throw new NotImplementedException();
        }

        public void Finalizer(int reason = 0)
        {
            throw new NotImplementedException();
        }

        public LocationContainerImpl Implementation(long hidemaruLine, long hidemaruColumn)
        {
            throw new NotImplementedException();
        }

        public LocationContainerImpl References(long hidemaruLine, long hidemaruColumn)
        {
            throw new NotImplementedException();
        }

        public ServerCapabilitiesImpl ServerCapabilities()
        {
            throw new NotImplementedException();
        }

        public bool SyncDocument()
        {
            throw new NotImplementedException();
        }

        public LocationContainerImpl TypeDefinition(long hidemaruLine, long hidemaruColumn)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
