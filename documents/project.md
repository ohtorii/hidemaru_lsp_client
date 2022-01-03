# projectフォルダについて

VisualStudioのソリューションとプロジェクトファイルがあります。

# フォルダ位置

\hidemaru_lsp_client\project

# ディレクトリ構成

|フォルダ名|説明|備考|
|--|--|--|
|ClientExample|LanguageServerProtocolのサンプル||
|Document|ドキュメント||
|HidemaruLspClient_BackEnd|***HidemaruLspClient_BackEnd.exe***を生成するプロジェクト|COM(Out-Of-Process server)|
|HidemaruLspClient_BackEndContract|COMで利用するIDL定義||
|HidemaruLspClient_Contract|全プロジェクトから参照するGUIDなどを定義||
|HidemaruLspClient_FrontEnd|***HidemaruLspClient_FrontEnd.dll***を生成するプロジェクト|COM(In-Process server)|
|LanguageServerProcess|***LanguageServerProcess.dll***を生成するプロジェクト||
|LanguageServerProtocol|LSPのC#実装|https://microsoft.github.io/language-server-protocol/specification|
|TestData|ClientExample,UnitTestで利用するデータを置くフォルダ||
|TestResults|単体テスト時に自動生成されるフォルダ||
|Tools|ビルド時に利用するツールを置くフォルダ||
|UnitTest|単体テスト||


# ビルド方法について

|ソリューション名|説明|
|--|--|
|lsp-all.sln|全プロジェクト込みのソリューション|
|hidemaru_lsp_client.sln|単体テスト、サンプルなどを除外したソリューション|


## ソリューション
以下ソリューションが有効です。

- Release
- Debug

## プラットフォーム

以下プラットフォームが有効です。
- x64
- x86

なお、以下プラットフォームの動作は未検証です。
- Any CPU

Any CPUで全プラットフォーム対応できそうですが、COMを扱う都合上x86,x64を明示的に分けています。

# ビルド確認した環境

Microsoft Visual Studio Community 2019
Version 16.11.6

以上