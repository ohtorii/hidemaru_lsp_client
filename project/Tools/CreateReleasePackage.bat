@echo off
REM 
REM CreateReleasePackage.bat ReleaseDir BaseDir
REM 
set ReleaseDir=%1
set BaseDir=%2

@echo off
setlocal

call :Main
exit /b %errorlevel%

:Main
    if not exist "%BaseDir%" (
        echo BaseDirが見つかりません
        exit /b 1
    )
    rmdir /S /Q "%ReleaseDir%"
    if exist "%ReleaseDir%" (
        echo [Error] パッケージディレクトリを削除できませんでした
        exit /b 1
    )
    mkdir "%ReleaseDir%"
    if not exist "%ReleaseDir%" (
        echo [Error] パッケージディレクトリを作成できませんでした
        exit /b 1
    )
    
    xcopy /E /Y "%BaseDir%\*.*" "%ReleaseDir%"
    exit /b %errorlevel%%
