@echo off
setlocal
if "%1"=="" (
    echo 入力ファイルを指定してください
    pause
    exit /b 1
)
set INFILE=%1
set OUT=%TEMP%\hidemaru_lsp_check_cs_TEMP.dll

REM "%CSC%"  /target:library /reference:%~dp0/../../../internal/bin/HidemaruLspClient_FrontEnd/LanguageServerProcess.dll "%INFILE%"


dotnet "C:\Program Files\dotnet\sdk\5.0.214\Roslyn\bincore\csc.dll" -r:"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.17\System.Private.CoreLib.dll" -r:"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.17\System.IO.dll" -r:"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.17\System.Runtime.dll" -r:"%~dp0/../../../project\LanguageServerProcess\bin\x64\Release\net5.0-windows\LanguageServerProcess.dll" /out:"%OUT%" /target:library "%INFILE%"

if "%ERRORLEVEL%"=="0" (
    echo =====================================
	echo 文法エラーは見つかりませんでした
    echo =====================================
) else (
    echo =====================================
	echo 失敗
    echo =====================================
)
del %OUT% > nul 2>&1
pause