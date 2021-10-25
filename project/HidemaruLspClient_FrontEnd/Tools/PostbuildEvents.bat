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
call "%Tools%\ClearDir.bat" "%PackageDir%"
if "%errorlevel%" NEQ "0" (
    exit /b 1
)

call "%Tools%\CopyPath.bat" "%TargetDir%" "%PackageDir%"
exit /b %errorlevel%
