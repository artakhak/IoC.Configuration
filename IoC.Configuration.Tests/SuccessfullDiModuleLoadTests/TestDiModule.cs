// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
using IoC.Configuration.DiContainer;
using IoC.Configuration.Tests.SuccessfullDiModuleLoadTests.TestClasses;

namespace IoC.Configuration.Tests.SuccessfullDiModuleLoadTests
{
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
}