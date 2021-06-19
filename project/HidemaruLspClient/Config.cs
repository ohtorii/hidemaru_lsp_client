using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class Config
    {
        static public string logFileName { get { return Environment.ExpandEnvironmentVariables(@"%HOMEDRIVE%%HOMEPATH%\hidemaru_lsp_client.log"); } }
    }
}
