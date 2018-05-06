=============================================
Type Bindings in 3-rd Party Container Modules
=============================================

Third party modules classes can be used to specify bindings.
The module class should be a sub-class or an implementation of a type
specified returned by property **IoC.Configuration.DiContainer.IDiManager.ModuleType** of **IoC.Configuration.DiContainer.IDiManager** object used to load the configuration.
To see of how **IoC.Configuration.DiContainer.IDiManager** type can be specified when loading the configuration, reference :doc:`../loading-ioc-configuration/loading-from-modules` (loading from modules) or :doc:`../xml-configuration-file/di-managers` (loading from configuration file).

Currently two implementations of **IoC.Configuration.DiContainer.IDiManager**:
    - **IoC.Configuration.Autofac.AutofacDiManager** available in Nuget package `IoC.Configuration.Autofac <https://www.nuget.org/packages/IoC.Configuration.Autofac>`_
    - **IoC.Configuration.Autofac.NinjectDiManager** available in Nuget package `IoC.Configuration.Ninject <https://www.nuget.org/packages/IoC.Configuration.Ninject>`_

The module types are passed as parameters, when loaded the configuration from modules (see :doc:`../loading-ioc-configuration/loading-from-modules`), or in **iocConfiguration/dependencyInjection/modules/module** elements in XML configuration file, if the configuration is loaded from XML file (see :doc:`../xml-configuration-file/modules`).

.. note::
    If the native module has a public method **void OnDiContainerReady(IDiContainer diContainer)**, **IoC.Configuration** will call this method, when the dependency injection is loaded. The native module can use the **IDiContainer** object to resolve types in type bindings.

Here is an example of **Autofac** module:

.. sourcecode:: csharp

    public class AutofacModule1 : AutofacModule
    {
        public IDiContainer DiContainer { get; private set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Interface1_Impl1>()
                    .As<IInterface1>()
                    .SingleInstance();
        }

        /// <summary>
        ///   The value of parameter <paramref name="diContainer" />
        ///   will be injected by <see cref="DiContainerBuilder" />.
        /// </summary>
        /// <param name="diContainer"></param>
        public void OnDiContainerReady(IDiContainer diContainer)
        {
            DiContainer = diContainer;
        }
    }

..

Here is an example of **Ninject** module:

.. sourcecode:: csharp

    public class NinjectModule1 : NinjectModule
    {
        public IDiContainer DiContainer { get; private set; }

        public override void Load()
        {
            Bind<IInterface1>().To<Interface1_Impl2>()
                               .InSingletonScope();
        }

        /// <summary>
        ///   The value of parameter <paramref name="diContainer" />
        ///   will be injected by <see cref="DiContainerBuilder" />.
        /// </summary>
        /// <param name="diContainer"></param>
        public void OnDiContainerReady(IDiContainer diContainer)
        {
            DiContainer = diContainer;
        }
    }