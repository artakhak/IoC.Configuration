===================================
Loading from XML Configuration File
===================================

An example of XML configuration file can be found at :doc:`../sample-configuration-file/index`. This file is used in test project `IoC.Configuration.Tests <https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration.Tests>`_.
-The XML Configuration file is validated against XML schema file **IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd** (see :doc:`./xml-configuration-file-schema`).
-A template XML Configuration file **IoC.Configuration.Template.xml** can be found in folder **IoC.Configuration.Content**, where the Nuget package **IoC.Configuration** is installed (see :doc:`./xml-configuration-template`).

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
                                    e.XmlDocument.SelectElements("/iocConfiguration/diManagers").First()
                                                 .SetAttributeValue("activeDiManagerName", "Autofac");
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
- Native and IoC.Configuration modules can be specified in XML configuration file, in **iocConfiguration/dependencyInjection/modules/module** elements. However, if necessary, use the following methods in **IoC.Configuration.DiContainerBuilder.FileBased.IFileBasedDiModulesConfigurator** to load additional **IoC.Configuration** modules (instances of IoC.Configuration.DiContainer.IDiModule), as well as native (e.g, Ninject or Autofac) modules:
    - IFileBasedDiModulesConfigurator.AddAdditionalDiModules(params IDiModule[] diModules)
    - IFileBasedDiModulesConfigurator.AddNativeModules(params object[] nativeModules)

    .. note::

        These methods can be called multiple times in any order. In other words, we can add some **IoC.Configuration** modules using **AddAdditionalDiModules**, then some native modules using **AddNativeModules()**, then some more **IoC.Configuration** modules using **AddAdditionalDiModules**.

Modifying XML Configuration at Runtime
======================================

The XML Configuration file can be modified at runtime by passing a delegate for parameter **configurationFileXmlDocumentLoaded** in method **IoC.Configuration.DiContainerBuilder.StartFileBasedDi(IConfigurationFileContentsProvider configurationFileContentsProvider, string entryAssemblyFolder, ConfigurationFileXmlDocumentLoadedEventHandler configurationFileXmlDocumentLoaded = null)**.
This method loads the configuration file into an instance of **System.Xml.XmlDocument** object, and executes the delagate passed in parameter **configurationFileXmlDocumentLoaded**.

By the time the delegate is executed, **System.Xml.XmlDocument** object is not yet validated against the XML schema file **IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd** (this is done after the delegate is executed). Therefore, the changes to **System.Xml.XmlDocument** object should be done in such a way that the XML document is successfully validated against this schema file.
Example of modifying the XML document at runtime to replace the value of attribute **activeDiManagerName** in element **/iocConfiguration/diManagers** with **Autofac** is shown below (this is copied from the C# code above).

.. sourcecode:: csharp

    new DiContainerBuilder.DiContainerBuilder()
           .StartFileBasedDi(
                // Other parameters...
                (sender, e) =>
                {
                    e.XmlDocument.SelectElements("/iocConfiguration/diManagers").First()
                         .SetAttributeValue("activeDiManagerName", "Autofac");
                })

.. toctree::

    xml-configuration-template.rst
    xml-configuration-file-schema.rst