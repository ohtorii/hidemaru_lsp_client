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
[Guid("47137F87-E07A-422E-9FB8-F0B13C615D2F")]
public interface ITargetServer
{
    string ServerName { get; set; }
    string RootUri { get; set; }
    void Initialize();
}

[ComVisible(true)]
[Guid(LspContract.Constants.ServerInterface)]
public interface IHidemaruLspBackEndServer
{
    ITargetServer CreateTargetServer();
    bool Initialize(string logFileName);
    ILspClientLogger GetLogger();
    int Add(int x, int y);

    void Finalizer(ITargetServer TargetServer, int reason);
    bool Start(
        ITargetServer TargetServer,
        string ExcutablePath,
        string Arguments,
        string WorkspaceConfig,
        string currentSourceCodeDirectory);
    void DigOpen(ITargetServer TargetServer,string filename, string text, int contentsVersion);
    void DigChange(ITargetServer TargetServer, string filename, string text, int contentsVersion);
    string Completion(ITargetServer TargetServer, string absFilename, long line, long column);
}

