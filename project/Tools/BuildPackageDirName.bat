@echo off
REM 
REM �^�[�Q�b�g�f�B���N�g���������ϐ�(PackageDir)�ŕԂ�
REM 
REM �i�R�}���h���C�������j
REM BuildPackageDirName.bat ProjectName Configuration Platform
REM 

set PackageDir=%~dp0..\..\internal\bin\%~1_%~2_%~3
