using OROptimizer.Xml; // add this using statement to be able to use XmlDocument extensions (i.e., e.XmlDocument.SelectElements("/iocConfiguration/diManagers"), etc.)
// ...
		
using (var containerInfo = 
        new DiContainerBuilder.DiContainerBuilder()
        .StartFileBasedDi(
            new FileBasedConfigurationParameters(
                new FileBasedConfigurationFileContentsProvider(
                    Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration_Overview.xml")),
                // Provide the entry assembly folder. Normally this is the folder,
                // where the executable file is. However for test projects this might not
                // be the case. This folder will be used in assembly resolution.
                Helpers.TestsEntryAssemblyFolder, 
                new LoadedAssembliesForTests())
            {
                AdditionalReferencedAssemblies = new string[]
                {
                    // List additional assemblies that should be added to dynamically generated assembly as references
                    Path.Combine(Helpers.GetTestFilesFolderPath(), @"DynamicallyLoadedDlls\TestProjects.DynamicallyLoadedAssembly1.dll"),
                    Path.Combine(Helpers.GetTestFilesFolderPath(), @"DynamicallyLoadedDlls\TestProjects.DynamicallyLoadedAssembly2.dll")
                },
                // Set the value of AttributeValueTransformers to list of 
                // IoC.Configuration.AttributeValuesProvider.IAttributeValueTransformer instances
                // to change some xml attribute values when the xml configuration is loaded,
                // before the configuration is parsed.
                // Good example of implementation of IoC.Configuration.AttributeValuesProvider.IAttributeValueTransformer
                // is IoC.Configuration.Tests.FileFolderPathAttributeValueTransformer.
                AttributeValueTransformers = new IAttributeValueTransformer []
                {
                    new FileFolderPathAttributeValueTransformer()
                },
                ConfigurationFileXmlDocumentLoaded = (sender, e) =>
                {
                    // Replace some elements in e.XmlDocument if needed,
                    // before the configuration is loaded.
                    // For example, we can replace the value of attribute 'activeDiManagerName' in element 
                    // iocConfiguration.diManagers to use a different DI manager (say
                    // switch from Autofac to Ninject).
                    Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"));
                    e.XmlDocument.SelectElements("/iocConfiguration/diManagers")
                        .First()
                        .SetAttributeValue("activeDiManagerName", "Autofac");
                }
            }, out _)        
        // Note, most of the time we will need to call method WithoutPresetDiContainer().
        // However, in some cases, we might need to create an instance of IoC.Configuration.DiContainer.IDiContainer,
        // and call the method WithDiContainer(IoC.Configuration.DiContainer.IDiContainer diContainer) instead.
        // This might be necessary when using the IoC.Configuration to configure dependency injection in 
        // ASP.NET Core projects.
        // An example implementation of IDIContainer is IoC.Configuration.Autofac.AutofacDiContainer in 
        // Nuget package IoC.Configuration.Autofac.
        .WithoutPresetDiContainer()
         
        // Note, native and IoC.Configuration modules can be specified in XML configuration file, in
        // iocConfiguration/dependencyInjection/modules/module elements.
        // However, if necessary, AddAdditionalDiModules() and AddNativeModules() can be used to load additional
        // IoC.Configuration modules (instances of IoC.Configuration.DiContainer.IDiModule), as well
        // as native (e.g, Ninject or Autofac) modules.
        // Also, AddAdditionalDiModules() and AddNativeModules() can be called multiple times in any order.
        .AddAdditionalDiModules(new TestDiModule())
        .AddNativeModules(CreateModule<object>("Modules.Autofac.AutofacModule1", 
            new ParameterInfo[] { new ParameterInfo(typeof(int), 5) }))
        .RegisterModules()
        .Start())
{
    var diContainer = containerInfo.DiContainer;
    
    // Once the configuration is loaded, resolve types using IoC.Configuration.DiContainer.IDiContainer
    // Note, interface IoC.Configuration.DiContainerBuilder.IContainerInfo extends System.IDisposable,
    // and should be disposed, to make sure all the resources are properly disposed of.
    var resolvedInstance = containerInfo.DiContainer.Resolve<SharedServices.Interfaces.IInterface7>();
}