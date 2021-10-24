@echo off
REM
REM パッケージを作成するバッチファイル
REM
REM ＊使い方
REM create_package.bat バージョン番号 出力先ディレクトリ名
REM
REM ＊使用例
REM create_package.bat 1.2.3 c:\project\package
REM

setlocal

REM --------------------------------------------------------------------
REM 変数設定
REM --------------------------------------------------------------------
REM VisualStudiのバージョンに合わせてください
REM set VSDEVCMD=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat
set VSDEVCMD=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat
set REPOSITORY=https://github.com/ohtorii/hidemaru_lsp_client.git

REM 一時ディレクトリ(
set TEMP_TIME=%time: =0%
set NOW=%date:/=%%TEMP_TIME:~0,2%%TEMP_TIME:~3,2%%TEMP_TIME:~6,2%
set ROOT_DIR=%TEMP%\tmp_hm_lspclient\%NOW%_%random%
set GIT_ROOT_DIR=%ROOT_DIR%\hidemaru_lsp_client

REM ソリューションファイル
set SOLUTION=%GIT_ROOT_DIR%\project\hidemaru_lsp_client.sln


REM --------------------------------------------------------------------
REM メイン処理
REM --------------------------------------------------------------------
set RESULT=0
echo ROOT_DIR=%ROOT_DIR%

set ARG_VERSION=%1
set ARG_OUTDIR=%2

call :Main
if "%RESULT%" NEQ "0" (
    echo ==============================================================
    echo エラー
    echo ==============================================================
) else (
    echo ==============================================================
    echo 成功
    echo ==============================================================
)
exit /b %RESULT%



:Main
    call "%VSDEVCMD%"
    if "%errorlevel%" NEQ "0" (
        set RESULT=1
        exit /b 1
    )
    
    REM 引数の確認
    call :CheckArguments
    if "%errorlevel%" NEQ "0" (
        set RESULT=1
        exit /b 1
    )

    REM 処理に必要なコマンドが存在するか調べる
    call :CheckCommand
    if "%errorlevel%" NEQ "0" (
        set RESULT=1
        exit /b 1
    )

    REM 作業用の一時ディレクトリを作る
    md "%ROOT_DIR%"
    if not exist "%ROOT_DIR%" (
        echo 一時ディレクトリを作成できませんでした
        set RESULT=1
        exit /b 1
    )
    REM 作業ディレクトリへ移動する
    pushd "%ROOT_DIR%"
    if "%errorlevel%" NEQ "0" (
        echo pushdコマンドが失敗しました
        set RESULT=1
        exit /b 1
    )

    REM メインの処理
    call :Core

    REM 元のディレクトリに戻る
    popd
    if "%errorlevel%" NEQ "0" (
        echo popdコマンドが失敗しました
        set RESULT=1
        exit /b 1
    )

    REM 後始末
    rmdir /S /Q "%ROOT_DIR%"

    exit /b %errorlevel%


REM 引数の確認
:CheckArguments
    if "%ARG_VERSION%" == "" (
        echo バージョン番号を指定してください。
        call :Usage
        set RESULT=1
        exit /b 1
    )
    if "%ARG_OUTDIR%" == "" (
        echo 出力ディレクトリを指定してください。
        call :Usage
        set RESULT=1
        exit /b 1
    )
    exit /b 0


REM バッチファイル中で使用するコマンドが存在するか調べる
:CheckCommand
    where git
    if "%errorlevel%" NEQ "0" (
        echo gitコマンドが見つかりません
        set RESULT=1
        exit /b 1
    )

    where 7z
    if "%errorlevel%" NEQ "0" (
        echo 7zコマンドが見つかりません
        set RESULT=1
        exit /b 1
    )
    where MSBuild
    if "%errorlevel%" NEQ "0" (
        echo MSBuildコマンドが見つかりません
        set RESULT=1
        exit /b 1
    )
    exit /b 0



:Core
    git clone --recursive --depth 1 "%REPOSITORY%"
    if not exist "%GIT_ROOT_DIR%" (
        echo gitコマンドが失敗しました。
        set RESULT=1
        exit /b 1
    )
    
    MSBuild "%SOLUTION%" /t:clean;rebuild /p:Configuration=Release;Platform="Any CPU"
    if %ERRORLEVEL% neq 0 (
        echo ErrorLevel:%ERRORLEVEL%
        echo ビルド失敗
        exit /b 1
    )
    exit /b 0


    REM call :DeleteUnnecessaryFiles
    REM if "%errorlevel%" NEQ "0" (
        REM echo aaaaa
        REM set RESULT=1
        REM exit /b 1
    REM )
    
    
    echo zip で固める処理はスキップ
    exit /b 1
    
    
    REM zipで固める
    md "%ARG_OUTDIR%"
    if not exist "%ARG_OUTDIR%" (
        echo 出力ディレクトリを作成できませんでした
        set RESULT=1
        exit /b 1
    )
    
    7z.exe a -mx9 -mmt%NUMBER_OF_PROCESSORS% "%ARG_OUTDIR%\unity-%ARG_VERSION%.zip" "%GIT_ROOT_DIR%\"
    if not exist  "%ARG_OUTDIR%\unity-%ARG_VERSION%.zip" (
        echo 7zコマンドが失敗しました
        set RESULT=1
        exit /b 1
    )
    
    exit /b 0



REM 不要なファイルを削除する
:DeleteUnnecessaryFiles
    exit /b 0
    
    rmdir /S /Q  "%GIT_ROOT_DIR%\.git"
    if exist     "%GIT_ROOT_DIR%\.git" (
        echo .git ディレクトリの削除に失敗しました。
        set RESULT=1
        exit /b 1
    )

    rmdir /S /Q  "%GIT_ROOT_DIR%\tools"
    if exist     "%GIT_ROOT_DIR%\tools" (
        echo tools ディレクトリの削除に失敗しました。
        set RESULT=1
        exit /b 1
    )

    rmdir /S /Q  "%GIT_ROOT_DIR%\help\images\resources"
    if exist     "%GIT_ROOT_DIR%\help\images\resources" (
        echo help\images\resources ディレクトリの削除に失敗しました。
        set RESULT=1
        exit /b 1
    )

    rmdir /S /Q  "%GIT_ROOT_DIR%\internal\dll_project"
    if exist     "%GIT_ROOT_DIR%\internal\dll_project" (
        echo internal\dll_project ディレクトリの削除に失敗しました。
        set RESULT=1
        exit /b 1
    )

    exit /b 0


:Usage
    echo;
    echo create_package.bat : Coopyright (c) 2020 ohtorii
    echo;
    echo ＊使い方
    echo create_package.bat バージョン番号 出力先ディレクトリ名
    echo
    echo ＊使用例
    echo create_package.bat 1.2.3 c:\project\package
    exit /b 0
