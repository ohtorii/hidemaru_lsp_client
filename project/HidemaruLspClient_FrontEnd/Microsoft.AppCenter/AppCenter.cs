using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{
    partial class MicrosoftAppCenter
    {
        /// <summary>
        /// CrashReport送信の有効化/無効化
        /// </summary>
        public static bool EnableSendCrashReport { get; set;}=false;

        public static void Start()
        {
            if (string.IsNullOrEmpty(appSecret))
            {
                return;
            }
            Crashes.ShouldAwaitUserConfirmation = () =>
            {
                if (EnableSendCrashReport)
                {
                    Crashes.NotifyUserConfirmation(UserConfirmation.Send);
                }
                else
                {
                    Crashes.NotifyUserConfirmation(UserConfirmation.DontSend);
                }
                return true;
            };
            Crashes.NotifyUserConfirmation(UserConfirmation.Send);
            AppCenter.Start(appSecret, typeof(Analytics), typeof(Crashes));
        }
    }
}
