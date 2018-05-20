======================
Constructor Parameters
======================

.. contents::
  :local:
  :depth: 2

Element **parameters** is used to specify constructor parameter values for a type specified in XML configuration file.

Eement **parameters** has one child element per constructor parameter.

Child elements are **byte**, **int16**, **int32**, **int64**, **double**, **boolean**, **datetime**, **string**, **object**, and **injectedObject**.

- Elements, **byte**, **int16**, **int32**, **int64**, **double**, **boolean**, **datetime**, **string**, are straightforward, and are used to specify constructor parameters of the same type (**byte**, **System.Int32**, **string**, etc).
- Element **object** is used to provide a constructor parameter value of arbitrary type using attributes **type** and **assembly**, as well as attribute **value**. The value in attribute **value** will be de-serialized into an instance of type specified in attributes **type** and **assembly**, using a serializer registered in element **iocConfiguration/parameterSerializers** for the type.

     .. note::
        Refer to :doc:`./parameter-serializers` for more details on parameter serializers.

- Element **injectedObject** is used to specify a constructor parameter that should be injected using the dependency injection mechanism.

    .. note::
        Child elements **injectedObject** are valid based on context, where **parameters** element is used. For example this element can be used when specifying service bindings, as shown in examples below, but cannot be used in **settings** element.

Example 1
=========

In example below, service of type **DynamicallyLoadedAssembly1.Implementations.SelfBoundService1** will be bound to an instance of the same type, which will be created using a constructor with three parameters of types **int**, **double**, and **DynamicallyLoadedAssembly1.Interfaces.IInterface1**.

- Parameter **param1** of type **System.Int32** will be initialized from textual value **14** using the parameter serializer **OROptimizer.Serializer.TypeBasedSimpleSerializerInt** in assembly **OROptimizer.Shared**.
- Parameter **param2** of type **System.Double** will be initialized from textual value **15.3** using the parameter serializer **OROptimizer.Serializer.TypeBasedSimpleSerializerDouble** in assembly **OROptimizer.Shared**.
- Parameter **param3** of type **DynamicallyLoadedAssembly1.Interfaces.IInterface1** will be injected into constructor of **DynamicallyLoadedAssembly1.Implementations.SelfBoundService1**.

.. code-block:: xml
    :linenos:

        <selfBoundService type="DynamicallyLoadedAssembly1.Implementations.SelfBoundService1"
                                      assembly="dynamic1"
                                      scope="singleton">
            <parameters>
                <int32 name="param1" value="14" />
                <double name="param2" value="15.3" />
                <injectedObject name="param3" type="DynamicallyLoadedAssembly1.Interfaces.IInterface1"
                                assembly="dynamic1" />
            </parameters>
        </selfBoundService>

Example 2
=========

In example below, service of type **TestPluginAssembly1.Interfaces.IRoom** will be bound to an instance of type **TestPluginAssembly1.Implementations.Room**, which will be created using a constructor that has two parameters, **door1** and **door2**, of type **TestPluginAssembly1.Interfaces.IDoor**.

The first parameter **door1** will be de-serialized from text **5,185.1**, using a serializer **TestPluginAssembly1.Implementations.DoorSerializer**, found under element **iocConfiguration/parameterSerializers** for type **TestPluginAssembly1.Interfaces.IDoor**.
    .. note:
        Refer to :doc:`./parameter-serializers` for more details on parameter serializers.

The second parameter **door2** will be injected into constructor of **TestPluginAssembly1.Implementations.Room**.

.. code-block:: xml
    :linenos:

        <service type="TestPluginAssembly1.Interfaces.IRoom" assembly="pluginassm1">
            <implementation type="TestPluginAssembly1.Implementations.Room"
                            assembly="pluginassm1"
                            scope="transient">
                <parameters>
                    <object name="door1" type="TestPluginAssembly1.Interfaces.IDoor"
                            assembly="pluginassm1"
                            value="5,185.1" />
                    <injectedObject name="door2" type="TestPluginAssembly1.Interfaces.IDoor"
                                    assembly="pluginassm1" />
                </parameters>
            </implementation>
        </service>

Example 3
=========

This example is similar to *Example 2* above, however **parameters** element is missing under the implementation type **TestPluginAssembly1.Implementations.Room**. Since no **parameters** element is provided, an instance of type **TestPluginAssembly1.Implementations.Room** will be injected using dependency injection mechanism, rather than using a specific constructor.

  .. note::
    If a **parameters** element is provided without any child parameters, an instance of type will be created using the default parameter-less constructor. Therefore the type is expected to have a default constructor.
    To use dependency injection mechanism to construct an instance of type, one should omit the **parameters** element, instead of providing an empty **parameters** element.

.. code-block:: xml
    :linenos:

        <service type="TestPluginAssembly1.Interfaces.IRoom" assembly="pluginassm1">
            <implementation type="TestPluginAssembly1.Implementations.Room"
                            assembly="pluginassm1"
                            scope="transient">
            </implementation>
        </service>