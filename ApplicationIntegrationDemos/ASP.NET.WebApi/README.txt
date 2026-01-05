Before running the WebApiDemo.csproj execute the scripts in project SetupScripts.csproj the following way

- In the command line change to the directory that contains the project SetupScripts.csproj
- Execute ExecuteCopyFiles.bat after changes to projects WebApiDemo.DynamicallyLoadedControllers.csproj and WebApiDemo.Extension.csproj (build these projects first, since the script copies the dlls))
- Execute ExecuteCreateSymbolicLinks.bat only once (no need to run this script again after executing ExecuteCopyFiles.bat again) to create the symbolic links.