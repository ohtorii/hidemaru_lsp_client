@echo off
setlocal
if "%1"=="" (
    echo ���̓t�@�C�����w�肵�Ă�������
    pause
    exit /b 1
)
set INFILE=%1
set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe 
set OUT=%TEMP%\hidemaru_lsp_check_cs_TEMP.dll

"%CSC%" /out:%OUT% /target:library /reference:%~dp0/../bin/Debug/LanguageServerProcess.dll "%INFILE%"
if "%ERRORLEVEL%"=="0" (
    echo =====================================
	echo ���@�G���[�͌�����܂���ł���
    echo =====================================
) else (
    echo =====================================
	echo ���s
    echo =====================================
)
del %OUT% > nul 2>&1
pause