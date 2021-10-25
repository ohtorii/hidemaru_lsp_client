@echo off
setlocal
REM 
REM ディレクトリを空にする
REM ClearDir.bat TargetDir
REM 

set TargetDir=%~1
rmdir /S /Q "%TargetDir%"
if exist "%TargetDir%" (
    echo [Error] ターゲットディレクトリを削除できませんでした
    echo [Error] ディレクトリ名 %TargetDir%
    exit /b 1
)
mkdir "%TargetDir%"
if not exist "%TargetDir%" (
    echo [Error] ディレクトリを作成できませんでした
    echo [Error] ディレクトリ名 %TargetDir%
    exit /b 1
)

exit /b 0
