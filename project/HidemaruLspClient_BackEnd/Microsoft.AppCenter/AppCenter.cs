using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace HidemaruLspClient
{
    partial class MicrosoftAppCenter
    {
        public static void Start()
        {
            if (string.IsNullOrEmpty(appSecret))
            {
                return;
            }
            Crashes.NotifyUserConfirmation(UserConfirmation.Send);
            AppCenter.Start(appSecret, typeof(Analytics), typeof(Crashes));

#if false
            //debug
            Crashes.GenerateTestCrash();
#endif
        }
    }
}

