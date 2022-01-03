# はじめに

本マクロの内部実装です、重要な箇所のみとりあげています。

# 概要

各プロセスとDLLの全体図です。

![概要](assets/Overview.drawio.png "概要")

### 秀丸エディタの特長

1. 秀丸エディタはファイル毎にプロセス生成しています上記図では4プロセス生成しています。VisualStudio,VSCodeのように1プロセスに纏まっていません。
2. 秀丸エディタにはプロジェクトのルートフォルダ（通常はCMS<Git,Svn...>のルートフォルダ）という概念がありません

端的に言い表すと秀丸エディタにLSPを組み込むのは難しいです、そこで、秀丸エディタと各言語のLSPをIn-Prcess Server/Out-Of-Process Serverが仲介することで実現しています。

# プロジェクトフォルダの指定方法

***hidemaru_lsp_client\config*** フォルダを参照してください。

# 各言語のLSPサーバと通信する

## モデル定義

[Language Server Protocol Specification](https://microsoft.github.io/language-server-protocol/specification) を参照してC#版のコードを記述します。</p>
**LanguageServerProtocol/Model/*.cs** を参照してください。

## HidemaruLspClient_BackEnd.exeの内部実装

IDLファイル(***HidemaruLspClient_BackEndContract/BackEndContract.idl***)でインターフェースを記述します。

IDLファイルから生成したインターフェースの実装方法は***HidemaruLspClient_BackEnd\BackEndContract***を参照します。


## HidemaruLspClient_FrontEnd.DLLの内部実装

|機能|ファイル|
|--|--|
|秀丸エディタへ公開するインターフェース|HidemaruLspClient_FrontEnd\IService.cs|
|インターフェースの実装（同期版）|HidemaruLspClient_FrontEnd\Service.cs|
|インターフェースの実装（非同期版）|HidemaruLspClient_FrontEnd\ServiceAsync.cs|

以上