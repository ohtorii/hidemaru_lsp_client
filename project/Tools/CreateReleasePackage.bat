@echo off
setlocal enabledelayedexpansion

REM 
REM CreateReleasePackage.bat BasePath ProjectName Configuration Platform
REM 


set BasePath=%~1
set ProjectName=%~2
set Configuration=%~3
set Platform=%~4
echo BasePath=%BasePath%

set ReleaseDir=%~dp0..\..\internal\bin\%ProjectName%_%Configuration%_%Platform%

call :Main
exit /b !errorlevel!

:Main
    if not exist "%BasePath%" (
        echo BasePath��������܂���
        exit /b 1
    )
    
    call :MakeReleaseDir
    if "!errorlevel!" NEQ "0" (
        exit /b 1
    )
    
    call :IsFolder "%BasePath%"
    if "!errorlevel!" EQU "1" (
        xcopy /E /Y "%BasePath%\*.*" "%ReleaseDir%"
        exit /b !errorlevel!
    ) 
    if "!errorlevel!" EQU "2" (
        copy /B /Y "%BasePath%" "%ReleaseDir%"
        exit /b !errorlevel!
    )
    
    echo �������s���ȃt�@�C��
    echo %BasePath%
    exit /b 1
    

:MakeReleaseDir
    REM rmdir /S /Q "%ReleaseDir%"
    REM if exist "%ReleaseDir%" (
        REM echo [Error] �p�b�P�[�W�f�B���N�g�����폜�ł��܂���ł���
        REM exit /b 1
    REM )
    mkdir "%ReleaseDir%"
    if not exist "%ReleaseDir%" (
        echo [Error] �p�b�P�[�W�f�B���N�g�����쐬�ł��܂���ł���
        exit /b 1
    )
    exit /b 0

:IsFolder
    SET A=%~a1
    REM �t�H���_
    IF %A:~0,1%==d exit /b 1
    REM �t�@�C��
    IF %A:~0,1%==- exit /b 2
    exit /b 3
    
