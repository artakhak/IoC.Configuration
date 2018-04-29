Welcome to IoC.Configuration's documentation!
=============================================

IoC.Configuration is a container agnostic configuration of dependency injection.

Among other things, it allows specifying dependency injection configuration in XML file, as well as in **IoC.Configuration** module classes,
or third party IoC container modules (e.g., **Autofac** or **Ninject** modules), easy switching between containers to use for service
resolution (e.g., **Autofac**, **Ninject**).
In addition, the configuration file has sections for settings, plugins, startup actions, dynamically generated implementations of interfaces, etc.

All these functionality will be explained in corresponding sections.
Here is a quick start:

- The dependency injection bindings are done use any combination of the following techniques:
    - In **IoC.Configuration** module classes using chained methods, pretty similar to how it is done in popular containers, **Ninject** and  **Autofac**.
      The **IoC.Configuration** module classes should either implement **IoC.Configuration.DiContainer.IDiModule**, or extend the class **IoC.Configuration.DiContainer.ModuleAbstr**, and override the method **Load()**.

    - In module classes of one of popular IoC packages (such as **Autofac** or **Ninject**). These modules will be referred as native modules.

    - In XML configuration file. This method is the preferred way to configure the dependency injection, since it is the most flexible.
      XML configuration files can specify service bindings, as well as **IoC.Configuration** or native (i.e., **Autofac**, **Ninject**, etc) modules.

- Service resolutions are done by one of popular IoC containers (e.g., **Autofac**, **Ninject**), through the usage of implementations of **IoC.Configuration.DiContainer.IDiManager** interface.
  Currently, two implementations of **IoC.Configuration.DiContainer.IDiManager**  are availabe, **Ninject** and **Autofac** implementations).
  These implementations are in packages `IoC.Configuration.Ninject <https://www.nuget.org/packages/IoC.Configuration.Ninject>`_ and
  `IoC.Configuration.Autofac <https://www.nuget.org/packages/IoC.Configuration.Autofac>`_.

- Also, the the container used to resolve services can be switched in XML configuration file.
  Here is an exert from :doc:`IoCConfiguration1.xml <../sample-configuration-file/index>`, that shows usage of element **diManagers** to specify container that will resolve services:

.. code-block:: xml

    <!--
    The value of type attribute should be a type that implements
    IoC.Configuration.DiContainer.IDiManager
    -->
    <diManagers activeDiManagerName="Autofac">
        <diManager name="Ninject" type="IoC.Configuration.Ninject.NinjectDiManager"
                   assembly="ninject_ext">
            <!--Use parameters element to specify constructor parameters, if the type specified in 'type' attribute
            has non-default constructor.-->
            <!--<parameters>
            </parameters>-->
        </diManager>
        <diManager name="Autofac" type="IoC.Configuration.Autofac.AutofacDiManager"
                   assembly="autofac_ext">
        </diManager>
    </diManagers>

.. toctree::
   :maxdepth: 3

   sample-configuration-file/index.rst
   bindings-in-modules/index.rst
   loading-ioc-from-xml-configuration-file/index.rst
   loading-ioc-from-modules/index.rst
   resolving-services/index.rst



Indices and tables
==================

* :ref:`genindex`
* :ref:`modindex`
* :ref:`search`
