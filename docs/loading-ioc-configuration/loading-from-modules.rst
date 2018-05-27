====================
Loading from Modules
====================

To load the IoC configuration from XML configuration file use method **IoC.Configuration.DiContainerBuilder.DiContainerBuilder.StartCodeBasedDi()** as shown below.

.. sourcecode:: csharp
     :linenos:

        var assemblyProbingPaths = new[]
        {
            @"K:\...\TestDlls\ThirdPartyLibs",
            @"K:\...\TestDlls\ContainerImplementations\Autofac"
        };

        using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()

                // Class IoC.Configuration.DiContainerBuilder.DiContainerBuilder has two overloaded methods StartCodeBasedDi(...)
                // DiContainerBuilder.StartCodeBasedDi(IoC.Configuration.DiContainer.IDiManager diManager,...) and
                // DiContainerBuilder.StartCodeBasedDi(string diManagerClassFullName, string diManagerClassAssemblyFilePath,...).
                // if the project references the library with implementation of IoC.Configuration.DiContainer.IDiManager,
                // the first one can be used. Otherwise the second overloaded method can be used, in which case reflection will be used to
                // create an instance of IoC.Configuration.DiContainer.IDiManager.
                .StartCodeBasedDi("IoC.Configuration.Autofac.meake cleanAutofacDiManager",
                               @"K:\...\TestDlls\ContainerImplementations\Autofac\IoC.Configuration.Autofac.dll",
                               new ParameterInfo[0], Helpers.TestsEntryAssemblyFolder, assemblyProbingPaths)
                // Note, most of the time we will need to call method WithoutPresetDiContainer().
                // However, in some cases, we might need to create an instance of IoC.Configuration.DiContainer.IDiContainer,
                // and call the method WithDiContainer(IoC.Configuration.DiContainer.IDiContainer diContainer) instead.
                // This might be necessary when using the IoC.Configuration to configure dependency injection in
                // ASP.NET Core projects.
                // An example implementation of IDIContainer is IoC.Configuration.Autofac.AutofacDiContainer in
                // Nuget package IoC.Configuration.Autofac.
                .WithoutPresetDiContainer()

                // The methods AddDiModules(params IDiModule[] diModules),
                // AddNativeModules(params object[] nativeModules), and
                // AddNativeModules(string nativeModuleClassFullName, string nativeModuleClassAssemblyFilePath, ...)
                // are used to load IoC.Configuration modules (instances of IoC.Configuration.DiContainer.IDiModule), as well
                // as native (e.g, Ninject or Autofac) modules.
                // Also, these three methods can be called multiple times in any order.
                .AddDiModules(new TestDiModule())
                .AddNativeModule("Modules.Autofac.AutofacModule1",
                    @"K:\...\TestDlls\DynamicallyLoadedDlls\TestProjects.Modules.dll",
                    new ParameterInfo[] { new ParameterInfo(typeof(int), 5) })

                .RegisterModules()
                .Start())
        {
            var diContainer = containerInfo.DiContainer;

            // Once the configuration is loaded, resolve types using IoC.Configuration.DiContainer.IDiContainer
            // Note, interface IoC.Configuration.DiContainerBuilder.IContainerInfo extends System.IDisposable,
            // and should be disposed, to make sure all the resources are properly disposed of.
            var resolvedInstance = containerInfo.DiContainer.Resolve<IInterface2>();
        }

- Once the configuration is loaded into containerInfo (an instance of **IoC.Configuration.DiContainerBuilder.IContainerInfo**), resolve types using property **DiContainer** in **IoC.Configuration.DiContainerBuilder.IContainerInfo** (the property is of type **IoC.Configuration.DiContainer.IDiContainer**).
- Interface **IoC.Configuration.DiContainerBuilder.IContainerInfo** extends System.IDisposable.

- Use one of the following overloaded methods in class **IoC.Configuration.DiContainerBuilder.DiContainerBuilder** to specify an instance of **IoC.Configuration.DiContainer.IDiManager**, that handles the type resolutions and translates the bindings in **IoC.Configuration** modules into native container bindings (e.g., Autofac and Ninject bindings). If the project references the library with implementation of IoC.Configuration.DiContainer.IDiManager, the first one can be used. Otherwise the second overloaded method can be used, in which case reflection will be used to create an instance of **IoC.Configuration.DiContainer.IDiManager**.
    - DiContainerBuilder.StartCodeBasedDi(IDiManager diManager, string entryAssemblyFolder, params string[] assemblyProbingPaths)
    - DiContainerBuilder.StartCodeBasedDi(string diManagerClassFullName, string diManagerClassAssemblyFilePath, ParameterInfo[] diManagerConstructorParameters, string entryAssemblyFolder, params string[] assemblyProbingPaths)

    .. note::
        Currently two implementations of **IoC.Configuration.DiContainer.IDiManager** are available: **IoC.Configuration.Autofac.AutofacDiManager** and **IoC.Configuration.Ninject.NinjectDiManager**. These implementations are available in Nuget packages `IoC.Configuration.Autofac <https://www.nuget.org/packages/IoC.Configuration.Autofac>`_ and `IoC.Configuration.Ninject <https://www.nuget.org/packages/IoC.Configuration.Ninject>`_

- The following methods in interface **IoC.Configuration.DiContainerBuilder.CodeBased.ICodeBasedDiModulesConfigurator** can be used to load **IoC.Configuration** modules **IoC.Configuration** modules (instances of **IoC.Configuration.DiContainer.IDiModule**), as well as native (e.g, Ninject or Autofac) modules:
    - ICodeBasedDiModulesConfigurator.AddDiModules(params IoC.Configuration.DiContainer.IDiModule[] diModules)
    - ICodeBasedDiModulesConfigurator.AddNativeModules(params object[] nativeModules)
    - ICodeBasedDiModulesConfigurator.AddNativeModule(string nativeModuleClassFullName, string nativeModuleClassAssemblyFilePath, ParameterInfo[] nativeModuleConstructorParameters)

    .. note::
        These methods can be called multiple times in any order.