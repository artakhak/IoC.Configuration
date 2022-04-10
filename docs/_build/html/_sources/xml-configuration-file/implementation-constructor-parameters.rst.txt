==========================================
Implementation Type Constructor Parameters
==========================================

.. contents::
   :local:
   :depth: 2

Normally we do not need to specify constructor parameters when specifying type bindings, since the constuctor parameters will be injected. However, in some cases we want to explicitly specify constructor parameter values in bound type.

Here are some of the cases when explicitly specifying constructor parameters might be useful:

- The bound type has multiple constructors, and we want to explicitly specify which constructor should be used.
- The bound type constructor has some parameters, which cannot be injected since no bindings are provided for the types of these parameters.
- We want to explicitly specify the constructor parameter values (say for primitive types, such as System.Int32, System.Double, etc), instead of relying on **IoC** container to inject these constructor parameter values.

.. note::
    Normally, if there are multiple constructors, the **IoC** container will pick the constructor with largest number of parameters. This pattern is true for **Autofac** and **Ninject**, however might be different for other implementations.

Element parameters
==================

- Use **parameters** in element **implementation** or **selfBoundService** to provide constructor parameter values.
- If **parameters** is used without any values (see Example 1 below), than the default constructor will be used to construct an instance of bound type, even if there are other constructors. In this case an exception will be thrown if the bound type does not have a default constructor. To let the **IoC** pick the constructor, do not use any **parameters** element in **implementation** and **selfBoundService** elements.
- **IoC.Configuration** will use parameter serializers specified in element **iocConfiguration/parameterSerializers** to de-serialize the values of parameters specified in child elements of element **parameters** (see :doc:`./parameter-serializers`).
- When the **IoC.Configuration** uses reflection to find the constructor using parameter types specified under **parameters** element. If no constructor is found, and exception will be thrown.

Example 1
=========

In the example below, type **DynamicallyLoadedAssembly1.Implementations.SelfBoundService1** is bound to itself.

- When injecting an instance of **DynamicallyLoadedAssembly1.Implementations.SelfBoundService1**, the object will be constructed using the constructor with three parameters of types **System.Int32**, **System.Double**, and **DynamicallyLoadedAssembly1.Interfaces.IInterface1**.
- The first and second parameter values will be de-serialized from textual values **14** and **15.3**.
- The third parameter value will be injected by **IoC.Configuration**, since **injectedObject** is used for this parameter. A binding for **DynamicallyLoadedAssembly1.Interfaces.IInterface1** should be specified in XML configuration file or in modules being loaded.

.. note::
    **IoC.Configuration** will automatically register a self bound service for a type specified in element **injectedObject**, if the type is not an abstract type or an interface, and if it is not already registered in configuration file. Therefore, if in example below we replace **DynamicallyLoadedAssembly1.Interfaces.IInterface1** with **DynamicallyLoadedAssembly1.Interfaces.IInterface1_Implementation** (i.e., non-abstract implementation of **DynamicallyLoadedAssembly1.Interfaces.IInterface1**), there will be no need to provide a binding for **DynamicallyLoadedAssembly1.Interfaces.IInterface1_Implementation**.

.. note::
     Using **injectedObject**, we can specify a type other than a type registered for interface **DynamicallyLoadedAssembly1.Interfaces.IInterface1** (i.e., the type of parameter **param3**). In other words, no matter what bindings are specified for interface **DynamicallyLoadedAssembly1.Interfaces.IInterface1**, the object injected for parameter **param3** will be of type specified in **injectedObject** element.

.. code-block:: xml

    <selfBoundService type="DynamicallyLoadedAssembly1.Implementations.SelfBoundService1"
                      assembly="dynamic1"
                      scope="singleton">
        <parameters>
            <int32 name="param1" value="14" />
            <double name="param2" value="15.3" />
            <injectedObject name="param3"
                            type="DynamicallyLoadedAssembly1.Interfaces.IInterface1"
                            assembly="dynamic1" />
        </parameters>
    </selfBoundService>

Example 2
=========

In the example below, interface **SharedServices.Interfaces.IRoom** is bound to class **SharedServices.Implementations.Room**.

When injecting an instance of **SharedServices.Interfaces.IRoom**, an object of type **SharedServices.Implementations.Room** will be constructed using a constructor with two parameter, both of type **SharedServices.Interfaces.IDoor**.

- The first parameter value will be de-seriazed from string **"5,185.1"** provided in attribute **value** in element **object**. A parameter serializer for type **SharedServices.Interfaces.IDoor** should be specified in element **iocConfiguration/parameterSerializers/serializers** (see :doc:`./parameter-serializers` for more details).
- The second parameter value will be injected by **IoC.Configuration**, since **injectedObject** is used for parameter value.

.. note::
    **IoC.Configuration** will automatically register a self bound service for a type specified in element **injectedObject**, if the type is not an abstract type or an interface, and if it is not already registered in configuration file. Therefore, no need to register a binding for type **SharedServices.Interfaces.OakDoor** used in **injectedObject** for parameter **door2**, since this type is non-abstract and non-interface.

.. note::
     Using **injectedObject**, we can specify a type other than a type registered for interface **SharedServices.Interfaces.IDoor** (i.e., the type of parameter **door2**). In other words, no matter what bindings are specified for interface **SharedServices.Interfaces.IDoor**, the object injected for parameter **door2** will be of type specified in **injectedObject** element.

.. code-block:: xml

    <service type="SharedServices.Interfaces.IRoom" assembly="shared_services">
        <implementation type="SharedServices.Implementations.Room"
                        assembly="shared_services"
                        scope="transient">
            <parameters>
                <object name="door1" type="SharedServices.Interfaces.IDoor"
                        assembly="shared_services"
                        value="5,185.1" />
                <injectedObject name="door2" type="SharedServices.Interfaces.OakDoor"
                                assembly="shared_services" />
            </parameters>
        </implementation>
    </service>

Example 3
=========

In the example below, a default constructor will be used to construct an instance of **SharedServices.Implementations.Interface8_Impl1**, even though type **SharedServices.Implementations.Interface8_Impl1** has also a non default constructor. The reason the default constructor is picked is that empty **parameters** element is used under element **implementation**.

.. code-block:: xml

    <service type="SharedServices.Interfaces.IInterface8"
                     assembly="shared_services">
        <implementation type="SharedServices.Implementations.Interface8_Impl1"
                                assembly="shared_services"
                                scope="singleton">

            <parameters>
            </parameters>
        </implementation>
    </service>

In the example below, non-default constructor will be used to construct an instance of **SharedServices.Implementations.Interface8_Impl1**, since no **parameters** element is used, and the type **SharedServices.Implementations.Interface8_Impl1** has both parameter-less constructor as well as constructor with parameters.

.. code-block:: xml

    <service type="SharedServices.Interfaces.IInterface8"
                     assembly="shared_services">
        <implementation type="SharedServices.Implementations.Interface8_Impl1"
                                assembly="shared_services"
                                scope="singleton">
        </implementation>
    </service>