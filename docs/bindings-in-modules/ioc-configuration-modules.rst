==========================================
Type Bindings in IoC.Configuration Modules
==========================================

.. contents::
  :local:
  :depth: 2

Type Bindings in Implementation of **IoC.Configuration.DiContainer.IDiModule**
==============================================================================

Type bindings can be specified either in implementation of **Load()** method of interface **IoC.Configuration.DiContainer.IDiModule**, or in overridden method **AddServiceRegistrations** in **IoC.Configuration.DiContainer.ModuleAbstr**.

    .. note::
        **IoC.Configuration.DiContainer.ModuleAbstr** is an implementation of interface **IoC.Configuration.DiContainer.IDiModule**.

Here is an example of specifying type bindings in overridden **AddServiceRegistrations()** method in a sub-class of **IoC.Configuration.DiContainer.ModuleAbstr**.

.. sourcecode:: csharp

    public class TestDiModule : IoC.Configuration.DiContainer.ModuleAbstr
    {
        protected override void AddServiceRegistrations()
        {
            Bind<Class1>().ToSelf()
                          .SetResolutionScope(DiResolutionScope.Singleton);
            Bind<IInterface1>().To<Interface1_Impl1>()
                               .SetResolutionScope(DiResolutionScope.Singleton);
            Bind<Class2>().ToSelf()
                          .SetResolutionScope(DiResolutionScope.Transient);
            Bind<IInterface2>().To<Interface2_Impl1>()
                               .SetResolutionScope(DiResolutionScope.Transient);
            Bind<Class3>().ToSelf()
                          .SetResolutionScope(DiResolutionScope.ScopeLifetime);
            Bind<IInterface3>().To<Interface3_Impl1>()
                               .SetResolutionScope(DiResolutionScope.ScopeLifetime);
            Bind<Class4>().ToSelf()
                          .SetResolutionScope(DiResolutionScope.Singleton);
            Bind<Class4>().ToSelf()
                          .SetResolutionScope(DiResolutionScope.Transient);

            // The first binding should be with Singletone scope, and the second
            // should with Transient, so that we can test that the first binding was used.
            Bind<Class5>().OnlyIfNotRegistered()
                          .ToSelf().SetResolutionScope(DiResolutionScope.Singleton);
            Bind<Class5>().OnlyIfNotRegistered()
                          .ToSelf().SetResolutionScope(DiResolutionScope.Transient);

            // Only the first binding below will be registered.
            Bind<IInterface4>().OnlyIfNotRegistered().To<Interface4_Impl1>()
                               .SetResolutionScope(DiResolutionScope.Transient);
            Bind<IInterface4>().OnlyIfNotRegistered().To<Interface4_Impl2>()
                               .SetResolutionScope(DiResolutionScope.Singleton);

            // Both binding below will be registered
            Bind<IInterface5>().OnlyIfNotRegistered()
                               .To<Interface5_Impl1>()
                               .SetResolutionScope(DiResolutionScope.Transient);
            Bind<IInterface5>().To<Interface5_Impl2>()
                               .SetResolutionScope(DiResolutionScope.Singleton);

            // Test delegates and resolution using IDiContainer
            Bind<IInterface6>()
                .To(diContainer => new Interface6_Impl1(11, diContainer.Resolve<IInterface1>()));


            #region Test circular references

            Bind<ICircularReferenceTestInterface1>()
                .To<CircularReferenceTestInterface1_Impl>()
                .OnImplementationObjectActivated(
                    (diContainer, instance) =>
                        // Note, type of instance is the implementation type
                        // CircularReferenceTestInterface1_Impl. So we can use Property1 setter.
                        // ICircularReferenceTestInterface1 has only getter for Property1.
                        instance.Property1 = diContainer.Resolve<ICircularReferenceTestInterface2>())
                .SetResolutionScope(DiResolutionScope.Singleton);

            Bind<ICircularReferenceTestInterface2>().To<CircularReferenceTestInterface2_Impl>()
                                                    .SetResolutionScope(DiResolutionScope.Singleton);
            #endregion
        }
    }

.. note::
    The examples below demonstrate dependency injection concepts supported by **IoC.Configuration** package. Services are resolved using an instance of **IoC.Configuration.DiContainer.IDiContainer**. To see how to create an instance of **IoC.Configuration.DiContainer.IDiContainer**, refer to sections :doc:`../loading-ioc-configuration/loading-from-xml` and :doc:`../loading-ioc-configuration/loading-from-modules`.
    In examples below, it is assumed that an instance of **IoC.Configuration.DiContainer.IDiContainer**, **diContainer**, was already creating using one of the techniques described in these sections.


Types of Bindings
=================

Binding to Self
---------------

This binding type can be used to specify that the type will be re-solved to an instance of the same type.

Here is an example of this type of binding in overridden method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**:

.. sourcecode:: csharp

    protected override void AddServiceRegistrations()
    {
        Bind<Class1>().ToSelf()
              .SetResolutionScope(DiResolutionScope.Singleton);
    }

Example of resolving the service **Class1**. Note, in example we use **IoC.Configuration.DiContainer.IDiContainer** to resolve **Class1** for demonstration purposes, however normally we would just use a constructor injection.

.. code-block:: csharp

    private void SelfBoundServiceDemo(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        var implementation = diContainer.Resolve<Class1>();
        Assert.IsTrue(implementation.GetType() == typeof(Class1));
    }

Binding to Type
---------------

This binding type can be used to specify that type will be bound to some other type, that is either the same type, implementation or sub-class of the type being re-solved.

Example of this type of binding in overridden method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**:

.. code-block:: csharp

    protected override void AddServiceRegistrations()
    {
        //...
        Bind<IInterface2>().To<Interface2_Impl1>()
                           .SetResolutionScope(DiResolutionScope.Transient);
    }

Example of resolving the service **IInterface2**.

.. code-block:: csharp

    private void BindToTypeDemo(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        var implementation = diContainer.Resolve<IInterface2>();

        Assert.IsTrue(implementation.GetType() == typeof(Interface2_Impl1));

        // Validate that the implementation is an instance of the resolved type.
        Assert.IsInstanceOfType(implementation, typeof(IInterface2));
    }

Binding to a Delegate
---------------------

Type is resolved to an object returned by a delegate.

.. note::
    The delegate that is used to create an instance of resolved type accepts as a parameter an instance of **IoC.Configuration.DiContainer.IDiContainer**. This parameter can be used to resolve other types, when constructing the object to return.

Example of this type of binding in overridden method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**:

.. code-block:: csharp

    protected override void AddServiceRegistrations()
    {
        //...
        Bind<IInterface6>().To(
        // The compiler will generate an error message if object of type IInterface6 is not assignable from an object of type Interface6_Impl1.
        diContainer => new Interface6_Impl1(11, diContainer.Resolve<IInterface1>()));
    }

Example of resolving the service **IInterface6**.

.. code-block:: csharp

    private void BindToAValueReturnedByDelegate(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        //...
        var implementation = diContainer.Resolve<IInterface6>();
        Assert.IsInstanceOfType(implementation, typeof(IInterface6));
    }

Specifying Resolution Scope
---------------------------

For more details on resolution scope, refer to section :doc:`../resolving-types/resolution-scopes`.
Here we will just mention that all three resolution scopes are supporetd in **IoC.Configuration** modules.

Here are some examples on how to specify the resolution scope in overridden method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**.

.. code-block:: csharp

    protected override void AddServiceRegistrations()
    {
        Bind<Class1>().ToSelf().SetResolutionScope(DiResolutionScope.Singleton);
        Bind<IInterface4>().To<Interface4_Impl1>().SetResolutionScope(DiResolutionScope.Transient);
        Bind<IInterface3>().To<Interface3_Impl1>().SetResolutionScope(DiResolutionScope.ScopeLifetime);
    }

Property Injection and Circular References
==========================================

The most common dependency injection type is a constructor injection, when dependency injection container creates objects and injects them into constructor of an object being resolved (this process is done recursively).

However, there are scenarios when two types reference each other. In this case constructor injection might fail. For example if type **TypeA** is specified as a constructor parameter in type **TypeB** and **TypeB** is specified as a constructor parameter in type **TypeA**, the dependency injection container will not be able to create an instance of **TypeA**, since it will need to create an instance of type **TypeB**, which requires creating an instance of type **TypeA**.

In such cases, property injection can be used to re-solve circular references. In this example type **TypeB** can be specified as a constructor parameter in type **TypeA**, and type **TypeA** can be a type of property **TypeB.TypeAProperty**, which will be initialized after the DI container created both types.

Here is an example of how property injection can be implemented in overridden **AddServiceRegistrations()** method in a sub-class of **IoC.Configuration.DiContainer.ModuleAbstr**:

In this example, the constructor of type **CircularReferenceTestInterface2_Impl** has a parameter of type **ICircularReferenceTestInterface1**, and the implementation of **ICircularReferenceTestInterface1**, **CircularReferenceTestInterface1_Impl**, has a setter property **Property1** of type **ICircularReferenceTestInterface2**.

.. note::
    The setter property used for property injection needs to be declared in implementation only.

.. code-block:: csharp

    public class TestDiModule : IoC.Configuration.DiContainer.ModuleAbstr
    {
        protected override void AddServiceRegistrations()
        {
            Bind<ICircularReferenceTestInterface1>()
                        .To<CircularReferenceTestInterface1_Impl>()
                        .OnImplementationObjectActivated(
                            (diContainer, instance) =>
                             // Note, type of parameter 'instance' is the implementation type
                             // CircularReferenceTestInterface1_Impl. So we can use Property1 setter in
                             // CircularReferenceTestInterface1_Impl only and not in ICircularReferenceTestInterface1.
                             // ICircularReferenceTestInterface1 has only getter for Property1.
                             instance.Property1 = diContainer.Resolve<ICircularReferenceTestInterface2>())
                        .SetResolutionScope(DiResolutionScope.Singleton);

            Bind<ICircularReferenceTestInterface2>().To<CircularReferenceTestInterface2_Impl>()
                                                    .SetResolutionScope(DiResolutionScope.Singleton);
        }
    }