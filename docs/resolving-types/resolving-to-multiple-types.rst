=================
Multiple Bindings
=================

If we know that only one binding for the type was specified in in configuration file (see :doc:`../xml-configuration-file/index`) or in modules (see :doc:`../bindings-in-modules/index`), we can resolve the type by specifying the type as parameter to **IoC.Configuration.DiContainer.IDiContainer.Resove(Type)** method as shown below:

.. sourcecode:: csharp

    private void ResolvingATypeWithSingleBinding(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        Type typeInterface2 = Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface2");

        var service1 = diContainer.Resolve(typeInterface2);
        var service2 = diContainer.Resolve(typeInterface2);
        Assert.AreNotSame(service1, service2);
    }


However, multiple bindings might be specified for the same type as well. Below are examples of specifying multiple bindings for the same type in configuration file, and in overridden method in method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**.

Example of multiple bindings for type in XML configuration file:

.. code-block:: xml

    <service type="SharedServices.Interfaces.IInterface5" assembly="shared_services">
        <implementation type="SharedServices.Implementations.Interface5_Impl1"
                                assembly="shared_services"
                                scope="singleton" />
        <implementation type="TestPluginAssembly1.Implementations.Interface5_Plugin1Impl"
                                assembly="pluginassm1" scope="singleton" />
        <implementation type="TestPluginAssembly2.Implementations.Interface5_Plugin2Impl"
                                assembly="pluginassm2" scope="transient" />
    </service>

Example of multiple bindings for type in overridden method in method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**:

.. sourcecode:: csharp

    public class TestDiModule : IoC.Configuration.DiContainer.ModuleAbstr
    {
        protected override void AddServiceRegistrations()
        {

            Bind<SharedServices.Interfaces.IInterface5>()
                        .To<SharedServices.Implementations.Interface5_Impl1>()
                        .SetResolutionScope(DiResolutionScope.Singleton);

            Bind<SharedServices.Interfaces.IInterface5>()
                        .To<TestPluginAssembly1.Implementations.Interface5_Plugin1Impl>()
                        .SetResolutionScope(DiResolutionScope.Singleton);

            Bind<SharedServices.Interfaces.IInterface5>()
                        .To<TestPluginAssembly2.Implementations.Interface5_Plugin2Impl>()
                        .SetResolutionScope(DiResolutionScope.Transient);
        }
    }

To resolve types that are bound to multiple types, resolve type **System.Collections.Generic.IEnumerable<TService>**.

.. note::

    We still can resolve to a single type, rather than to a collection. However, not all implementations support this resolution, when multiple bindings exist. For example, **Autofac** implementation will resolve the type to the last binding, while **Ninject** implementation will throw an exception.

.. sourcecode:: csharp

    private void ResolvingATypeWithMultipleBindings(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        var resolvedInstances = diContainer.Resolve<IEnumerable<SharedServices.Interfaces.IInterface5>>()
                                           .ToList();

        Assert.AreEqual(3, resolvedInstances.Count);

        var typeOfInterface5 = typeof(IInterface5);
        Assert.IsInstanceOfType(resolvedInstances[0], typeOfInterface5);
        Assert.IsInstanceOfType(resolvedInstances[1], typeOfInterface5);
        Assert.IsInstanceOfType(resolvedInstances[2], typeOfInterface5);
    }