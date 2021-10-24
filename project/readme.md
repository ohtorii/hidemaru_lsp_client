# はじめに

Lspマクロに必要なCOMプログラムをビルドするための説明です。

# マクロの構成

## 全体構成

LSPの主な処理はCOMサーバで記述しています。

	（処理の流れ）
	秀丸エディタ　<-> hidemaru_lsp_client.mac　<-> COMクライアント(HidemaruLspClient_FrontEnd.DLL) <-> COMサーバ(HidemaruLspClient_BackEnd.exe) <-> 各言語のLSPサーバ

### HidemaruLspClient_BackEnd.exe（COMサーバ）

このアウトプロセスサーバのインスタンスは一つです、各言語のLSPサーバとCOMクライアントを仲介します。

### HidemaruLspClient_FrontEnd.DLL（COMクライアント）

このインプロセスサーバのインスタンスは秀丸エディタで開いた **ファイル数だけ起動** します。
COMサーバと秀丸エディタを仲介します。


# ビルド手順

lsp-all.sln をVisualStudioで開󠄀き「Release AnyCPU」でビルドできます。

# ソリューションファイル

|ソリューションファイル|説明|
|:--:|:--:|
|lsp-all.sln|UnitTestなど全部入りソリューション|
|hidemaru_lsp_client.sln|秀丸マクロの動作に必要なプログラムに絞ったソリューション|

# ソリューション構成

|ターゲット|説明|
|:--:|:--:|
|AnyCPU|開発用(ターゲットは秀丸エディタ 64bit版)|
|x64|リリース用 x64版|
|x86|リリース用 x86版|

秀丸エディタがx64,x86をサポートしているため、リリース時はx64,x86を明示的に分けています。

# 各言語のLSPサーバと通信する

## モデル定義

[Language Server Protocol Specification](https://microsoft.github.io/language-server-protocol/specification) を参照してC#版のコードを記述します。</p>
**LanguageServerProtocol/Model/*.cs** を参照してください。

## COMサーバ実装

IDLでインターフェースを記述します。</p>
HidemaruLspClient_BackEndContract/BackEndContract.idl</p>
HidemaruLspClient_BackEndでインターフェースを実装します。

## COMクライアント実装

- 秀丸エディタへ公開するインターフェースを定義します。
	- HidemaruLspClient_FrontEnd\IService.cs
- インターフェースを実装します（同期版）
	- HidemaruLspClient_FrontEnd\Service.cs
- インターフェースを実装します（非同期版）
	- HidemaruLspClient_FrontEnd\ServiceAsync.cs

