==================
Property Injection
==================

.. contents::
   :local:
   :depth: 2

- **IoC.Configuration** allows property injection in XML configuration file (for property injection in modules see :doc:`../bindings-in-modules/ioc-configuration-modules`).
- The property value is injected using **injectedProperties** element under element **service/implementation** or **selfBoundService**.
- Normally we should rely on constructor injection. However property injection can be used to handle circular references, when two types reference each other.
- Property injection is done after the object is constructed. Therefore, the values of properties set by property injection will override the values set by constructor.

Example
=======

In the example below interface **SharedServices.Interfaces.IInterface2** is bound to type **SharedServices.Implementations.Interface2_Impl3**.

- When type **SharedServices.Interfaces.IInterface2** is injected into another type via a constructor or property injection, an instance of **SharedServices.Implementations.Interface2_Impl3** will be created and its properties will be set the following way:

    - Property **Property1** will be set to a **System.Double** value de-serialized from textual value **148.3**.
    - Property **Property2** will be injected by **IoC.Configuration**, since element **injectedObject** is used for this property.
    - Property **Wheel1** will be set to an instance of **SharedServices.Interfaces.IWheel** de-serialized from a textual value **27,45**.
        .. note::
            A parameter serializer for type **SharedServices.Interfaces.IWheel** should exist for type **SharedServices.Interfaces.IWheel** under element **iocConfiguration/parameterSerializers/serializers**.

.. note::
    **IoC.Configuration** will automatically register a self bound service for a type specified in element **injectedObject**, if the type is not an abstract type or an interface, and if it is not already registered in configuration file. Therefore, no need to register a binding for type **SharedServices.Implementations.Interface3_Impl3** used in **injectedObject** for property **Property2**, since this type is a non-abstract and non-interface.

.. note::
     Using **injectedObject**, we can specify a type other than a type registered for type of property **Property2** somewhere else. By using element **injectedObject** we explicitly state the type of the object that should be injected no matter what types are registered for the type of the property.

.. note::
    The implementation type **SharedServices.Implementations.Interface2_Impl3** should have a setter for property **Property2**, which should be assignable from type **SharedServices.Implementations.Interface3_Impl3**. However, the setter property should be in type **SharedServices.Implementations.Interface2_Impl3** and not in type specified in **service** element.

.. code-block:: xml

    <service type="SharedServices.Interfaces.IInterface2" assembly="shared_services">
        <implementation type="SharedServices.Implementations.Interface2_Impl3"
                                            assembly="shared_services"
                                            scope="singleton">
            <injectedProperties>
                <double name="Property1" value="148.3" />

                <injectedObject name="Property2"
                                type="SharedServices.Implementations.Interface3_Impl3"
                                assembly="shared_services" />
                <object name="Wheel1"
                        type="SharedServices.Interfaces.IWheel"
                        assembly="shared_services" value="27,45" />
            </injectedProperties>
        </implementation>
    <service>