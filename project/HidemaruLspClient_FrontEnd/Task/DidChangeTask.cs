using HidemaruLspClient_BackEndContract;
using System;
using System.Threading;

namespace HidemaruLspClient_FrontEnd
{
    class DidChangeTask
    {
        Service service_;
        ILspClientLogger logger_;
        CancellationToken cancellationToken_;
        System.Windows.Forms.Timer timer_;

        public DidChangeTask(Service service, ILspClientLogger logger, CancellationToken cancellationToken)
        {
            service_ = service;
            logger_ = logger;
            cancellationToken_ = cancellationToken;

            timer_ = new System.Windows.Forms.Timer();
            timer_.Interval = 500;
            timer_.Tick += MainLoop;
            timer_.Start();
        }

        void MainLoop(object sender, EventArgs e)
        {
            try
            {
                if (cancellationToken_.IsCancellationRequested)
                {
                    return;
                }
                service_.SyncDocument();
            }
            catch (Exception exce)
            {
                logger_.Error(exce.ToString());
                throw;
            }
        }
    }

}
