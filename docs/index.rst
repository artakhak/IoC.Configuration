Welcome to IoC.Configuration's documentation!
=============================================

- IoC.Configuration is a container agnostic configuration of dependency injection.
- Service bindings can be configured using XML configuration file or in module classes, which include **IoC.Configuration** modules as well as modules of one of popular dependency injection containers (e.g., **Autofac**, **Ninject**).

- Service resolutions are done by one of popular IoC containers (e.g., **Autofac**, **Ninject**), through the usage of implementations of **IoC.Configuration.DiContainer.IDiManager** interface.
  Currently, two implementations of **IoC.Configuration.DiContainer.IDiManager**  are availabe (i.e., **Ninject** and **Autofac** implementations). These implementations are in packages `IoC.Configuration.Ninject <https://www.nuget.org/packages/IoC.Configuration.Ninject>`_ and `IoC.Configuration.Autofac <https://www.nuget.org/packages/IoC.Configuration.Autofac>`_.

.. toctree::
   :maxdepth: 3

   overview/index.rst



Indices and tables
==================

* :ref:`genindex`
* :ref:`modindex`
* :ref:`search`
