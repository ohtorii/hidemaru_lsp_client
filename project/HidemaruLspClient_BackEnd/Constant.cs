using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class Constant
    {
        public static string tempDirectoryName { get { return Environment.ExpandEnvironmentVariables(@"%TEMP%\hidemaru_lsp_client"); } }
        public class Logger
        {
            public const string HeaderMain="BackEnd.Main";
            public const string HeaderClient="BackEnd.Client";
        }
    }
}
