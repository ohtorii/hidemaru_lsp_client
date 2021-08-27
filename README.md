# hidemaru_lsp_client

秀丸エディタから Language Server Protocol (LSP) を利用するマクロ。

## スケジュール感

- 2021年　機能を作りきる
- 2022年　自分の環境で利用し実運用上の問題点を改修する。
- 2023年　version 1.0.0 をリリース

地道にのんびりやってます～

# 予めのお断りとお願い

このマクロを使用してみようと思う方は既にある程度のスキルを持っておられるものと推測し、このドキュメントもそのような方が読まれることを前提として書いてあります。 

# できること

|機能|LSP method|実装|
|--|--|:--:|
|補完|textDocument/completion|○|
|Lint|textDocument/publishDiagnostics|○|
|宣言へ移動|textDocument/declaration|○|
|定義へ移動|textDocument/definition|○|
|カーソル位置にあるシンボルの型定義へジャンプする|textDocument/typeDefinition|○|
|カーソル位置のシンボルを参照している箇所へジャンプ|textDocument/references|○|
|実装へ移動|textDocument/implementation|○|
|Hover|textDocument/hover|○|
||||


## 動作確認した言語サーバ

- clangd
- OmniSharp
- sumneko-lua-language-server
- pyls
- pyls-ms
- Microsoft.Python.LanguageServer
- jedi-language-server
- vim-language-server

# 言語サーバ(LSP)の導入方法

C#,C++,Lua,Python...の各言語サーバ(LSP)は各自で導入してください。

# マクロの導入方法

## カスタマイズ

# 謝辞

- coc.nvim
- LanguageClient-neovim
- vim-lsc
- vim-lsp
- neovim
- vim-lsp-settings

ソースコードをかなり参考にさせて頂きました、ありがとうございます😄

# 連絡先

- <https://ohtorii.hatenadiary.jp> 
- <https://twitter.com/ohtorii> 
- <https://github.com/ohtorii>


後で書く