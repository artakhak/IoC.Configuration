=================================
Using Types in Configuration File
=================================

Many configuration elements reference **C#** types.

Either the full type name of the element should be specified using a type attribute (the attribute name might be different depending on element),
or type alias should be specified, to reference a type declared in element under element **/iocConfiguration/typeDefinitions/typeDefinitions** (see below for more details).

Some examples are:

- <service type="DynamicallyLoadedAssembly1.Interfaces.IInterface1">
- <service type="DynamicallyLoadedAssembly1.Interfaces.IInterface2 assembly="shared_services">
- <service typeRef="IInterface1">

If the element uses the full type name, an optional attribute **assembly** can be used to specify the assembly, where the type is.
     .. note::
        Refer to :doc:`./assemblies-and-probing-paths` for more details on assemblies in configuration file.

**IoC.Configuration** looks up the type in all the assemblies under element **/iocConfiguration/assemblies**.

.. note::
   Refer to :doc:`../sample-files/IoCConfiguration_GenericTypesAndTypeReUse` for more examples of using types in configuration file.

Generic Types in Configuration File
===================================

To reference generic types, list the comma separated generic type parameters within opening and closing square brackets (i.e., []) after the type name.

Some examples are:

.. code-block:: xml

    <!--This type is similar to C# type
    SharedServices.Implementations.Generic.Generic1_1<SharedServices.Implementations.Interface1_Impl1> -->
    <implementation
        type="SharedServices.Implementations.Generic.Generic1_1[SharedServices.Implementations.Interface1_Impl1]"
        scope="singleton" />


.. code-block:: xml

    <!--This type is similar to C# type
    SharedServices.Interfaces.Generic.IGeneric2_1<SharedServices.Implementations.Generic.Generic3_1<System.Int32> -->
    <service
        type="SharedServices.Interfaces.Generic.IGeneric2_1[SharedServices.Implementations.Generic.Generic3_1[System.Int32]]" >

Array Types in Configuration File
=================================

Array types can be specified by appending character **#** after the array item type name.

Example is:

    .. code-block:: xml

        <!--The type definition below is similar to C# type SharedServices.Interfaces.IInterface1[]-->
        <service type="SharedServices.Interfaces.IInterface1#" >
            <!--Some implementation for service SharedServices.Interfaces.IInterface1[] goes here.-->
        </service>

Re-Using Types
==============

To avoid specifying the full type name in multiple elements in configuration file, we can define the type in **/iocConfiguration/typeDefinitions/typeDefinition** element, and reference the type using tye type alias in other elements.

Here is an example of declaring a type **System.Collections.Generic.IEnumerable<SharedServices.Interfaces.IInterface1>** with alias **IEnumerableOf_IInterface1** in **typeDefinition** element:

    .. code-block:: xml

        <typeDefinitions>
            <typeDefinition
                alias="IEnumerableOf_IInterface1"
                type="System.Collections.Generic.IEnumerable[SharedServices.Interfaces.IInterface1]" />
        </typeDefinitions>

Here is an example of referencing the type with alias **IEnumerableOf_IInterface1** declared in **typeDefinition** element:

    .. code-block:: xml

        <service typeRef="IEnumerableOf_IInterface1">
            <!--Some implementation for service
            System.Collections.Generic.IEnumerable<SharedServices.Interfaces.IInterface1> goes here.-->
        </service>