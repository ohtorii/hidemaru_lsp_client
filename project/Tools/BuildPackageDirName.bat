@echo off
REM 
REM ターゲットディレクトリ名を環境変数(PackageDir)で返す
REM 
REM （コマンドライン引数）
REM BuildPackageDirName.bat ProjectName Configuration Platform
REM 

set PackageDir=%~dp0..\..\internal\bin\%~1_%~2_%~3
