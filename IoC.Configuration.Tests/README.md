Before running the tests in this project, make sure to modify the folder paths in PostBuildCommands.bat.
The batch file PostBuildCommands.bat is executed on post-build event.

Note, to be able to test that the nuget package IoC.Configuration loads properly, this project references the nuget package rather than
IoC.Configuration itself. To debug IoC.Configuration project, copy the dll and pdb files for this project to bin\debug folder and add a 
reference to this project (no need to remove reference to nuget package IoC.Configuration). After debugging, remove the reference to this project.
At some point will create a symbol server to make this process
easier.

