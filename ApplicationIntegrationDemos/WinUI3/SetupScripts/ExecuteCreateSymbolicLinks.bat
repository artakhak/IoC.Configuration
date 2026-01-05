:: Ensure the script runs from its own directory
cd /d "%~dp0"

echo Running Symbolic Link Creation for all configurations...

call CreateSymbolicLinks.bat Debug
call CreateSymbolicLinks.bat Release

echo All symbolic links processed.
pause