@echo off
setlocal
REM 
REM �f�B���N�g������ɂ���
REM ClearDir.bat TargetDir
REM 

set TargetDir=%~1
rmdir /S /Q "%TargetDir%"
if exist "%TargetDir%" (
    echo [Error] �^�[�Q�b�g�f�B���N�g�����폜�ł��܂���ł���
    echo [Error] �f�B���N�g���� %TargetDir%
    exit /b 1
)
mkdir "%TargetDir%"
if not exist "%TargetDir%" (
    echo [Error] �f�B���N�g�����쐬�ł��܂���ł���
    echo [Error] �f�B���N�g���� %TargetDir%
    exit /b 1
)

exit /b 0
