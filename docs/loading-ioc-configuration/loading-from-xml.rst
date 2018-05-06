===================================
Loading from XML Configuration File
===================================

To load the IoC configuration from XML configuration file use method **IoC.Configuration.DiContainerBuilder.DiContainerBuilder.StartFileBasedDi()** as shown below.

.. sourcecode:: csharp
     :linenos:

        using (var containerInfo =
            new DiContainerBuilder.DiContainerBuilder()
                .StartFileBasedDi(
                            new FileBasedConfigurationFileContentsProvider(
                                Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration1.xml")),
                                // Provide the entry assembly folder. Normally this is the folder, where the executable file is,
                                // however for test projects this might not be the case.
                                // This folder will be used in assembly resolution.
                                Helpers.TestsEntryAssemblyFolder,
                                (sender, e) =>
                                {
                                    // Replace some elements in e.XmlDocument if needed,
                                    // before the configuration is loaded.
                                    // For example, we can replace the value of attribute 'activeDiManagerName' in element
                                    // iocConfiguration.diManagers to use a different DI manager (say switch from Autofac to Ninject).
                                })

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

- Once the configuration is loaded into containerInfo (an instance of **IoC.Configuration.DiContainerBuilder.IContainerInfo**), resolve types using property **DiContainer** in **IoC.Configuration.DiContainerBuilder.IContainerInfo** (the property is of type **IoC.Configuration.DiContainer.IDiContainer**).
- Interface **IoC.Configuration.DiContainerBuilder.IContainerInfo** extends **System.IDisposable**.

- Native and IoC.Configuration modules can be specified in XML configuration file, in **iocConfiguration/dependencyInjection/modules/module** elements.
    However, if necessary, use the following methods in **IoC.Configuration.DiContainerBuilder.FileBased.IFileBasedDiModulesConfigurator** to load additional **IoC.Configuration** modules (instances of IoC.Configuration.DiContainer.IDiModule), as well as native (e.g, Ninject or Autofac) modules:
    - IFileBasedDiModulesConfigurator.AddAdditionalDiModules(params IDiModule[] diModules)
    - IoC.Configuration.DiContainerBuilder.FileBased.IFileBasedDiModulesConfigurator.AddNativeModules(params object[] nativeModules)

    .. note::

        These methods can be called multiple times in any order. In other words, we can add some **IoC.Configuration** modules using **AddAdditionalDiModules**, then some native modules using **AddNativeModules()**, then some more **IoC.Configuration** modules using **AddAdditionalDiModules**.