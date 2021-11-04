@echo  off
REM 
REM Åià¯êîÅj
REM PostbuildEvents.bat SolutionDir TargetDir ProjectName ConfigurationName PlatformName
REM 
setlocal

set SolutionDir=%~1
set TargetDir=%~2
set ProjectName=%~3
set ConfigurationName=%~4
set PlatformName=%~5

set Tools=%SolutionDir%\Tools

call "%Tools%\BuildPackageDirName.bat" %ProjectName% %ConfigurationName% %PlatformName%
REM call "%Tools%\ClearDir.bat" "%PackageDir%"
REM if "%errorlevel%" NEQ "0" (
    REM exit /b 1
REM )
REM 
REM call "%Tools%\CopyPath.bat" "%TargetDir%" "%PackageDir%"
REM if "%errorlevel%" NEQ "0" (
    REM exit /b 1
REM )
call "%Tools%\CopyPath.bat" "%SolutionDir%\HidemaruLspClient_Contract\bin\%PlatformName%\HidemaruLspClient_BackEndContract.tlb" "%TargetDir%"
exit /b %errorlevel%
