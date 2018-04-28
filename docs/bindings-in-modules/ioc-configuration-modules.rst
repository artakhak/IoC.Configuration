======================================================
Specifying Binding in IoC.Configuration Module Classes
======================================================

.. contents::
  :local:
  :depth: 2


Specifying Bindings in implementation of **IoC.Configuration.DiContainer.IDiModule**
====================================================================================
The service bindings are specified in implementation of **Load()** method of interface a class that implements **IoC.Configuration.DiContainer.IDiModule**.
The easiest way to do this, is to sub-class from **IoC.Configuration.DiContainer.ModuleAbstr**, since it already implements all the methods in
**IoC.Configuration.DiContainer.IDiModule**, except the method **Load()**.

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


Types of Binding
================

Binding to Self
---------------

This binding type can be used to specify that the service will be re-solved to the same class.

