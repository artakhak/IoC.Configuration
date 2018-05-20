============================
Element **selfBoundService**
============================

Element **selfBoundService** is used to bind a type specified in attributes **type** and **assembly** to itself.

An example of **selfBoundService** element to bind type **DynamicallyLoadedAssembly1.Implementations.SelfBoundService3** in assembly with alias **dynamic1** to itself is shown below.

.. code-block:: xml

    <!--...-->
    <services>
        <!--...-->
        <selfBoundService type="DynamicallyLoadedAssembly1.Implementations.SelfBoundService3"
                          assembly="dynamic1"
                          scope="scopeLifetime">
        </selfBoundService>
        <!--...-->
    </services>

An instance of type **DynamicallyLoadedAssembly1.Implementations.SelfBoundService3** will be injected as a constructor parameter or into properties when type **DynamicallyLoadedAssembly1.Implementations.SelfBoundService3** is requested.

Here is an example of injecting **DynamicallyLoadedAssembly1.Implementations.SelfBoundService3** as a constructor parameter **selfBoundService3**.

.. code-block:: csharp

    public class TestConstructorInjection
    {
        public TestConstructorInjection(
                DynamicallyLoadedAssembly1.Implementations.SelfBoundService3 selfBoundService3)
        {
        }
    }

Binding Scope
=============

Attribute **scope** in element **implementation** under element **service** is used to specify binding resolution scope for resolved types (see :doc:`../resolving-types/resolution-scopes` for more details).
The value of this attribute can be one of the following: **singleton**, **transient**, and **scopeLifetime**.