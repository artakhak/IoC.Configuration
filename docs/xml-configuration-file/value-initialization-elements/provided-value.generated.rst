=========================
**providedValue** element
=========================

The ``providedValue`` element is used to inject values that are resolved at runtime through custom logic rather than being hardcoded in the XML or resolved directly from the DI container. This is particularly useful for injecting external dependencies like loggers, session data, or environment-specific values.

.. note::
    Refer to :doc:`../../sample-files/IoCConfiguration_providedValue.generated` for more examples on **providedValue** element as well as related unit tests in `ProvidedValue tests <https://github.com/artakhak/IoC.Configuration/tree/master/IoC.Configuration.Tests/ProvidedValue>`_ for more examples.

How it Works
------------

When the configuration parser encounters a ``providedValue`` element, it attempts to resolve the value using a list of ``IValueProvider`` implementations passed during the initialization of the ``FileBasedConfiguration``.

Setup in C#
~~~~~~~~~~~

To use ``providedValue``, you must initialize the ``ValueProviders`` property in ``IoC.Configuration.DiContainerBuilder.FileBased.FileBasedConfigurationParameters``.

.. code-block:: csharp

    // Example setup for ValueProviders
    var configurationParameters = new FileBasedConfigurationParameters(
        configurationFileContentsProvider,
        entryAssemblyFolder,
        loadedAssemblies)
    {
        // Initialize the list of value providers
        ValueProviders = new List<IValueProvider>
        {
            new MyCustomValueProvider(logger)
        }
    };

    // Use configurationParameters to build the DiContainer

The ``IValueProvider`` interface requires implementing ``TryResolveValue``, which receives information about the target (type, name, and whether it's a parameter, property, or setting) to provide the appropriate object.

XML Configuration Examples
--------------------------

The ``providedValue`` element can be used for module parameters, service constructor arguments, properties, and settings.

1. Injection into Modules
~~~~~~~~~~~~~~~~~~~~~~~~~

You can pass externally provided values into DI modules. This is useful for passing a pre-configured logger to a module that needs to register it.

.. code-block:: xml

    <module type="Modules.IoC.DiModule3">
        <parameters>
            <!-- The value for 'diModule3_param1' will be resolved by a ValueProvider -->
            <providedValue name="diModule3_param1" type="System.Int32" />
        </parameters>
    </module>

2. Injection into Services and Properties
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

It can be used within ``<services>`` to inject dependencies into constructors or via property injection.

.. code-block:: xml

    <service type="IoC.Configuration.Tests.ProvidedValue.TestClasses.ITestInterface1">
        <implementation type="IoC.Configuration.Tests.ProvidedValue.TestClasses.TestInterface1_Impl1">
            <parameters>
                <providedValue name="logger" type="OROptimizer.Diagnostics.Log.ILog" />
            </parameters>
            <injectedProperties>
                <providedValue name="LoggerInjectedIntoProperty" type="OROptimizer.Diagnostics.Log.ILog" />
            </injectedProperties>
        </implementation>
    </service>

3. Usage in Settings
~~~~~~~~~~~~~~~~~~~~

``providedValue`` can also define application settings that are resolved at runtime.

.. code-block:: xml

    <settings>
        <providedValue name="Logger" type="OROptimizer.Diagnostics.Log.ILog" />
    </settings>

4. Plugin Support
~~~~~~~~~~~~~~~~~

Plugins can also leverage provided values for their own initialization and settings.

.. code-block:: xml
    :linenos:
    
    <pluginSetup plugin="Plugin1">
        <!--The type in pluginImplementation should be non-abstract class 
            that implements IoC.Configuration.IPlugin and which has a public constructor-->
        <pluginImplementation type="TestPluginAssembly1.Implementations.Plugin1">
            <parameters>
                <providedValue name="param1" type="System.Int64"/>
            </parameters>
            <injectedProperties>
                <providedValue name="Property2" type="System.Int64"/>
            </injectedProperties>
        </pluginImplementation>
        <settings>
            <providedValue name="Int32Setting1" type="System.Int32"/>
            <int64 name="Int64Setting1" value="25" />
        </settings>
        <dependencyInjection>
            <modules>            
                <module type="ModulesForPlugin1.Autofac.AutofacModule1" >
                    <parameters>
                        <providedValue name="param1" type="System.Int32"/>
                    </parameters>
                </module>
                <module type="ModulesForPlugin1.IoC.DiModule1" >
                    <parameters>
                        <providedValue name="param1" type="System.Int32"/>
                    </parameters>
                </module>
            </modules>
            
            <services>
                <service type="TestPluginAssembly1.Interfaces.IDoor">
                    <implementation scope="singleton" type="TestPluginAssembly1.Implementations.Door">
                        <parameters>
                            <providedValue name="color" type="System.Int32"/>
                        </parameters>
                        <injectedProperties>
                            <providedValue name="Height" type="System.Double" />
                        </injectedProperties>
                    </implementation>
                </service>
            </services>
        </dependencyInjection>
    </pluginSetup>