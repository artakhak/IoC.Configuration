@echo off
:: Access the first argument
set "CONFIG=%~1"

:: Check if the argument is actually empty or just whitespace
if "%CONFIG%"=="" goto :usage
if "%CONFIG%"==" " goto :usage

set "LINK_PATH=K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\ApplicationIntegrationDemos\WinUI3\WinUI3Demo\bin\x64\%CONFIG%\net8.0-windows10.0.19041.0\win-x64\IoCConfigurationFiles"
set "TARGET_PATH=K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\ApplicationIntegrationDemos\WinUI3\WinUI3Demo\bin\IoCConfigurationFiles"

echo Creating symbolic link for configuration: [%CONFIG%]

:: Remove existing link/folder
if exist "%LINK_PATH%" (
    echo Removing existing path: %LINK_PATH%
    rmdir /q "%LINK_PATH%"
)

mklink /D "%LINK_PATH%" "%TARGET_PATH%"
echo Finished %CONFIG%
echo ---------------------------------------
exit /b 0

:usage
echo Error: No configuration specified.
echo Usage: CreateSymbolicLinks.bat [Debug|Release]
exit /b 1