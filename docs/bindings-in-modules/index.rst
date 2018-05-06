=======================
Type Binding In Modules
=======================

- The bindings can be specified either in **IoC.Configuration** module classes or in 3-rd party container modules, such as **Autofac** or **Ninject** modules.

.. note::

    For details on how to specify type bindings in XML configuration file, see :doc:`../xml-configuration-file/index`.

- To load the modules, use one of the following techniques
   - Include the modules in in element **iocConfiguration/dependencyInjection/modules/module** in XML file configuration file, that will be loaded. See the following sections on how to do this:
      - :doc:`../xml-configuration-file/index`.
      - :doc:`../loading-ioc-configuration/loading-from-xml`.

   - Load the modules directly, without loading any XML Configuration file. The module classes are specified as parameters in chained methods.
         To see how this is done, refer to :doc:`../loading-ioc-configuration/loading-from-modules`.

.. toctree::

    ioc-configuration-modules.rst
    native-modules.rst