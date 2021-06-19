using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerProcess
{
    /// <summary>
    /// LSPのサーバ情報
    /// </summary>
    public abstract class Configuration
    {
        /// <summary>
        /// 実行ファイル名を取得する
        /// </summary>
        /// <returns></returns>
        public abstract string GetExcutablePath();
        /// <summary>
        /// 引数を取得する
        /// </summary>
        /// <returns></returns>
        public virtual string GetArguments() { return ""; }
        /// <summary>
        /// rootUriを取得する
        /// </summary>
        /// <returns></returns>
        public virtual string GetRootUri() { return ""; }
        /// <summary>
        /// workspaceConfigを取得する
        /// </summary>
        /// <returns></returns>
        public virtual string GetWorkspaceConfig() { return ""; }
    }
}
