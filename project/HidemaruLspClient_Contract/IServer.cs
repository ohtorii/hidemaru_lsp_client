using System;
using System.Runtime.InteropServices;

/*Memo: メンバの並び順はIDL定義ファイルと合わせること。
 * 並ぶ順を合わせないとアプリがクラッシュする。
 */

[ComVisible(true)]
[Guid("318C52CD-4A12-4C98-892B-1830B026768D")]
public interface ILspClientLogger
{
    bool IsFatalEnabled { get; }
    bool IsErrorEnabled { get; }
    bool IsWarnEnabled { get; }
    bool IsInfoEnabled { get; }
    bool IsDebugEnabled { get; }
    bool IsTraceEnabled { get; }
    void Trace(string message);
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message);
    void Fatal(string message);
}

[ComVisible(true)]
[Guid("E8153825-63C2-4C50-9889-0DEB4CFB4033")]
public interface IWorker
{
    void DigOpen(string filename, string text, int contentsVersion);
    void DigChange(string filename, string text, int contentsVersion);
    string Completion(string absFilename, long line, long column);
}

[ComVisible(true)]
[Guid(LspContract.Constants.ServerInterface)]
public interface IHidemaruLspBackEndServer
{    
    bool Initialize(string logFileName);
    ILspClientLogger GetLogger();
    int Add(int x, int y);

    //void Finalizer(ITargetServer TargetServer, int reason);
    IWorker CreateWorker(
                string ServerName,
                string ExcutablePath,
                string Arguments,
                string RootUri,
                string WorkspaceConfig);
    void DestroyWorker(IWorker worker);
}

