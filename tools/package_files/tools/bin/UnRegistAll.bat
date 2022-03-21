@echo off
setlocal
REM 
REM HidemaruLspClientのCOMをレジストリから全て登録解除します。
REM 
REM [Usage]
REM UnRegistAll.bat x86|x64
REM 

call :Main %*
exit /b %errorlevel%

:Main
    call :GetOption %1
    if errorlevel 1 exit /b 1
    call :DeleteKeys 
    exit /b %errorlevel%

:DeleteKeys
    @echo on
    reg delete "HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{0B0A4550-4B16-456C-B7C7-9EE172234251}"    /f %BIT_OPTION%
    reg delete "HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{0B0A4550-A71F-4142-A4EC-BC6DF50B9590}"    /f %BIT_OPTION%
    reg delete "HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{ef516543-d040-46cc-88b3-fd64c09db652}"    /f %BIT_OPTION%
    reg delete "HKEY_CURRENT_USER\SOFTWARE\Classes\HidemaruLspClient_FrontEnd.Service"              /f %BIT_OPTION%
    reg delete "HKEY_CURRENT_USER\SOFTWARE\Classes\HidemaruLspClient_FrontEnd.ServiceAsync"         /f %BIT_OPTION%
    reg delete "HKEY_CURRENT_USER\SOFTWARE\Classes\TypeLib\{27E4EB65-F8C3-4191-BF62-E46D85964111}"  /f %BIT_OPTION%
    @echo off
    exit /b 0

:GetOption
    if %1==x86 (
        set BIT_OPTION=/reg:32
        exit /b 0
    )
    if %1==x64 (
        set BIT_OPTION=/reg:64
        exit /b 0
    )
    exit /b 1
