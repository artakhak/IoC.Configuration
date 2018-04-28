============================================
Specifying Service Binding In Module Classes
============================================

- The bindings can be specified either in **IoC.Configuration** module classes or in 3-rd party container modules, such as **Autofac** or **Ninject**
    modules (see :doc:`Loading IoC Configuration from XML Configuration File <../loading-ioc-from-xml-configuration-file/index>`) for details on how to specify bindings in XML configuration file.

- To load the modules, use one of the following techniques
   - Include the modules in in element **iocConfiguration/dependencyInjection/modules/module** in XML file configuration file that will be loaded. See the following sections on how to do this:
      - :doc:`Specifying modules in XML configuration file <../loading-ioc-from-xml-configuration-file/modules>`.
      - :doc:`Loading XML Configuration File <../loading-ioc-from-xml-configuration-file/loading-xml-configuration-file.rst>`.

      - Load the modules directly, without loading any XML Configuration file (the module classes are specified as parameters in chained methods).
         To see how this is done, refer to :doc:`Loading IoC Configuration from Modules <../loading-ioc-from-modules/index>`.

.. toctree::

    ioc-configuration-modules.rst
    native-modules.rst