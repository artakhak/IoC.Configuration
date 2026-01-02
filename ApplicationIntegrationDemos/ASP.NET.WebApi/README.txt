Before running the WebApiDemo.csproj execute the scripts in project SetupScripts.csproj the following way

- In command line change to directory that contains the project SetupScripts.csproj
- Execute ExecuteCreateSymbolicLinks.bat once 
- Execute ExecuteCopyFiles.bat after changes to projects WebApiDemo.DynamicallyLoadedControllers.csproj and WebApiDemo.Extension.csproj (build this projects first, since the script copies the dlls))