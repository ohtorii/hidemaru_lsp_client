@echo off
REM 
REM .tlbƒtƒ@ƒCƒ‹‚ð.dll‚Ö•ÏŠ·‚·‚é
REM 
REM (Usage)
REM tlb2dll.bat TlbFileName
REM 
REM (Example)
REM tlb2dll.bat c:\project\foo.tlb
REM 


set TLBIMP=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\TlbImp.exe
REM set TLBIMP=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools\TlbImp.exe
REM set TLBIMP=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\TlbImp.exe

set INFILE=%1
set OUTDIR=%~dp1
set OUTFILENAME=%~n1.dll
echo INFILE=%INFILE%
echo OUTDIR=%OUTDIR%
echo OUTFILENAME=%OUTFILENAME%
"%TLBIMP%" "%INFILE%" /out:"%OUTDIR%\%OUTFILENAME%" /publickey:%~dp0public.key


