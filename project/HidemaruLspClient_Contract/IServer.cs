using System;
using System.Runtime.InteropServices;

/*Memo: メンバの並び順はIDL定義ファイルと合わせること。
 * 並ぶ順を合わせないとアプリがクラッシュする。
 */
[ComVisible(true)]
[Guid(LspContract.Constants.ServerInterface)]
public interface IHidemaruLspBackEndServer
{
    bool Initialize(string logFileName);
    int Add(int x, int y);

    void Finalizer(int reason);
    bool Start(string ExcutablePath,
                string Arguments,
                string RootUri,
                string WorkspaceConfig,
                string currentSourceCodeDirectory);     
    string Completion(string absFilename, long line, long column, string text);
}

