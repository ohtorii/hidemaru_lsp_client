using System;

namespace HidemaruLspClient
{
    class Constant
    {
        public static string tempDirectoryName { get { return Environment.ExpandEnvironmentVariables(@"%TEMP%\hidemaru_lsp_client"); } }
        public class Logger
        {
            public const string HeaderMain = "BackEnd.Main";
            public const string HeaderClient = "BackEnd.Client";
        }
    }
}
