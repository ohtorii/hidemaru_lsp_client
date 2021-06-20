using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class Config
    {
        public static string logFileName { get { return Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\hidemaru_lsp_client.log"); } }
        public static string tempDirectoryName { get { return Environment.ExpandEnvironmentVariables(@"%TEMP%\hidemaru_lsp_client"); } }
    }
}
