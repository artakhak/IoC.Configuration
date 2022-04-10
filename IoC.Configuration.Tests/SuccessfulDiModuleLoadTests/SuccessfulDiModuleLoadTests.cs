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
using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.DiContainerBuilder.FileBased;
using IoC.Configuration.Tests.SuccessfulDiModuleLoadTests.TestClasses;
using NUnit.Framework;
using OROptimizer;
using OROptimizer.Utilities.Xml;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestsSharedLibrary;
using TestsSharedLibrary.DependencyInjection;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.SuccessfulDiModuleLoadTests
{
    public abstract class SuccessfulDiModuleLoadTests
    {
        #region Member Variables

        private static IContainerInfo _containerInfo;
        private static IDiContainer _diContainer;

        private static DiImplementationType _diImplementationType;
        private static bool _mainLifeTimeScopeTerminatedExecuted;

        #endregion

        #region Member Functions

        protected static void ClassCleanupCommon()
        {
            _containerInfo.Dispose();

            Assert.IsTrue(_mainLifeTimeScopeTerminatedExecuted);
        }

        protected static void ClassInitializeCommon(DiImplementationType diImplementationType,
                                                    Func<IContainerInfo> createContainerInfo)
        {
            TestsHelper.SetupLogger();
            Log4Tests.LogLevel = LogLevel.Debug;

            _diImplementationType = diImplementationType;

            _mainLifeTimeScopeTerminatedExecuted = false;

            _containerInfo = createContainerInfo();

            _diContainer = _containerInfo.DiContainer;

            _diContainer.MainLifeTimeScope.LifeTimeScopeTerminated += (sender, e) => { _mainLifeTimeScopeTerminatedExecuted = true; };
        }

        protected static IContainerInfo CreateCodeBasedContainerInfo(DiImplementationType diImplementationType)
        {
            var diImplementationInfo = DiManagerHelpers.GetDiImplementationInfo(diImplementationType);

            var assemblyProbingPaths = new[]
            {
                DiManagerHelpers.ThirdPartyLibsFolder, diImplementationInfo.DiManagerFolder
            };

            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();

            return diContainerBuilder.StartCodeBasedDi(diImplementationInfo.DiManagerClassName,
                                         diImplementationInfo.DiManagerAssemblyPath,
                                         new ParameterInfo[0], Helpers.TestsEntryAssemblyFolder, assemblyProbingPaths)
                                     .WithoutPresetDiContainer()
                                     .AddDiModules(new TestDiModule(), new TestModule2())
                                     .RegisterModules().Start();
        }

        protected static IContainerInfo CreateFileConfigurationBasedContainerInfo(DiImplementationType diImplementationType)
        {
            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();

            return diContainerBuilder.StartFileBasedDi(
                    new FileBasedConfigurationParameters(new FileBasedConfigurationFileContentsProvider(
                            Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration2.xml")),
                        Helpers.TestsEntryAssemblyFolder, new LoadedAssembliesForTests())
                    {
                        AttributeValueTransformers = new[] { new FileFolderPathAttributeValueTransformer() },
                        ConfigurationFileXmlDocumentLoaded = (sender, e) =>
                        {
                            Helpers.EnsureConfigurationDirectoryExistsOrThrow(e.XmlDocument.SelectElement("/iocConfiguration/appDataDir").GetAttribute("path"));
                            Helpers.ReplaceActiveDiManagerInConfigurationFile(e.XmlDocument, diImplementationType);
                        }
                    }, out _)
                    .WithoutPresetDiContainer()
                    .AddAdditionalDiModules(new TestModule2())
                    .RegisterModules()
                    .Start();
        }

        /// <summary>
        ///     Tests constructor and property injection.
        /// </summary>
        [Test]
        public void TestCircularReferences()
        {
            var service1 = _diContainer.Resolve<ICircularReferenceTestInterface1>();
            Assert.IsInstanceOf<CircularReferenceTestInterface1_Impl>(service1);

            var service2 = _diContainer.Resolve<ICircularReferenceTestInterface2>();
            Assert.IsInstanceOf<CircularReferenceTestInterface2_Impl>(service2);

            Assert.AreSame(service1.Property1, service2,
                $"Property injection is used for '{service1.GetType().FullName}.{nameof(ICircularReferenceTestInterface1.Property1)}'");
            Assert.AreSame(service2.Property1, service1,
                $"Constructor injection is used for '{service2.GetType().FullName}.{nameof(ICircularReferenceTestInterface2.Property1)}'");

            TestSingletoneScope(typeof(ICircularReferenceTestInterface1));
            TestSingletoneScope(typeof(ICircularReferenceTestInterface2));
        }

        [Test]
        public void TestDelegateInBinding()
        {
            var instance = _diContainer.Resolve<TestClasses.IInterface6>();

            Assert.IsInstanceOf<TestClasses.Interface6_Impl1>(instance);
            Assert.AreEqual(11, instance.Property1);
            Assert.IsInstanceOf<TestClasses.Interface1_Impl1>(instance.Property2);
        }

        // NOTE: Currently this test is for no-completed functionality and will fail. 
        [Test]
        public void TestDelegateReturningCollectionInBinding()
        {
            var interface8 = _diContainer.Resolve<TestClasses.IInterface8>();

            // Enable this test once IEnumerable<T> binding injection into constructor works in Ninject.
            // After all the bindings are configured, go over all IEnumerable<T> in Ninject handler and
            // create bindings for each item

            var values = interface8.Property1.ToList();
            Assert.AreEqual(2, values.Count);

            values = _diContainer.Resolve<IEnumerable<TestClasses.IInterface7>>().ToList();
            Assert.AreEqual(2, values.Count);
        }

        private void TestLifetimeScope(Type serviceType)
        {
            // Same objects are created in default lifetime scope.
            var service1InMainScope = _diContainer.Resolve(serviceType);
            var service2InMainScope = _diContainer.Resolve(serviceType);

            Assert.AreSame(service1InMainScope, service2InMainScope);

            object serviceInScope1;
            object serviceInScope2;

            using (var lifeTimeScope = _diContainer.StartLifeTimeScope())
            {
                serviceInScope1 = _diContainer.Resolve(serviceType, lifeTimeScope);
                var service2InScope1 = _diContainer.Resolve(serviceType, lifeTimeScope);

                Assert.AreSame(serviceInScope1, service2InScope1);
                Assert.AreNotSame(serviceInScope1, service1InMainScope);
            }

            using (var lifeTimeScope = _diContainer.StartLifeTimeScope())
            {
                serviceInScope2 = _diContainer.Resolve(serviceType, lifeTimeScope);
                var service2InScope2 = _diContainer.Resolve(serviceType, lifeTimeScope);

                Assert.AreSame(serviceInScope2, service2InScope2);
                Assert.AreNotSame(serviceInScope2, service1InMainScope);
            }

            Assert.AreNotSame(serviceInScope1, serviceInScope2);
        }

        [Test]
        public void TestRegisterIfNotRegistered()
        {
            var servicesInjectionTesterForInterface5 = _diContainer.Resolve<ClassToTestServicesInjection<TestClasses.IInterface5>>();
            Assert.AreEqual(2, servicesInjectionTesterForInterface5.Implementations.Count);

            var servicesInjectionTesterForInterface4 = _diContainer.Resolve<ClassToTestServicesInjection<TestClasses.IInterface4>>();
            Assert.AreEqual(1, servicesInjectionTesterForInterface4.Implementations.Count);

            var service = _diContainer.Resolve<TestClasses.IInterface4>();
            Assert.IsInstanceOf<TestClasses.Interface4_Impl1>(service);
            TestTransientScope(typeof(TestClasses.IInterface4));
        }

        [Test]
        public void TestRegisterIfNotRegistered_SelfBoundService()
        {
            var servicesInjectionTesterForClass4 = _diContainer.Resolve<ClassToTestServicesInjection<Class4>>();
            var servicesInjectionTesterForClass5 = _diContainer.Resolve<ClassToTestServicesInjection<Class5>>();

            Assert.AreEqual(2, servicesInjectionTesterForClass4.Implementations.Count);
            Assert.AreEqual(1, servicesInjectionTesterForClass5.Implementations.Count);

            // Lets make sure that the binding with singletone scope was selected.
            TestSingletoneScope(typeof(Class5));
        }

        [Test]
        public void TestSelfBoundService()
        {
            var implementation = _diContainer.Resolve<Class1>();
            Assert.AreSame(typeof(Class1), implementation.GetType());
        }

        [Test]
        public void TestSelfBoundServiceLifetimeScope()
        {
            TestLifetimeScope(typeof(Class3));
        }

        [Test]
        public void TestSelfBoundServiceSingletoneScope()
        {
            TestSingletoneScope(typeof(Class1));
        }

        [Test]
        public void TestSelfBoundServiceTransientScope()
        {
            TestTransientScope(typeof(Class2));
        }

        [Test]
        public void TestServiceLifetimeScope()
        {
            TestLifetimeScope(typeof(TestClasses.IInterface3));
        }

        [Test]
        public void TestServiceSingletoneScope()
        {
            TestSingletoneScope(typeof(TestClasses.IInterface1));
        }

        [Test]
        public void TestServiceTransientScope()
        {
            TestTransientScope(typeof(TestClasses.IInterface2));
        }

        private void TestSingletoneScope(Type serviceType)
        {
            var service1 = _diContainer.Resolve(serviceType);
            var service2 = _diContainer.Resolve(serviceType);
            Assert.AreSame(service1, service2);
        }

        private void TestTransientScope(Type serviceType)
        {
            var service1 = _diContainer.Resolve(serviceType);
            var service2 = _diContainer.Resolve(serviceType);
            Assert.AreNotSame(service1, service2);
        }


        [Test]
        public void TestBindingSelfBoundImplementationToInterface()
        {
            var interface13_Impl1_1 = _diContainer.Resolve<Interface13_Impl1>();
            var interface13_Impl1_2 = _diContainer.Resolve<Interface13_Impl1>();

            Assert.AreSame(interface13_Impl1_1, interface13_Impl1_2);

            var interface13_1 = _diContainer.Resolve<IInterface13>();
            var interface13_2 = _diContainer.Resolve<IInterface13>();

            Assert.AreSame(interface13_1, interface13_2);
            Assert.AreSame(interface13_1, interface13_Impl1_1);

            var interface14 = _diContainer.Resolve<IInterface14>();
            Assert.AreSame(interface14.InterfaceInjectedValue, interface14.NonInterfaceInjectedValue);

        }
        #endregion

        #region Nested Types

        protected class TestModule2 : ModuleAbstr
        {
            #region Member Functions

            protected override void AddServiceRegistrations()
            {
                Bind<ClassToTestServicesInjection<Class4>>().ToSelf();
                Bind<ClassToTestServicesInjection<Class5>>().ToSelf();
                Bind<ClassToTestServicesInjection<TestClasses.IInterface4>>().ToSelf();
                Bind<ClassToTestServicesInjection<TestClasses.IInterface5>>().ToSelf();

                Bind<Interface13_Impl1>().ToSelf().SetResolutionScope(DiResolutionScope.Singleton);
                Bind<IInterface13>().To(x => x.Resolve<Interface13_Impl1>()).SetResolutionScope(DiResolutionScope.Singleton);
                Bind<IInterface14>().To<Interface14_Impl1>().SetResolutionScope(DiResolutionScope.Singleton);
            }

            #endregion
        }
        #endregion
    }
}