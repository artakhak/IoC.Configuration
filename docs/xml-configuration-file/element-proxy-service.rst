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