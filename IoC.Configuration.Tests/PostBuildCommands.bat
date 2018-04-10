REM In Visual Studio post build event use this command to execute the batch 
REM call $(ProjectDir)PostBuildCommands.bat $(ConfigurationName)

REM ConfigurationName is either Debug or Release
SET ConfigurationName=%1

SET "SolutionDir=K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration"

SET "DynamicDllsDir=K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\IoC.Configuration.Tests\TestDlls"
SET "NugetPackagesDir=C:\Users\artak\.nuget\packages"
SET "ThirdPartyLibsDir=K:\Projects\OROptimizer\MyGitHubProjects\IoC.Configuration\IoC.Configuration.Tests\TestDlls\ThirdPartyLibs"

REM Copy third party libraries
xcopy "%NugetPackagesDir%\ninject\3.3.0\lib\netstandard2.0\Ninject.*" %ThirdPartyLibsDir% /Y 
xcopy "%NugetPackagesDir%\autofac\4.5.0\lib\netstandard1.1\Autofac.*" %ThirdPartyLibsDir% /Y
xcopy "%NugetPackagesDir%\autofac.extensions.dependencyinjection\4.0.0\lib\netstandard1.1\Autofac.Extensions.DependencyInjection.*" %ThirdPartyLibsDir% /Y

xcopy "%NugetPackagesDir%\oroptimizer.shared\1.0.0-beta2\lib\netstandard2.0\OROptimizer.Shared.*" %ThirdPartyLibsDir% /Y 

REM Copy IoC extensions
xcopy "%NugetPackagesDir%\ioc.configuration.autofac\1.0.0-beta2\lib\netstandard2.0\IoC.Configuration.Autofac.*"  %DynamicDllsDir%\ContainerImplementations\Autofac /Y 
xcopy "%NugetPackagesDir%\ioc.configuration.ninject\1.0.0-beta2\lib\netstandard2.0\IoC.Configuration.Ninject.*"  %DynamicDllsDir%\ContainerImplementations\Ninject /Y 

REM copy DynamicallyLoadedDlls
xcopy %SolutionDir%\TestProjects.AssemblyToTestAssemblyResolution\bin\%ConfigurationName%\TestProjects.AssemblyToTestAssemblyResolution.*  %DynamicDllsDir%\TestAssemblyResolution /Y 

xcopy %SolutionDir%\TestProjects.Modules\bin\%ConfigurationName%\TestProjects.Modules.*  %DynamicDllsDir%\DynamicallyLoadedDlls /Y 

xcopy %SolutionDir%\TestProjects.DynamicallyLoadedAssembly1\bin\%ConfigurationName%\TestProjects.DynamicallyLoadedAssembly1.*  %DynamicDllsDir%\DynamicallyLoadedDlls /Y 
xcopy %SolutionDir%\TestProjects.DynamicallyLoadedAssembly2\bin\%ConfigurationName%\TestProjects.DynamicallyLoadedAssembly2.*  %DynamicDllsDir%\DynamicallyLoadedDlls /Y 
xcopy %SolutionDir%\TestProjects.TestForceLoadAssembly\bin\%ConfigurationName%\TestProjects.TestForceLoadAssembly.*  %DynamicDllsDir%\DynamicallyLoadedDlls /Y 

REM copy plugin DLLs
xcopy %SolutionDir%\TestProjects.TestPluginAssembly1\bin\%ConfigurationName%\TestProjects.TestPluginAssembly1.*  %DynamicDllsDir%\PluginDlls\Plugin1 /Y 
xcopy %SolutionDir%\TestProjects.ModulesForPlugin1\bin\%ConfigurationName%\TestProjects.ModulesForPlugin1.*  %DynamicDllsDir%\PluginDlls\Plugin1 /Y 

xcopy %SolutionDir%\TestProjects.TestPluginAssembly2\bin\%ConfigurationName%\TestProjects.TestPluginAssembly2.*  %DynamicDllsDir%\PluginDlls\Plugin2 /Y 
xcopy %SolutionDir%\TestProjects.TestPluginAssembly3\bin\%ConfigurationName%\TestProjects.TestPluginAssembly3.*  %DynamicDllsDir%\PluginDlls\Plugin3 /Y 

pause