=====================
Parameter Serializers
=====================

.. contents::
  :local:
  :depth: 2

The element **parameterSerializers** is used to de-serialize textual values used for type constructor parameters and property values, into objects of specific types.

Here is an example of **parameterSerializers** element.

.. sourcecode:: xml
     :linenos:

        <assemblies>
            <assembly name="OROptimizer.Shared" alias="oroptimizer_shared" />
            <assembly name="TestProjects.TestPluginAssembly1" alias="pluginassm1" plugin="Plugin1" />
            <assembly name="TestProjects.TestPluginAssembly2" alias="pluginassm1" plugin="Plugin2" />
            <!--Some more assemblies here...-->
        </assemblies>

        <parameterSerializers
                    serializerAggregatorType="OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator"
                    assembly="oroptimizer_shared">
            <!--
            Use parameters element to specify constructor parameters,
            if the type specified in 'serializerAggregatorType' attribute
            has non-default constructor.
            -->
            <!--<parameters>
            </parameters>-->
            <serializers>
                <parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerDouble"
                                     assembly="oroptimizer_shared" />
                <parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerLong"
                                     assembly="oroptimizer_shared" />
                <parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerInt"
                                     assembly="oroptimizer_shared" />
                <parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerShort"
                                     assembly="oroptimizer_shared" />
                <parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerByte"
                                     assembly="oroptimizer_shared" />
                <parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerBoolean"
                                     assembly="oroptimizer_shared" />
                <parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerDateTime"
                                     assembly="oroptimizer_shared" />
                <parameterSerializer type="OROptimizer.Serializer.TypeBasedSimpleSerializerString"
                                     assembly="oroptimizer_shared" />

                <parameterSerializer type="TestPluginAssembly1.Implementations.DoorSerializer"
                                     assembly="pluginassm1" />
                <parameterSerializer type="TestPluginAssembly2.Implementations.WheelSerializer"
                                     assembly="pluginassm2">
                    <!--
                    Use parameters element to specify constructor parameters,
                    if the type specified in 'type' attribute has non-default constructor.
                    -->
                    <!--<parameters>
                    </parameters>-->
                </parameterSerializer>
            </serializers>
        </parameterSerializers>

- The value of attribute **serializerAggregatorType** in element **parameterSerializers** should be full name of a type that implements the interface **OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator** in assembly **OROptimizer.Shared.dll** (the assembly is available in Nuget package **OROptimizer.Shared**).

  The default implementation of this interface is class **OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator** in assembly **OROptimizer.Shared.dll**.

- Attribute **assembly** is an assembly alias, that contains the type specified in attribute **serializerAggregatorType** (the assembly should be declared in some element **/iocConfiguration/assemblies/assembly**).
- Element **parameters** specifies values of constructor parameters for the type specified in attribute **serializerAggregatorType** (for details on constructor parameters reference :doc:`./constructor-parameters`).
- Element **serializers** lists the registered serializers using **parameterSerializer** elements. Each **parameterSerializer** element registers a serializer for specific type (see the **Element parameterSerializer** section below).

.. note::
    Some parameter serializers are provided by default, even if we do not list them under element **parameterSerializers/serializers**. Examples are serializers for some common types, such as parameter serializers for **System.Int32**, and **System.Double**, which are **OROptimizer.Serializer.TypeBasedSimpleSerializerInt** and **OROptimizer.Serializer.TypeBasedSimpleSerializerDouble**.

Element **parameterSerializer**
===============================

The element **parameterSerializer** registers a serializer for specific type. This element has two attributes, **type** and **assembly**, which are used to specify the full type name and the assembly for a serializer class that implements interface **OROptimizer.Serializer.ITypeBasedSimpleSerializer**.

Here is the definition of interface **OROptimizer.Serializer.ITypeBasedSimpleSerializer**:

.. sourcecode:: csharp
     :linenos:

        namespace OROptimizer.Serializer
        {
            /// <summary>
            ///     A simple serializer that serializes/de-serializes objects to and from strings.
            ///     The serialized string does not have any information about the type, so specific implementation de-serializes
            ///     specific type.
            ///     For example integer value 3 will be de-serialized from "3".
            /// </summary>
            public interface ITypeBasedSimpleSerializer
            {
                Type SerializedType { get; }
                bool TryDeserialize(string valueToDeserialize, out object deserializedValue);
                bool TrySerialize(object valueToSerialize, out string serializedValue);
            }
        }

.. note::
    The property **OROptimizer.Serializer.ITypeBasedSimpleSerializer.SerializedType** is used to pick a deserializer type from the serializers registered in element **parameterSerializers**.


Example 1
---------

.. note::
    Refer to :doc:`./constructor-parameters` for more details on **parameters** element to specify constructor parameter values in configuration file.

The **selfBoundService** element below is a definition of self-bound service for type **DynamicallyLoadedAssembly1.Implementations.SelfBoundService1** from configuration file.

.. sourcecode:: xml

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

The type **DynamicallyLoadedAssembly1.Implementations.SelfBoundService1** has a constructor with three parameters of types **System.Int32**, **System.Double**, and **DynamicallyLoadedAssembly1.Interfaces.IInterface1**.

- Since there is a **parameterSerializer** element (see example of **parameterSerializers** element above) for type **System.Int32** (i.e., **OROptimizer.Serializer.TypeBasedSimpleSerializerInt**), which de-serializes textual values into **System.Int32** values, **IoC.Configuration** will use **OROptimizer.Serializer.TypeBasedSimpleSerializerInt** to de-serialze the textual value "14" into a **System.Int32** value for the constructor parameter **param1**.
- Since there is a **parameterSerializer** element (see example of **parameterSerializers** element above) for type **System.Double** (i.e., **OROptimizer.Serializer.TypeBasedSimpleSerializerDouble**), which de-serializes textual values into **System.Double** values, **IoC.Configuration** will use **OROptimizer.Serializer.TypeBasedSimpleSerializerDouble** to de-serialze the textual value "15.3" into an **System.Double** value for the constructor parameter **param2**.
- The constructor parameter **param3** will be injected into constructor of **DynamicallyLoadedAssembly1.Implementations.SelfBoundService1**, using dependency injection mechanism, since **injectedObject** element is used for this parameter.

Example 2
---------

.. note::
    Refer to :doc:`./constructor-parameters` for more details on **parameters** element to specify constructor parameter values in configuration file.

The **service** element below defines type bindings for interface **TestPluginAssembly1.Interfaces.IRoom**.

.. sourcecode:: xml
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
                <injectedProperties>
                    <object name="Door2" type="TestPluginAssembly1.Interfaces.IDoor"
                            assembly="pluginassm1"
                            value="7,187.3" />
                </injectedProperties>
            </implementation>
        </service>

The constructor of type **TestPluginAssembly1.Implementations.Room** in element **implementation** has two constructor parameters named **door1** and **door2**, both of type **TestPluginAssembly1.Interfaces.IDoor**.

- Since there is a **parameterSerializer** element (see example of **parameterSerializers** element above) for type **TestPluginAssembly1.Implementations.DoorSerializer**, which de-serializes textual values into **TestPluginAssembly1.Interfaces.IDoor** values, **IoC.Configuration** will use **TestPluginAssembly1.Implementations.DoorSerializer** to de-serialze the textual value **"5,185.1"** into a **TestPluginAssembly1.Interfaces.IDoor** value to use for constructor parameter **door1**.
- The constructor parameter **door2** will be injected into constructor of **TestPluginAssembly1.Implementations.Room**, using dependency injection mechanism, since **injectedObject** element is used for this parameter.
- Property **TestPluginAssembly1.Implementations.Room.Door2** has a setter, and is of type **TestPluginAssembly1.Interfaces.IDoor** as well, therefore **IoC.Configuration** will use **TestPluginAssembly1.Implementations.DoorSerializer** as well, to de-serialze the textual value **"7,187.3"** into a **TestPluginAssembly1.Interfaces.IDoor** value and to assign this value to a property **TestPluginAssembly1.Implementations.Room.Door2** in bound object of type **TestPluginAssembly1.Implementations.Room**.