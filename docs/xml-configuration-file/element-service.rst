===================
Element **service**
===================

.. contents::
  :local:
  :depth: 2

Element **service** is used to bind a type specified in attributes **type** and **assembly** to one ore more types specified in **implementation** child elements.


Single Implementation
=====================

An example of **service** that binds type **SharedServices.Interfaces.IInterface4** in assembly with alias **shared_services** to type **SharedServices.Implementations.Interface4_Impl1** is shown below.

.. code-block:: xml

    <!--...-->
    <services>
        <!--...-->
        <service type="SharedServices.Interfaces.IInterface4" assembly="shared_services">
            <implementation type="SharedServices.Implementations.Interface4_Impl1"
                            assembly="shared_services"
                            scope="singleton">
            </implementation>
        </service>
        <!--...-->
    </services>

An instance of type **SharedServices.Implementations.Interface4_Impl1** will be injected as a constructor parameter or into properties when interface **SharedServices.Interfaces.IInterface4** is requested.

Here is an example of injecting **SharedServices.Implementations.Interface4_Impl1** as a constructor parameter **interface4**.

.. code-block:: csharp

    public class TestConstructorInjection
    {
        // An instance of type SharedServices.Implementations.Interface4_Impl1 will
        // be injected for constructor parameter interface4.
        public TestConstructorInjection(SharedServices.Interfaces.IInterface4 interface4)
        {
        }
    }

Multiple Implementations
========================

If multiple **implementation** elements are specified under **service** element, the type specified in element **service** will be bound to multiple types.
In such a cases we should use **System.Collections.Generic.IEnumerable<TService>**.

An example of **service** that binds a type **SharedServices.Interfaces.IInterface8** in assembly with alias **shared_services** to two types, **SharedServices.Implementations.Interface8_Impl1** and **SharedServices.Implementations.Interface8_Impl2** is shown below.

.. code-block:: xml

    <!--...-->
    <services>
        <service type="SharedServices.Interfaces.IInterface8"
                 assembly="shared_services">
            <implementation type="SharedServices.Implementations.Interface8_Impl1"
                            assembly="shared_services"
                            scope="singleton">
            </implementation>

            <implementation type="SharedServices.Implementations.Interface8_Impl2"
                            assembly="shared_services"
                            scope="transient">
            </implementation>
        </service>
        <!--...-->
    </services>

Here is an example of injecting instances of types **SharedServices.Implementations.Interface8_Impl1** and **SharedServices.Implementations.Interface8_Impl2** for parameter **interface8Resolutions** of type **System.Collections.Generic.IEnumerable<SharedServices.Interfaces.IInterface8>**.
In this example, the injected collection **interface8Resolutions** will have two items. The first item will be of type **SharedServices.Implementations.Interface8_Impl1** and second item will be of type **SharedServices.Implementations.Interface8_Impl2**.

.. code-block:: csharp

    public class TestConstructorInjectionForMultipleBindings
    {
        public TestConstructorInjection(
                IEnumerable<SharedServices.Interfaces.IInterface8> interface8Resolutions)
        {
        }
    }

Binding Scope
=============

Attribute **scope** in element **implementation** under element **service** is used to specify binding resolution scope for resolved types (see :doc:`../resolving-types/resolution-scopes` for more details).
The value of this attribute can be one of the following: **singleton**, **transient**, and **scopeLifetime**.