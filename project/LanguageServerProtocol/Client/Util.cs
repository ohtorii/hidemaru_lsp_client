using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Client
{
    class Util
    {
        /// <summary>
        /// URIを正規化する        
        /// (Ex.)
        /// Before:file:///c%3A/Users/foo/GitHub/hidemaru_lsp_client/project/TestData/lua/test1.lua
        /// After :file:///c:/Users/foo/GitHub/hidemaru_lsp_client/project/Testdata/lua/test1.lua			
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static public string NormalizeUri(string uri)
        {
            return Uri.UnescapeDataString(uri);
        }
    }
}
