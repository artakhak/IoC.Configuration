=====================
Specifying DI Manager
=====================

- The XML configuration file has a required element **diManagers**, which specifies a types that implements interface **IoC.Configuration.DiContainer.IDiManager**. The **IoC.Configuration.DiContainer.IDiManager** implementations are specified in child **diManager** elements.
- Also, **diManagers** element has an attribute **activeDiManagerName**, that specifies which **IoC.Configuration.DiContainer.IDiManager** will be used to handle IoC service bindings and resolutions, as well as some other dependency injection related behaviour (such as valid module types, etc).
    .. note::
        Currently two implementations of **IoC.Configuration.DiContainer.IDiManager** are available: **IoC.Configuration.Ninject.NinjectDiManager** and **IoC.Configuration.Autofac.AutofacDiManager** in Nuget packages `IoC.Configuration.Ninject <https://www.nuget.org/packages/IoC.Configuration.Ninject/>`_ and `IoC.Configuration.Autofac <https://www.nuget.org/packages/IoC.Configuration.Autofac/>`_
- The selected **IoC.Configuration.DiContainer.IDiManager** implementation (e.g., **IoC.Configuration.Ninject.NinjectDiManager**, **IoC.Configuration.Autofac.AutofacDiManager**) handles type bindings that are specified in **IoC.Configuration** modules, as well as type resolutions.

Example of this element is shown below. To switch between **Ninject** and **Aurofac** containers, one needs to set the value of **activeDiManagerName** to either **Ninject** or **Aurofac**.

.. code-block:: xml
    :linenos:

        <diManagers activeDiManagerName="Autofac">
            <diManager name="Ninject"
                    type="IoC.Configuration.Ninject.NinjectDiManager"
                    assembly="ninject_ext">
                <!--
                Use parameters element to specify constructor parameters,
                if the type specified in 'type' attribute
                has non-default constructor.-->
                <!--<parameters>
                </parameters>-->
            </diManager>
            <diManager name="Autofac"
                     type="IoC.Configuration.Autofac.AutofacDiManager"
                     assembly="autofac_ext">
            </diManager>
        </diManagers>

