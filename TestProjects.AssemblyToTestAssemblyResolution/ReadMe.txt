This assembly is referenced in project TestProjects.TestPluginAssembly1.
However, TestProjects.TestPluginAssembly1 will be loaded from Plugin1 folder by IoC.Configuration, and
the assembly TestProjects.AssemblyToTestAssemblyResolution will be in a different folder, that is included in 
additionalAssemblyProbingPaths element in IoC.Configuration file.
This project is used to test that assembly resolution works.