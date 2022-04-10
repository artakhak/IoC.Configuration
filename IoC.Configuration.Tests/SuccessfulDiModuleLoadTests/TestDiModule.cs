// This file is used in documentation as demo
using IoC.Configuration.DiContainer;
using System.Collections.Generic;
using IoC.Configuration.Tests.SuccessfulDiModuleLoadTests.TestClasses;

namespace IoC.Configuration.Tests.SuccessfulDiModuleLoadTests;

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

        // The first binding should be with Singleton scope, and the second
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

        // Test delegates that return a collection using IDiContainer
        Bind<IEnumerable<IInterface7>>().To(x => new List<IInterface7>()
        {
            new Interface7_Impl1(10),
            new Interface7_Impl1(11)
        }).SetResolutionScope(DiResolutionScope.Singleton);

        Bind<IInterface8>().To<Interface8_Impl1>().SetResolutionScope(DiResolutionScope.Singleton);

        #region Test circular references

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
        #endregion
    }
}