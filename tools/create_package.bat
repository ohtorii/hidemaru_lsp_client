@echo off
REM
REM �p�b�P�[�W���쐬����o�b�`�t�@�C��
REM
REM ���g����
REM create_package.bat �o�[�W�����ԍ� �o�͐�f�B���N�g����
REM
REM ���g�p��
REM create_package.bat 1.2.3 c:\project\package
REM

setlocal enabledelayedexpansion

REM --------------------------------------------------------------------
REM �ϐ��ݒ�
REM --------------------------------------------------------------------
REM VisualStudi�̃o�[�W�����ɍ��킹�Ă�������
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
REM ���C������
REM --------------------------------------------------------------------
echo ROOT_DIR=%ROOT_DIR%

set ARG_VERSION=%1
set ARG_OUTDIR=%2

call :Main
if ERRORLEVEL 1 (
    echo ==============================================================
    echo �G���[
    echo ==============================================================
) else (
    echo ==============================================================
    echo ����
    echo ==============================================================
)
exit /b !ERRORLEVEL!



:Main
    REM �����̊m�F
    call :CheckArguments
    if ERRORLEVEL 1 (
        exit /b 1
    )
    call "%VSDEVCMD%"
    if ERRORLEVEL 1 (
        exit /b 1
    )

    REM �����ɕK�v�ȃR�}���h�����݂��邩���ׂ�
    call :CheckCommands
    if ERRORLEVEL 1 (
        exit /b 1
    )

    REM ��Ɨp�̈ꎞ�f�B���N�g�������
    md "%ROOT_DIR%"
    if not exist "%ROOT_DIR%" (
        echo �ꎞ�f�B���N�g�����쐬�ł��܂���ł���
        exit /b 1
    )
    REM ��ƃf�B���N�g���ֈړ�����
    pushd "%ROOT_DIR%"

    REM ���C���̏���
    call :CreatePackageDirectory
    if ERRORLEVEL 1 (
        echo CreatePackageDirectory���s
    )
    set RET=!ERRORLEVEL!
    REM ���̃f�B���N�g���ɖ߂�
    popd
    REM ��n��
    rmdir /S /Q "%ROOT_DIR%"

    exit /b !RET!


:SetupRootDir
    setlocal
    set TEMP_TIME=%time: =0%
    set NOW=%date:/=%%TEMP_TIME:~0,2%%TEMP_TIME:~3,2%%TEMP_TIME:~6,2%
    set ROOT_DIR=%TEMP%\tmp_hm_lspclient\%NOW%_%random%
    endlocal && set ROOT_DIR=%ROOT_DIR%
    exit /b 0


REM �����̊m�F
:CheckArguments
    if "%ARG_VERSION%" == "" (
        echo �o�[�W�����ԍ����w�肵�Ă��������B
        call :Usage
        exit /b 1
    )
    if "%ARG_OUTDIR%" == "" (
        echo �o�̓f�B���N�g�����w�肵�Ă��������B
        call :Usage
        exit /b 1
    )
    exit /b 0


REM �o�b�`�t�@�C�����Ŏg�p����R�}���h�����݂��邩���ׂ�
:CheckCommands
    where git
    if ERRORLEVEL 1 (
        echo git�R�}���h��������܂���
        exit /b 1
    )

    where 7z
    if ERRORLEVEL 1 (
        echo 7z�R�}���h��������܂���
        exit /b 1
    )
    
    where dotnet
    if ERRORLEVEL 1 (
        echo dotnet�R�}���h��������܂���
        exit /b 1
    )
    
    where MSBuild
    if ERRORLEVEL 1 (
        echo MSBuild�R�}���h��������܂���
        exit /b 1
    )
    exit /b 0



:CreatePackageDirectory
    git clone --recursive --depth 1 "%REPOSITORY%"
    if ERRORLEVEL 1 (
        echo git�R�}���h�����s���܂����B
        exit /b 1
    )

    dotnet restore "%SOLUTION%"
    if ERRORLEVEL 1 (
        echo dotnet restore���s
        exit /b 1
    )
    
    MSBuild "%SOLUTION%" -maxcpucount:%NUMBER_OF_PROCESSORS% /t:clean;rebuild /p:Configuration=Release;Platform="x64"
    if ERRORLEVEL 1 (
        echo �r���h���s:x64
        exit /b 1
    )
    
    MSBuild "%SOLUTION%" -maxcpucount:%NUMBER_OF_PROCESSORS% /t:clean;rebuild /p:Configuration=Release;Platform="x86"
    if ERRORLEVEL 1 (
        echo �r���h���s:x86
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
        echo ���s DeleteUnnecessaryFiles
        exit /b 1
    )
    

    REM zip�Ōł߂�
    md "%ARG_OUTDIR%"
    if not exist "%ARG_OUTDIR%" (
        echo �o�̓f�B���N�g�����쐬�ł��܂���ł���
        exit /b 1
    )

    7z.exe a -mx9 -mmt%NUMBER_OF_PROCESSORS% "%ARG_OUTDIR%\hidemaru_lsp_client-%ARG_VERSION%.zip" "%GIT_ROOT_DIR%\"
    if not exist  "%ARG_OUTDIR%\hidemaru_lsp_client-%ARG_VERSION%.zip" (
        echo 7z�R�}���h�����s���܂���
        exit /b 1
    )

    exit /b 0



REM �s�v�ȃt�@�C�����폜����
:DeleteUnnecessaryFiles
    rmdir /S /Q  "%GIT_ROOT_DIR%\.git"
    if exist     "%GIT_ROOT_DIR%\.git" (
        echo .git �f�B���N�g���̍폜�Ɏ��s���܂����B
        exit /b 1
    )

    rmdir /S /Q  "%GIT_ROOT_DIR%\tools"
    if exist     "%GIT_ROOT_DIR%\tools" (
        echo tools �f�B���N�g���̍폜�Ɏ��s���܂����B
        exit /b 1
    )

    rmdir /S /Q  "%GIT_ROOT_DIR%\project"
    if exist     "%GIT_ROOT_DIR%\project" (
        echo project �f�B���N�g���̍폜�Ɏ��s���܂����B
        exit /b 1
    )
    
    del "%GIT_ROOT_DIR%\.gitattributes"
    del "%GIT_ROOT_DIR%\.gitignore"
    exit /b 0


:Usage
    echo;
    echo create_package.bat : Coopyright (c) 2020 ohtorii
    echo;
    echo ���g����
    echo create_package.bat �o�[�W�����ԍ� �o�͐�f�B���N�g����
    echo;
    echo ���g�p��
    echo create_package.bat 1.2.3 c:\project\package
    exit /b 0
