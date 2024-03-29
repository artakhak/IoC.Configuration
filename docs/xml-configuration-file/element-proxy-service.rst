========================
Element **proxyService**
========================

Element **iocConfiguration/dependencyInjection/services/proxyService** (or **iocConfiguration/pluginsSetup/pluginSetup/dependencyInjection/services/proxyService** for plugins) can be used to resolve multiple services to the same implementation.

Lets say we have an interface **IoC.Configuration.Tests.ProxyService.Services.IAppManager_Extension** which extends interfaces
**IoC.Configuration.Tests.ProxyService.Services.IAppManager** and **IoC.Configuration.Tests.ProxyService.Services.IAppManager2**, as shown below.

.. code-block:: csharp

    namespace IoC.Configuration.Tests.ProxyService.Services
    {
        public interface IAppManager_Extension : IAppManager, IAppManager2
        {
            IAppData DefaultApp { get; }
        }
    }

We want to make sure that services **IoC.Configuration.Tests.ProxyService.Services.IAppManager** and **IoC.Configuration.Tests.ProxyService.Services.IAppManager2** are resolved to the same type, to which service **IoC.Configuration.Tests.ProxyService.Services.IAppManager_Extension** is resolved.

This can be done using **proxyService** elements as shown below:

.. code-block:: xml
    :linenos:

    <services>

          <!--IoC.Configuration.Tests.ProxyService.Services.IAppManager
            will be resolved by resolving
            IoC.Configuration.Tests.ProxyService.Services.IAppManager_Extension.-->
          <proxyService
            type="IoC.Configuration.Tests.ProxyService.Services.IAppManager" >
            <serviceToProxy
                type="IoC.Configuration.Tests.ProxyService.Services.IAppManager_Extension" />
          </proxyService>

          <!--IoC.Configuration.Tests.ProxyService.Services.IAppManager2 will
              also be resolved to
              IoC.Configuration.Tests.ProxyService.Services.IAppManager_Extension.-->
          <proxyService type="IoC.Configuration.Tests.ProxyService.Services.IAppManager2" >
            <serviceToProxy
                type="IoC.Configuration.Tests.ProxyService.Services.IAppManager_Extension"/>
          </proxyService>

          <!--Some more services here.-->
    </services>
    
Another use case for proxy services is when have module(s) that scan assemblies and self-binds non-abstract classes. 
In this cases we can use "proxyService" element if we want the interface specified in "proxyService" element to resolve to exactly the same value to which the self bound class is bound.

For example lets say we have a module that has a binding like 

.. code-block:: csharp
    :linenos:
    
    Bind<DatabaseMetadata>().ToSelf().SetResolutionScope(DiResolutionScope.Singleton);
    
If we bind IDatabaseMetadata to DatabaseMetadata in configuration like the following

.. code-block:: xml
    :linenos:
    
    <services>       
        <service type="IDatabaseMetadata">
          <implementation type="DatabaseMetadata" scope="singleton" />
        </service>
    <services>
    
In this case the following two resolutions will result in two different instances

.. code-block:: csharp
    :linenos:
    
    IDiContainer diContainer; // IDiContainer will be initialized from IoC.Configuration
    var instance1 = diContainer.Resolve<DatabaseMetadata>();
    var instance2 = diContainer.Resolve<IDatabaseMetadata>();
    
    
The reason is that the underlying native DI containers (Ninject, Autofac, etc), might disregard that there is a self bound registration for
DatabaseMetadata, when registering the binding for IDatabaseMetadata. In other words, IDatabaseMetadata might be bound by using reflection to create
DatabaseMetadata object, and not be bound by resolving DatabaseMetadata.

To resolve this issue, we might use 'proxyService' element in configuration file to bind IDatabaseMetadata interface to the same instance to which class DatabaseMetadata is bound:

.. code-block:: xml
    :linenos:
    
    <services>
        <proxyService type="IDatabaseMetadata">
            <serviceToProxy type="DatabaseMetadata"/>
        </proxyService>
    <services>
    
The same result can be achieved using binding in module as follows:

.. code-block:: csharp
    :linenos:
    
    public class DiModule1 : IoC.Configuration.DiContainer.ModuleAbstr
    {
        protected override void AddServiceRegistrations()
        {
            Bind<IDatabaseMetadata>().To(x => x.Resolve<MetadataDatabaseMetadata>());
        }
    }