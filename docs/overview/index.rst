==========================
IoC.Configuration Overview
==========================

- The dependency injection bindings are done in one or combination of the following techniques:
    - In **IoC.Configuration** module classes using chained methods, pretty similar to how it is done in popular containers, **Ninject** and  **Autofac**.
      The **IoC.Configuration** module classes should either implement **IoC.Configuration.DiContainer.IDiModule**, or extend the class **IoC.Configuration.DiContainer.ModuleAbstr**, and override the method **Load()**.

    - In module classes of one of popular IoC packages (such as **Autofac** or **Ninject**). These modules will be referred as native modules.

    - In XML configuration file. This method is the preferred way to configure the dependency injection, since it is the most flexible.
        XML configuration files can specify service bindings, as well as **IoC.Configuration** or native (i.e., **Autofac**, **Ninject**, etc) modules.

- Service resolutions are done by one of popular IoC containers (e.g., **Autofac**, **Ninject**), through the usage of implementations of **IoC.Configuration.DiContainer.IDiManager** interface.
  Currently, two implementations of **IoC.Configuration.DiContainer.IDiManager**  are availabe (i.e., **Ninject** and **Autofac** implementations). These implementations are in packages `IoC.Configuration.Ninject <https://www.nuget.org/packages/IoC.Configuration.Ninject>`_ and `IoC.Configuration.Autofac <https://www.nuget.org/packages/IoC.Configuration.Autofac>`_.
