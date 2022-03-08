using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace HidemaruLspClient_FrontEnd
{
    partial class MicrosoftAppCenter
    {
        public static void Start()
        {
            Crashes.NotifyUserConfirmation(UserConfirmation.Send);
            AppCenter.Start(appSecret, typeof(Analytics), typeof(Crashes));
        }
    }
}
