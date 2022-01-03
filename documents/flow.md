# 処理概要

秀丸マクロ全体の処理概要です、細かな部分は省略しています。


	hidemaru_lsp_client.mac
	↓
	HidemaruLspClient_BackEnd.exe をレジストリへ登録
	↓
	dll = HidemaruLspClient_FrontEnd.dllをロードする
	↓
	dll.Initialize("server_config.ini")         //iniファイルを読み込む
	↓
	dll.InitializeBackEndService()              //Ole32.CoCreateInstanceでHidemaruLspClient_BackEndを生成する
	    - server_config.iniを参照し、拡張子からconfig/xxx.csファイルのパスを取得
	    - config/xxx.csを評価しLSPサーバの情報取得
	    - LSPサーバ起動
	↓
	dll.Completion(引数は省略)                  //LSPの機能を呼ぶ



# COMのレジストリ登録について

COMを利用したプログラムにありがちな、COMの登録と登録解除を行うバッチファイルは必要ありません。

ユーザーはCOMの存在を気にすること無くマクロを利用できます。

## HidemaruLspClient_BackEnd.exe

自分自身をレジストリに登録できるため、秀丸マクロ中で以下のように実行することでレジストリへ登録しています。

	HidemaruLspClient_BackEnd.exe -mode RegServerPerUser

## HidemaruLspClient_FrontEnd.dll

秀丸マクロの***createobject命令***を利用することで自動的にレジストリへ登録されます。

以上
