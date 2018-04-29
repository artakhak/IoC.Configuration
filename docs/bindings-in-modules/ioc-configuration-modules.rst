======================================================
Specifying Binding in IoC.Configuration Module Classes
======================================================

.. contents::
  :local:
  :depth: 2


Specifying Bindings in implementation of **IoC.Configuration.DiContainer.IDiModule**
====================================================================================
The service bindings are specified either in implementation of **Load()** method of interface **IoC.Configuration.DiContainer.IDiModule**,
or in overridden method **AddServiceRegistrations** in **IoC.Configuration.DiContainer.ModuleAbstr** (note, **IoC.Configuration.DiContainer.ModuleAbstr** is an implementation of interface **IoC.Configuration.DiContainer.IDiModule**).

Here is an example of specifying binding in overridden **Load()** method in a sub-class of **IoC.Configuration.DiContainer.ModuleAbstr**.

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
                        // Note, type of instance1 is the implementation type
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

    The examples below demonstrate dependency injection concepts supported by **IoC.Configuration** package. Services are resolved using an instance of **IoC.Configuration.DiContainer.IDiContainer**. To see how to create an instance of **IoC.Configuration.DiContainer.IDiContainer**, refer to sections :doc:`Loading XML Configuration File <../loading-ioc-from-xml-configuration-file/loading-xml-configuration-file>` and :doc:`Loading IoC Configuration from Modules <../loading-ioc-from-modules/index>`.
    In examples below, it is assumed that an instance of **IoC.Configuration.DiContainer.IDiContainer**, **diContainer**, was already creating using one of the techniques described in these sections.


Types of Binding
================


Binding to Self
---------------

This binding type can be used to specify that the type will be re-solved to an instance of the same type.

Example of this type of binding in overridden method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**:

.. sourcecode:: csharp

    protected override void AddServiceRegistrations()
    {
        //...
        Bind<Class1>().ToSelf()
              .SetResolutionScope(DiResolutionScope.Singleton);
    }

Example of resolving the service **Class1**.

.. sourcecode:: csharp

    private void SelfBoundServiceDemo(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        var implementation = diContainer.Resolve<Class1>();
        Assert.IsTrue(implementation.GetType() == typeof(Class1));
    }

Binding to Implementation Type
------------------------------

This binding type can be used to specify that the type will be re-solved to an instance of arbitrary type, that is either the same type, implementation or sub-class of the type being re-solved.

Example of this type of binding in overridden method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**:

.. sourcecode:: csharp

    protected override void AddServiceRegistrations()
    {
        //...
        Bind<IInterface2>().To<Interface2_Impl1>()
                           .SetResolutionScope(DiResolutionScope.Transient);
    }

Example of resolving the service **IInterface2**.

.. sourcecode:: csharp

    private void BindToTypeDemo(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        var implementation = diContainer.Resolve<IInterface2>();

        Assert.IsTrue(implementation.GetType() == typeof(Interface2_Impl1));

        // Validate that the implementation is an instance of the resolved type.
        Assert.IsInstanceOfType(implementation, typeof(IInterface2));
    }

Binding to an Instance of Resolved Type, Returned by a Delegate
---------------------------------------------------------------

Type is resolved to an object returned by a delegate.

Example of this type of binding in overridden method **IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations()**:

.. note::

    The delegate that is used to create an instance of resolved type accepts as an parameter an instance of **IoC.Configuration.DiContainer.IDiContainer**. This parameter can be used to resolve other types, when constructin the oobject to return.

.. sourcecode:: csharp

    protected override void AddServiceRegistrations()
    {
        //...
        Bind<IInterface6>().To(
        // The comiler will generate an error message if Interface6_Impl1 is not assignable to IInterface6.
        diContainer => new Interface6_Impl1(11, diContainer.Resolve<IInterface1>()));
    }

Example of resolving the service **IInterface6**.

.. sourcecode:: csharp

    private void BindToAValueReturnedByDelegate(IoC.Configuration.DiContainer.IDiContainer diContainer)
    {
        //...
        var implementation = diContainer.Resolve<IInterface6>();
        Assert.IsInstanceOfType(implementation, typeof(IInterface6));
    }