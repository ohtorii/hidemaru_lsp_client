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

setlocal enabledelayedexpansion

REM --------------------------------------------------------------------
REM 変数設定
REM --------------------------------------------------------------------
REM VisualStudiのバージョンに合わせてください
REM set VSDEVCMD=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat
set VSDEVCMD=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat
set REPOSITORY=https://github.com/ohtorii/hidemaru_lsp_client.git

call :SetupRootDir
REM set ROOT_DIR=C:\Users\ikeuc\AppData\Local\Temp\tmp_hm_lspclient\20211122211013_32702
set GIT_ROOT_DIR=%ROOT_DIR%\hidemaru_lsp_client
set SOLUTION=%GIT_ROOT_DIR%\project\hidemaru_lsp_client.sln

set BIN_SRC_BACKEND_X64_RELEASE=%GIT_ROOT_DIR%\project\HidemaruLspClient_BackEnd\bin\x64\Release\net5.0
set BIN_DST_BACKEND_X64_RELEASE=%GIT_ROOT_DIR%\internal\bin\HidemaruLspClient_BackEnd-x64-Release
set BIN_SRC_BACKEND_X86_RELEASE=%GIT_ROOT_DIR%\project\HidemaruLspClient_BackEnd\bin\x86\Release\net5.0
set BIN_DST_BACKEND_X86_RELEASE=%GIT_ROOT_DIR%\internal\bin\HidemaruLspClient_BackEnd-x86-Release

set BIN_SRC_FRONTEND_X64_RELEASE=%GIT_ROOT_DIR%\project\HidemaruLspClient_FrontEnd\bin\x64\Release
set BIN_DST_FRONTEND_X64_RELEASE=%GIT_ROOT_DIR%\internal\bin\HidemaruLspClient_FrontEnd-x64-Release
set BIN_SRC_FRONTEND_X86_RELEASE=%GIT_ROOT_DIR%\project\HidemaruLspClient_FrontEnd\bin\x86\Release
set BIN_DST_FRONTEND_X86_RELEASE=%GIT_ROOT_DIR%\internal\bin\HidemaruLspClient_FrontEnd-x86-Release


REM --------------------------------------------------------------------
REM メイン処理
REM --------------------------------------------------------------------
echo ROOT_DIR=%ROOT_DIR%

set ARG_VERSION=%1
set ARG_OUTDIR=%2

call :Main
if ERRORLEVEL 1 (
    echo ==============================================================
    echo エラー
    echo ==============================================================
) else (
    echo ==============================================================
    echo 成功
    echo ==============================================================
)
exit /b !ERRORLEVEL!



:Main
    REM 引数の確認
    call :CheckArguments
    if ERRORLEVEL 1 (
        exit /b 1
    )
    call "%VSDEVCMD%"
    if ERRORLEVEL 1 (
        exit /b 1
    )

    REM 処理に必要なコマンドが存在するか調べる
    call :CheckCommands
    if ERRORLEVEL 1 (
        exit /b 1
    )

    REM 作業用の一時ディレクトリを作る
    md "%ROOT_DIR%"
    if not exist "%ROOT_DIR%" (
        echo 一時ディレクトリを作成できませんでした
        exit /b 1
    )
    REM 作業ディレクトリへ移動する
    pushd "%ROOT_DIR%"

    REM メインの処理
    call :CreatePackageDirectory
    if ERRORLEVEL 1 (
        echo CreatePackageDirectory失敗
    )
    set RET=!ERRORLEVEL!
    REM 元のディレクトリに戻る
    popd
    REM 後始末
    rmdir /S /Q "%ROOT_DIR%"

    exit /b !RET!


:SetupRootDir
    setlocal
    set TEMP_TIME=%time: =0%
    set NOW=%date:/=%%TEMP_TIME:~0,2%%TEMP_TIME:~3,2%%TEMP_TIME:~6,2%
    set ROOT_DIR=%TEMP%\tmp_hm_lspclient\%NOW%_%random%
    endlocal && set ROOT_DIR=%ROOT_DIR%
    exit /b 0


REM 引数の確認
:CheckArguments
    if "%ARG_VERSION%" == "" (
        echo バージョン番号を指定してください。
        call :Usage
        exit /b 1
    )
    if "%ARG_OUTDIR%" == "" (
        echo 出力ディレクトリを指定してください。
        call :Usage
        exit /b 1
    )
    exit /b 0


REM バッチファイル中で使用するコマンドが存在するか調べる
:CheckCommands
    where git
    if ERRORLEVEL 1 (
        echo gitコマンドが見つかりません
        exit /b 1
    )

    where 7z
    if ERRORLEVEL 1 (
        echo 7zコマンドが見つかりません
        exit /b 1
    )
    
    where dotnet
    if ERRORLEVEL 1 (
        echo dotnetコマンドが見つかりません
        exit /b 1
    )
    
    where MSBuild
    if ERRORLEVEL 1 (
        echo MSBuildコマンドが見つかりません
        exit /b 1
    )
    exit /b 0



:CreatePackageDirectory
    git clone --recursive --depth 1 "%REPOSITORY%"
    if ERRORLEVEL 1 (
        echo gitコマンドが失敗しました。
        exit /b 1
    )

    dotnet restore "%SOLUTION%"
    if ERRORLEVEL 1 (
        echo dotnet restore失敗
        exit /b 1
    )
    
    MSBuild "%SOLUTION%" -maxcpucount:%NUMBER_OF_PROCESSORS% /t:clean;rebuild /p:Configuration=Release;Platform="x64"
    if ERRORLEVEL 1 (
        echo ビルド失敗:x64
        exit /b 1
    )
    
    MSBuild "%SOLUTION%" -maxcpucount:%NUMBER_OF_PROCESSORS% /t:clean;rebuild /p:Configuration=Release;Platform="x86"
    if ERRORLEVEL 1 (
        echo ビルド失敗:x86
        exit /b 1
    )
   
    del /Q "%BIN_SRC_BACKEND_X64_RELEASE%\*.pdb"
    del /Q "%BIN_SRC_BACKEND_X86_RELEASE%\*.pdb"
    del /Q "%BIN_SRC_FRONTEND_X64_RELEASE%\*.pdb"
    del /Q "%BIN_SRC_FRONTEND_X86_RELEASE%\*.pdb"
    
    xcopy "%BIN_SRC_BACKEND_X64_RELEASE%"  "%BIN_DST_BACKEND_X64_RELEASE%"  /Y /I /E
    xcopy "%BIN_SRC_BACKEND_X86_RELEASE%"  "%BIN_DST_BACKEND_X86_RELEASE%"  /Y /I /E
    xcopy "%BIN_SRC_FRONTEND_X64_RELEASE%" "%BIN_DST_FRONTEND_X64_RELEASE%" /Y /I /E
    xcopy "%BIN_SRC_FRONTEND_X86_RELEASE%" "%BIN_DST_FRONTEND_X86_RELEASE%" /Y /I /E
    
    call :DeleteUnnecessaryFiles
    if ERRORLEVEL 1 (
        echo 失敗 DeleteUnnecessaryFiles
        exit /b 1
    )
    

    REM zipで固める
    md "%ARG_OUTDIR%"
    if not exist "%ARG_OUTDIR%" (
        echo 出力ディレクトリを作成できませんでした
        exit /b 1
    )

    7z.exe a -mx9 -mmt%NUMBER_OF_PROCESSORS% "%ARG_OUTDIR%\hidemaru_lsp_client-%ARG_VERSION%.zip" "%GIT_ROOT_DIR%\"
    if not exist  "%ARG_OUTDIR%\hidemaru_lsp_client-%ARG_VERSION%.zip" (
        echo 7zコマンドが失敗しました
        exit /b 1
    )

    exit /b 0



REM 不要なファイルを削除する
:DeleteUnnecessaryFiles
    rmdir /S /Q  "%GIT_ROOT_DIR%\.git"
    if exist     "%GIT_ROOT_DIR%\.git" (
        echo .git ディレクトリの削除に失敗しました。
        exit /b 1
    )

    rmdir /S /Q  "%GIT_ROOT_DIR%\tools"
    if exist     "%GIT_ROOT_DIR%\tools" (
        echo tools ディレクトリの削除に失敗しました。
        exit /b 1
    )

    rmdir /S /Q  "%GIT_ROOT_DIR%\project"
    if exist     "%GIT_ROOT_DIR%\project" (
        echo project ディレクトリの削除に失敗しました。
        exit /b 1
    )
    
    del "%GIT_ROOT_DIR%\.gitattributes"
    del "%GIT_ROOT_DIR%\.gitignore"
    exit /b 0


:Usage
    echo;
    echo create_package.bat : Coopyright (c) 2020 ohtorii
    echo;
    echo ＊使い方
    echo create_package.bat バージョン番号 出力先ディレクトリ名
    echo;
    echo ＊使用例
    echo create_package.bat 1.2.3 c:\project\package
    exit /b 0
