@echo off
REM 
REM Coverity�����ɕK�v�ȃt�@�C���𐶐�����
REM 
REM �i�O�����j
REM �K�v�ɉ����� bin\cov-configure --msvc �R�}���h�����s���邱�ƁB
REM 

setlocal

set COV_BUILD=C:\cov-analysis-win64-2021.12.1-csharp\bin\cov-build.exe

REM �t�H���_���͕ς��Ȃ�����(Coverity�̃}�j���A���ɐ�������)
set COV_DIR=cov-int

set SOLUTION=%~dp0..\lsp-all.sln
set BUILD_COMMAND=msbuild %SOLUTION% /t:clean;rebuild /p:Configuration=Release;Platform=x64;PostBuildEventUseInBuild=false
if not exist %COV_BUILD% (
    echo cov-build.exe ��������܂���
    exit /b 1
)

cd %~dp0
call :Main
exit /b %errorlevel%

:Main
	call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat"
	if %errorlevel% neq 0 (
		echo VsDevCmd.bat�̎��s�Ɏ��s���܂���
	)
	%COV_BUILD% --dir %COV_DIR% %BUILD_COMMAND%
	if %errorlevel% neq 0 (
		echo cov-build�����s���܂���
		exit /b 1
	)
	REM �������ʂ�WEB�փA�b�v���邽�߂�tar�Ōł߂�
	tar czvf hidemaru_lsp_client.tgz %COV_DIR%
	exit /b 0
