namespace LanguageServerProcess
{
    /// <summary>
    /// LSPのサーバ情報
    /// </summary>
    public abstract class Configuration
    {
        /// <summary>
        /// サーバ名を取得する
        /// </summary>
        /// <returns></returns>
        public abstract string GetServerName();
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
        public virtual string GetRootUri() { return LanguageServerProcess.Environment.FindRootUriAsAdhoc(); }
        /// <summary>
        /// workspaceConfigを取得する.
        /// サーバからの問い合わせ（method: ‘workspace/configuration’）に対する応答。
        /// </summary>
        /// <returns></returns>
        public virtual string GetWorkspaceConfig() { return ""; }
    }
}
