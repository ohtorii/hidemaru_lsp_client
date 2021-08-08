#if false 
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

#region IPublishDiagnosticsParams
[ComVisible(true)]
[Guid("53A84500-EAF3-4248-B167-D9921EE8AB51")]
public interface IPosition
{
    long character { get; }
    long line { get;  }
}
[ComVisible(true)]
[Guid("D3E64D92-3ABB-494B-A26F-A856DFE54D54")]
public interface IRange
{
    IPosition start { get;}
    IPosition end { get; }
}

[ComVisible(true)]
[Guid("5DC500BE-636F-4886-8FF5-6359D46B3342")]
public interface IDiagnostic
{
    IRange range { get; }
}
[ComVisible(true)]
[Guid("C5E68A27-29AD-4D35-8E12-7C5712D781D3")]
public interface IPublishDiagnosticsParams
{
    string uri { get; }
    long version { get; }
    long getDiagnosticsLength();
    IDiagnostic getDiagnostics(long index);
}
#endregion

[ComVisible(true)]
[Guid("E8153825-63C2-4C50-9889-0DEB4CFB4033")]
public interface IWorker
{
    void DigOpen(string absFilename, string text, int contentsVersion);
    void DigChange(string absFilename, string text, int contentsVersion);
    /// <summary>
    /// 補完
    /// </summary>
    /// <param name="absFilename"></param>
    /// <param name="line"></param>
    /// <param name="column"></param>
    /// <returns>単語一覧のファイル名</returns>
    string Completion(string absFilename, long line, long column);
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    long GetDiagnosticsParamsLength();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IPublishDiagnosticsParams GetDiagnosticsParams(long index);
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

#endif