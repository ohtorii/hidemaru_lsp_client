@echo off
setlocal enabledelayedexpansion

REM 
REM CopyPath.bat BasePath PackageDir
REM 


set BasePath=%~1
set PackageDir=%~2

call :Main
exit /b !errorlevel!

:Main
    if not exist "%BasePath%" (
        echo BasePath��������܂���
        exit /b 1
    )
    
    call :IsFolder "%BasePath%"
    if "!errorlevel!" EQU "1" (
        xcopy /E /Y "%BasePath%\*.*" "%PackageDir%"
        exit /b !errorlevel!
    ) 
    if "!errorlevel!" EQU "2" (
        copy /B /Y "%BasePath%" "%PackageDir%"
        exit /b !errorlevel!
    )
    
    echo �������s���ȃt�@�C��
    echo %BasePath%
    exit /b 1
    
:IsFolder
    SET A=%~a1
    REM �t�H���_
    IF %A:~0,1%==d exit /b 1
    REM �t�@�C��
    IF %A:~0,1%==- exit /b 2
    exit /b 3
    
