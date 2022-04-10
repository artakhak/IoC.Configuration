=======
Modules
=======

The **iocConfiguration/dependencyInjection/modules** element lists the modules (both **IoC.Configuration** modules, as well as native modules, such as **Autofac** or **Ninject** modules), that should be loaded into the **IoC** container. See :doc:`../bindings-in-modules/index` for more details on modules.

Here is an example of **modules** element in configuration file:

.. code-block:: xml
    :linenos:

        <iocConfiguration>
        <!--...-->
            <dependencyInjection>
                <modules>
                    <!--Type Modules.Autofac.AutofacModule1 is an Autofac module and is a
                                            subclass of Autofac.AutofacModule-->
                    <module type="Modules.Autofac.AutofacModule1" assembly="modules">
                        <parameters>
                            <int32 name="param1" value="1" />
                        </parameters>
                    </module>

                    <!--Type Modules.IoC.DiModule1 is an IoC.Configuration module and is a
                        subclass of IoC.Configuration.DiContainer.ModuleAbstr-->
                    <module type="Modules.IoC.DiModule1" assembly="modules">
                        <parameters>
                            <int32 name="param1" value="2" />
                        </parameters>
                    </module>

                    <!--Type Modules.Ninject.NinjectModule1 is a Ninject module and is a
                                             subclass of Ninject.Modules.NinjectModule-->
                    <module type="Modules.Ninject.NinjectModule1" assembly="modules">
                        <parameters>
                            <int32 name="param1" value="3" />
                        </parameters>
                    </module>
                </modules>

                <!--...-->
            </dependencyInjection>

            <!--...-->
        <iocConfiguration>


- Each child **module** element in **modules** element specifies a module type that should be either **IoC.Configuration** module (i.e., should either implement interface **IoC.Configuration.DiContainer.IDiModule** or be a subclass of **IoC.Configuration.DiContainer.ModuleAbstr** class), or should be a native module (e.g., **Autofac** or **Ninject** module).
- If the type specified by **module** is a native module (e.g., **Autofac** or **Ninject** module), then the type of the module should be assignable from one of the types specified by property **IoC.Configuration.DiContainer.IDiManager.ModuleType** in **IDiManager** objects listed in **iocConfiguration/diManagers/diManager** elements (see :doc:`./specifying-di-manager` for more details on specifying **IoC.Configuration.DiContainer.IDiManager** implementations).
- **IoC.Configuration** modules (i.e., modules that either implement interface **IoC.Configuration.DiContainer.IDiModule** or are subclasses of **IoC.Configuration.DiContainer.ModuleAbstr** class), and native modules (i.e., **Autofac** or **Ninject** modules), can be listed in any order in element **iocConfiguration/dependencyInjection/modules**.
- Constructor parameter values can be specified using **parameters** element, if the module does not have a default constructor (see :doc:`./constructor-parameters` for more details about constructor parameters).