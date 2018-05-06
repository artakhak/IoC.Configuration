===============
Resolving Types
===============

To resolve the types, we first need to load the configuration into an instance of **IoC.Configuration.DiContainerBuilder.IContainerInfo**.

Refer to sections  :doc:`../loading-ioc-configuration/loading-from-xml` and :doc:`../loading-ioc-configuration/loading-from-modules` for more details.

Here is an example of loading from XML COnfiguration file:

.. The example is taken from
.. sourcecode:: csharp

    var configurationFileContentsProvider = new FileBasedConfigurationFileContentsProvider(
                Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration1.xml"));

    using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()
                   .StartFileBasedDi(configurationFileContentsProvider,
                        Helpers.TestsEntryAssemblyFolder,
                        (sender, e) =>
                        {
                            // Replace some elements in e.XmlDocument if needed,
                            // before the configuration is loaded.
                        })
                   .WithoutPresetDiContainer()
                   .AddAdditionalDiModules(new TestDiModule())
                   .RegisterModules()
                   .Start())
    {
        var container = containerInfo.DiContainer;
        Assert.IsNotNull(containerInfo.DiContainer.Resolve<IInterface6>());
    }

Once the **IoC** is loadexd into **IoC.Configuration.DiContainerBuilder.IContainerInfo**, use methods in **IoC.Configuration.DiContainerBuilder.IContainerInfo.DiContainer** to resolve types.

To resolve types, use methods in in **IoC.Configuration.DiContainerBuilder.IContainerInfo.DiContainer**.

Examples are:

.. sourcecode:: csharp

    IoC.Configuration.DiContainerBuilder.IContainerInfo.DiContainer diContainer; // diContainer is created from XML configuration file or modules.

    var instance1 = diContainer.Resolve(typeof(IInterface3));
    var instance2 = diContainer.Resolve<IInterface3>();

    using (var lifeTimeScope = diContainer.StartLifeTimeScope())
    {
        var instance3 = diContainer.Resolve<IInterface1>(lifeTimeScope);
        var instance4 = diContainer.Resolve(typeof(IInterface3), lifeTimeScope);
    }

When the type is re-solved, the bindings specified in configuration file (see :doc:`../xml-configuration-file/index`) or in module classes (see :doc:`../bindings-in-modules/index`) are used to recursively inject constructor parameters, or to set the property values of resolved types (if property injection is specified in configuration file or in modules).

.. toctree::

    resolution-scopes.rst
    resolving-to-multiple-types.rst








