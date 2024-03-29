=======================
Type Binding In Modules
=======================

- Type bindings can be specified either in **IoC.Configuration** module classes or in 3-rd party container modules, such as **Autofac** or **Ninject** modules.

.. note::
    For details on how to specify type bindings in XML configuration file, see :doc:`../xml-configuration-file/index`.

- To load the modules, use one of the following techniques
   - Specify the module types in **iocConfiguration/dependencyInjection/modules/module** elements in XML configuration file (if the configuration is loaded from XML configuration). See the following sections for more details on how to do this.
      - :doc:`../xml-configuration-file/index`.
      - :doc:`../loading-ioc-configuration/loading-from-xml.generated`.

   - Load the modules directly, without loading any XML Configuration file. The module classes are specified as parameters in chained methods. To see how this is done, refer to :doc:`../loading-ioc-configuration/loading-from-modules`.

.. toctree::

    ioc-configuration-modules.rst
    native-modules.rst