===================================
Loading IoC Injection Configuration
===================================

- The dependency injection configuration can be loaded either from modules (see :doc:`../bindings-in-modules/index` for more details on modules), or from XML Configuration file, in which module classes can be specified (see :doc:`../xml-configuration-file/index` for more details on XML Configuration file).
- Before the configuration is loaded, an instance of **OROptimizer.Diagnostics.Log.ILogHelperContext** should be registered, using **OROptimizer.Diagnostics.Log.LogHelper.RegisterContext(ILogHelperContext)**. An implementation of **ILogHelperContext** for **log4net**, **OROptimizer.Diagnostics.Log.Log4NetHelperContext**, can be found in Nuget package **OROptimizer.Shared**.

    Here is an example of registering the logger:

    .. sourcecode:: csharp

        if (!OROptimizer.Diagnostics.Log.LogHelper.IsContextInitialized)
            OROptimizer.Diagnostics.Log.LogHelper.RegisterContext(
                new Log4NetHelperContext("MyApp.log4net.config"));

.. toctree::

    loading-from-xml.rst
    loading-from-modules.rst