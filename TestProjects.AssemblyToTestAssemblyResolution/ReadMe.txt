Dependency on IoC.Configuration.Autofac and IoC.Configuration.Ninject was added only to make sure the packages are loaded by Visual Studio.
These pacakages are copied from nuget location by PostBuildCommands.bat in IoC.Configuration.Tests project, into a dll folder,
referenced in configuration file, when IoC.Configuration.Tests is built.