=================
Resolution Scopes
=================

Resolution scope determines if the same object or different objects will be returned by the container on subsequent requests of type.

**IoC.Configuration** supports three types of resolution scopes: Singleton, ScopeLifetime, and Transient scopes.


.. contents::
   :local:
   :depth: 2

Scope: Singleton
================

Singleton scope results in type being resolved to the same instance on subsequent requests.

Here is an example of specifying **Singleton** resolution scope in method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()** (see :doc:`Type Bindings in IoC.Configuration Modules <../bindings-in-modules/ioc-configuration-modules>`):

.. sourcecode:: csharp

    public class TestDiModule : IoC.Configuration.DiContainer.ModuleAbstr
    {
        protected override void AddServiceRegistrations()
        {
            Bind<SharedServices.Interfaces.IInterface9>()
            .To<SharedServices.Implementations.Interface9_Impl1>()
            .SetResolutionScope(DiResolutionScope.Singleton);
        }
    }

Here is an example of specifying **Singleton** resolution scope in XML configuration file (see :doc:`../xml-configuration-file/type-bindings`):

.. code-block:: xml

    <service type="SharedServices.Interfaces.IInterface9" assembly="shared_services">
        <implementation type="SharedServices.Implementations.Interface9_Impl1"
                                    assembly="shared_services"
                                    scope="singleton" />
    </service>

Here is an example of resolving types bound with **Singleton** scope resolution

.. sourcecode:: csharp

    private void SingletonScopeResolutionExample(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        var service1 = diContainer.Resolve<IInterface9>();
        var service2 = diContainer.Resolve<IInterface9>();
        Assert.AreSame(service1, service2);
    }

Scope: Transient
================

Transient scope results in type being resolved to a newly created instance on each requests.

Here is an example of specifying **Transient** resolution scope in method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()** (see :doc:`Type Bindings in IoC.Configuration Modules <../bindings-in-modules/ioc-configuration-modules>`):

.. sourcecode:: csharp

    public class TestDiModule : IoC.Configuration.DiContainer.ModuleAbstr
    {
        protected override void AddServiceRegistrations()
        {
            Bind<DynamicallyLoadedAssembly1.Interfaces.IInterface2>()
            .To<SharedServices.DynamicallyLoadedAssembly1.Implementations.Interface2_Impl1>()
            .SetResolutionScope(DiResolutionScope.Transient);
        }
    }

Here is an example of specifying **Transient** resolution scope in XML configuration file (see :doc:`../xml-configuration-file/type-bindings`):

.. code-block:: xml

    <service type="DynamicallyLoadedAssembly1.Interfaces.IInterface2"
                assembly="dynamic1">
        <implementation type="DynamicallyLoadedAssembly1.Implementations.Interface2_Impl1"
                        assembly="dynamic1"
                        scope="transient">
        </implementation>
    </service>

Here is an example of resolving types bound with **Transient** scope resolution

.. sourcecode:: csharp

    private void TransientScopeResolutionExample(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        Type typeInterface2 = Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface2");

        var service1 = diContainer.Resolve(typeInterface2);
        var service2 = diContainer.Resolve(typeInterface2);
        Assert.AreNotSame(service1, service2);
    }

Scope: ScopeLifetime
====================

ScopeLifetime scope results in type being resolved to the same instance on subsequent requests, if the same instance of **IoC.Configuration.DiContainer.ILifeTimeScope** is used as a parameter to method **diContainer.Resolve(Type typeToResolve, ILifeTimeScope lifetimeScope)**.

.. note::

    If DiResolutionScope.ScopeLifetime was not used when specifying the binding for the type, the value passed for **ILifeTimeScope** parameter in **diContainer.Resolve(Type typeToResolve, ILifeTimeScope lifetimeScope)** does not matter, and the type will be resolved with resoltion scope used in type binding (e.g., **Singletone**, **Transient**).

Here is an example of specifying **ScopeLifetime** resolution scope in method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()** (see :doc:`../bindings-in-modules/ioc-configuration-modules`):

.. sourcecode:: csharp

    public class TestDiModule : IoC.Configuration.DiContainer.ModuleAbstr
    {
        protected override void AddServiceRegistrations()
        {
            Bind<DynamicallyLoadedAssembly1.Interfaces.IInterface3>()
            .To<DynamicallyLoadedAssembly1.Implementations.Interface3_Impl1>()
            .SetResolutionScope(DiResolutionScope.ScopeLifetime);
        }
    }

Here is an example of specifying **ScopeLifetime** resolution scope in XML configuration file (see :doc:`../xml-configuration-file/type-bindings`):

.. code-block:: xml

    <service type="DynamicallyLoadedAssembly1.Interfaces.IInterface3" assembly="dynamic1">
        <implementation type="DynamicallyLoadedAssembly1.Implementations.Interface3_Impl1"
                                    assembly="dynamic1"
                                    scope="scopeLifetime">
        </implementation>
    </service>


Here is an example of resolving types bound with **ScopeLifetime** scope resolution

.. sourcecode:: csharp

    private void LifetimeScopeResolutionExample(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        Type typeInterface3 = Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface3");

        // Same objects are created in default lifetime scope.
        var service1InMainScope = diContainer.Resolve(typeInterface3);
        var service2InMainScope = diContainer.Resolve(typeInterface3);

        Assert.AreSame(service1InMainScope, service2InMainScope);

        using (var lifeTimeScope = diContainer.StartLifeTimeScope())
        {
            // IDiContainer.Resolve(Type, ILifetimeScope) returns the same object for the same scope lifeTimeScope.
            var service1InScope1 = diContainer.Resolve(typeInterface3, lifeTimeScope);
            var service2InScope1 = diContainer.Resolve(typeInterface3, lifeTimeScope);

            Assert.AreSame(service1InScope1, service2InScope1);

            // However, the objects are different from the ones created in main lifetime scope.
            Assert.AreNotSame(service1InScope1, service1InMainScope);
        }
    }



