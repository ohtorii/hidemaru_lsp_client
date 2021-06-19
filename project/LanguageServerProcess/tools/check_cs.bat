@echo off
setlocal
if "%1"=="" (
    echo 入力ファイルを指定してください
    pause
    exit /b 1
)
set INFILE=%1
set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe 
set OUT=%TEMP%\hidemaru_lsp_check_cs_TEMP.dll

"%CSC%" /out:%OUT% /target:library /reference:%~dp0/../bin/Debug/LanguageServerProcess.dll "%INFILE%"
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