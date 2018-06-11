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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer;
using SharedServices;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using TestsSharedLibrary;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.SuccessfulConfigurationLoadTests
{
    public class SuccessfulConfigurationLoadTests
    {
        #region Member Variables

        private static IContainerInfo _containerInfo;
        private static IDiContainer _diContainer;

        private static DiImplementationType _diImplementationType;

        private static bool _mainLifeTimeScopeTerminatedExecuted;
        private static List<IPlugin> _pluginsToTest;

        private static List<IStartupAction> _startupActionsToTest;

        #endregion
        
        #region Member Functions

        protected static void ClassCleanupCommon()
        {
            _containerInfo.Dispose();

            Assert.IsTrue(_mainLifeTimeScopeTerminatedExecuted);

            ValidateStartupActions(StartupActionState.StartCalled | StartupActionState.StopCalled);
            ValidatePluginsState(true);
        }
      
        protected static void ClassInitializeCommon(DiImplementationType diImplementationType, Action<ConfigurationFileXmlDocumentLoadedEventArgs> configurationFileXmlDocumentLoadedEventHandler)
        {
            TestsHelper.SetupLogger();
            Log4Tests.LogLevel = LogLevel.Debug;

            _diImplementationType = diImplementationType;

            _mainLifeTimeScopeTerminatedExecuted = false;
            var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();
            _containerInfo = diContainerBuilder.StartFileBasedDi(
                                                   new FileBasedConfigurationFileContentsProvider(
                                                       Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration1.xml")),
                                                   Helpers.TestsEntryAssemblyFolder,
                                                   (sender, e) =>
                                                   {
                                                       Helpers.ReplaceActiveDiManagerInConfigurationFile(e.XmlDocument, _diImplementationType);
                                                       configurationFileXmlDocumentLoadedEventHandler?.Invoke(e);
                                                   })
                                               .WithoutPresetDiContainer()
                                               .AddAdditionalDiModules(new TestModule2())
                                               .RegisterModules()
                                               .Start();

            _diContainer = _containerInfo.DiContainer;

            var injectedStartupActionsHelper = _diContainer.Resolve<ClassToTestServicesInjection<IStartupAction>>();

            _startupActionsToTest = injectedStartupActionsHelper.Implmentations.Where(x => x is IStartupActionState &&
                                                                                           new[]
                                                                                               {
                                                                                                   "DynamicallyLoadedAssembly1.Implementations.StartupAction1",
                                                                                                   "DynamicallyLoadedAssembly1.Implementations.StartupAction2"
                                                                                               }
                                                                                               .Contains(x.GetType().FullName)).ToList();

            var injectedPluginsHelper = _diContainer.Resolve<ClassToTestServicesInjection<IPlugin>>();
            _pluginsToTest = injectedPluginsHelper.Implmentations.Where(x => x is IPluginState &&
                                                                             new[]
                                                                                 {
                                                                                     "TestPluginAssembly1.Implementations.Plugin1",
                                                                                     "TestPluginAssembly2.Implementations.Plugin2"
                                                                                 }
                                                                                 .Contains(x.GetType().FullName)).ToList();

            _diContainer.MainLifeTimeScope.LifeTimeScopeTerminated += (sender, e) => { _mainLifeTimeScopeTerminatedExecuted = true; };
        }

        private IModuleElement GetModule(IDependencyInjection dependencyInjection, string moduleType)
        {
            var allModules = dependencyInjection.Modules.Modules.Where(x => moduleType.Equals(x.DiModule.GetType().FullName, StringComparison.Ordinal)).ToList();

            Assert.AreEqual(1, allModules.Count);

            return allModules[0];
        }

        [TestMethod]
        public void LoadAlwaysAttributeTest()
        {
            // Test attributes "loadAlways" and "overrideDirectory" in element "assembly"
            Assert.IsNotNull(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(
                x => "TestProjects.TestForceLoadAssembly".Equals(x.GetName().Name, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void PluginSettingsTest_SettingInGlobalSettings()
        {
            var pluginRepository = _diContainer.Resolve<IPluginDataRepository>();

            var pluginData = pluginRepository.GetPluginData("Plugin1");

            // Plugin does not have this setting. However, global setting is retrieved.
            Assert.AreEqual(pluginData.Settings.GetSettingValueOrThrow<double>("MaxCharge"), 155.7);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PluginSettingsTest_SettingInPluginSettings()
        {
            var pluginRepository = _diContainer.Resolve<IPluginDataRepository>();

            var pluginData = pluginRepository.GetPluginData("Plugin1");

            Assert.AreEqual(pluginData.Settings.GetSettingValueOrThrow<int>("Int32Setting1"), 25);

            // This line will throw an exception, since the setting is of int type.
            Assert.AreEqual(pluginData.Settings.GetSettingValueOrThrow<double>("Int32Setting1"), 25);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SettingInConfigurationButWithDifferentTypeTest()
        {
            var settings = _diContainer.Resolve<ISettings>();
            Assert.IsTrue(settings.GetSettingValue("MaxCharge", 17.5, out var settingValueDouble) && settingValueDouble == 155.7);
            Assert.IsTrue(!settings.GetSettingValue("MaxCharge", 17, out var settingValueInt) && settingValueInt == 17);

            // This line will throw an exception.
            settings.GetSettingValueOrThrow<int>("MaxCharge");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SettingNotInConfigurationTest()
        {
            var settings = _diContainer.Resolve<ISettings>();
            Assert.IsTrue(!settings.GetSettingValue("NoSuchSetting", 15.3, out var settingValue) && settingValue == 15.3);

            // This line will throw an exception.
            settings.GetSettingValueOrThrow<int>("NoSuchSetting");
        }

        [TestMethod]
        public void SettingsTest()
        {
            var settings = _diContainer.Resolve<ISettings>();

            Assert.AreEqual(settings.GetSettingValueOrThrow<int>("SynchronizerFrequencyInMilliseconds"), 5000);
            Assert.AreEqual(settings.GetSettingValueOrThrow<double>("MaxCharge"), 155.7);
        }

        [TestMethod]
        public void StartupActionAndPluginsStartedTest()
        {
            ValidateStartupActions(StartupActionState.StartCalled);
            ValidatePluginsState(false);
        }

        [TestMethod]
        public void TestAutogeneratedInterfaceImplementationFactories1()
        {
            var cleanupJobFactory = _diContainer.Resolve<ICleanupJobFactory>();

            // parameter value is 1
            var cleanupJobs = cleanupJobFactory.GetCleanupJobs(1).ToList();
            Assert.AreEqual(2, cleanupJobs.Count);

            Assert.IsInstanceOfType(cleanupJobs[0], Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJob1"));
            // CleanupJobData was injected in CleanupJob1 constructor.
            Assert.IsInstanceOfType(cleanupJobs[0].CleanupJobData, Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJobData"));

            Assert.IsInstanceOfType(cleanupJobs[1], Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJob2"));
            // CleanupJobData2 was injected in CleanupJob2 constructor.
            Assert.IsInstanceOfType(cleanupJobs[1].CleanupJobData, Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJobData2"));


            // parameter value is 2
            cleanupJobs = cleanupJobFactory.GetCleanupJobs(2).ToList();
            Assert.AreEqual(1, cleanupJobs.Count);

            Assert.IsInstanceOfType(cleanupJobs[0], Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJob2"));
            // CleanupJobData2 was injected in CleanupJob2 constructor.
            Assert.IsInstanceOfType(cleanupJobs[0].CleanupJobData, Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJobData2"));

            // parameter value is anything else (default scenario)
            cleanupJobs = cleanupJobFactory.GetCleanupJobs(5).ToList();
            Assert.AreEqual(2, cleanupJobs.Count);

            Assert.IsInstanceOfType(cleanupJobs[0], Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJob1"));
            // CleanupJobData was injected into CleanupJob1 constructor.
            Assert.IsInstanceOfType(cleanupJobs[0].CleanupJobData, Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJobData"));


            Assert.IsInstanceOfType(cleanupJobs[1], Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJob3"));
            // CleanupJobData2 was injected into CleanupJob3.CleanupJobData property.
            Assert.IsInstanceOfType(cleanupJobs[1].CleanupJobData, Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.CleanupJobData2"));
        }

        [TestMethod]
        public void TestAutogeneratedInterfaceImplementationFactories2()
        {
            Type[] parameterTypes = {typeof(int), typeof(string)};
            var getObjectsMethodName = "GetInstances";

            var actionValidatorFactory1 = _diContainer.Resolve(Helpers.GetType("DynamicallyLoadedAssembly2.IActionValidatorFactory1"));

            ValidateAutogeneratedFactoryImplementationResuls(actionValidatorFactory1, getObjectsMethodName, parameterTypes,
                new object[] {1, "project1"},
                new[] {Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator3"), Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator1")});

            ValidateAutogeneratedFactoryImplementationResuls(actionValidatorFactory1, getObjectsMethodName, parameterTypes,
                new object[] {1, "project2"},
                new[] {Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator1"), Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator2")});

            // If the value of parameter1 1 is 2, the returned values will be of types ActionValidator1, ActionValidator2, and ActionValidator3,
            // regardless of the value of parameter2
            ValidateAutogeneratedFactoryImplementationResuls(actionValidatorFactory1, getObjectsMethodName, parameterTypes,
                new object[] {2, "anything"},
                new[]
                {
                    Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator1"),
                    Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator2"),
                    Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator3")
                });

            // For all other parameter values the return values are of types ActionValidator2 and ActionValidator1.
            ValidateAutogeneratedFactoryImplementationResuls(actionValidatorFactory1, getObjectsMethodName, parameterTypes,
                new object[] {3, "project1"},
                new[]
                {
                    Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator2"),
                    Helpers.GetType("DynamicallyLoadedAssembly2.ActionValidator1")
                });
        }


        [TestMethod]
        public void TestAutogeneratedInterfaceImplementationFactoriesInPlugin()
        {
            Type[] parameterTypes = {typeof(string)};
            var getObjectsMethodName = "GetValidators";

            var resourceAccessVailidatorFactory = _diContainer.Resolve(Helpers.GetType("TestPluginAssembly1.Interfaces.IResourceAccessValidatorFactory"));

            ValidateAutogeneratedFactoryImplementationResuls(resourceAccessVailidatorFactory, getObjectsMethodName, parameterTypes,
                new object[] {"public_pages"},
                new[] {Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator1")});

            ValidateAutogeneratedFactoryImplementationResuls(resourceAccessVailidatorFactory, getObjectsMethodName, parameterTypes,
                new object[] {"admin_pages"},
                new[]
                {
                    Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator1"),
                    Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator2")
                });

            ValidateAutogeneratedFactoryImplementationResuls(resourceAccessVailidatorFactory, getObjectsMethodName, parameterTypes,
                new object[] {"anything else will return this."},
                new[]
                {
                    Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator2"),
                    Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator1")
                });
        }

        [TestMethod]
        public void TestCircularReferences()
        {
            var interface3Impl = _diContainer.Resolve<IInterface3>();
            var interface4Impl = _diContainer.Resolve<IInterface4>();

            Assert.IsNotNull(interface3Impl.Property2);
            Assert.IsNotNull(interface4Impl.Property1);

            Assert.AreSame(interface3Impl.Property2, interface4Impl);
            Assert.AreSame(interface4Impl.Property1, interface3Impl);
        }

        /// <summary>
        ///     There is a separate test class to test code bindings configured in
        ///     <see cref="IoC.Configuration.DiContainer.ModuleAbstr.AddServiceRegistrations" />.
        ///     This test just does a basic smoke test of modules.
        /// </summary>
        [TestMethod]
        public void TestCommonAndPluginModules()
        {
            var injectionTest = _diContainer.Resolve<ClassToTestServicesInjection<IInterface1>>();

            Assert.AreEqual(injectionTest.Implmentations.Count, 4);

            var implementationTypes = new List<string>(4);

            // Add implementations added by modules in Modules assembly 
            if (_diImplementationType == DiImplementationType.Autofac)
                implementationTypes.Add("SharedServices.Implementations.Interface1_Impl1");

            implementationTypes.Add("SharedServices.Implementations.Interface1_Impl3");

            if (_diImplementationType == DiImplementationType.Ninject)
                implementationTypes.Add("SharedServices.Implementations.Interface1_Impl2");

            // Add implementations added by modules in ModulesForPlugin1 assembly 
            if (_diImplementationType == DiImplementationType.Ninject)
                implementationTypes.Add("TestPluginAssembly1.Implementations.Interface1_Impl2");
            else if (_diImplementationType == DiImplementationType.Autofac)
                implementationTypes.Add("TestPluginAssembly1.Implementations.Interface1_Impl1");

            implementationTypes.Add("TestPluginAssembly1.Implementations.Interface1_Impl3");

            injectionTest.ValidateImplementationTypes(implementationTypes);
        }

        [TestMethod]
        public void TestConstructorParametersAndInjectedPropertiesInImplementation()
        {
            var injectedServicesWrapper = _diContainer.Resolve<ClassToTestServicesInjection<IInterface2>>();
            Assert.AreEqual(4, injectedServicesWrapper.Implmentations.Count);

            // Lets make sure the default resolution for iInterface3 is Interface3_Impl1.
            Assert.AreEqual(typeof(Interface3_Impl1),
                _diContainer.Resolve<IInterface3>().GetType());

            for (var implementationIndex = 0; implementationIndex < 4; ++implementationIndex)
            {
                var implementation = injectedServicesWrapper.Implmentations[implementationIndex];

                var property1Value = DateTime.MinValue;
                double property2Value = 0;

                switch (implementationIndex)
                {
                    case 0:
                        // Injected in constructor
                        property1Value = new DateTime(2014, 10, 29, 23, 59, 59, 99);
                        property2Value = 125.1;

                        // Default implementation is injected into constructor.
                        Assert.AreEqual(typeof(Interface3_Impl1), implementation.Property3.GetType());
                        break;

                    case 1:
                        // Property injection
                        property1Value = new DateTime(1915, 4, 24, 0, 0, 0, 1);
                        property2Value = 365.41;

                        // Default implementation is injected into property.
                        Assert.AreEqual(typeof(Interface3_Impl1), implementation.Property3.GetType());
                        break;

                    case 2:
                        // Property injection overrides constructor parameters
                        property1Value = new DateTime(2017, 10, 29, 23, 59, 59, 99);
                        property2Value = 148.3;

                        var implementation2 = (Interface2_Impl3) implementation;

                        // Specific implementations are injected into constructor and property.
                        Assert.AreEqual(typeof(Interface3_Impl2), implementation2.Param3_InitializedInConstructor.GetType());
                        Assert.AreEqual(typeof(Interface3_Impl3), implementation2.Property3.GetType());
                        break;

                    case 3:
                        // Default primitive values are injected into constructor.
                        // The values are specified in module IoC.Configuration.Tests.PrimitiveTypeDefaultBindingsModule
                        property1Value = new DateTime(1915, 4, 24, 0, 0, 0, 0);
                        property2Value = 0;

                        // Default implementation is injected into constructor.
                        Assert.AreEqual(typeof(Interface3_Impl1), implementation.Property3.GetType());
                        break;
                }

                Assert.AreEqual(property1Value, implementation.Property1);
                Assert.AreEqual(property2Value, implementation.Property2);
            }
        }

        [TestMethod]
        public void TestLifetimeScope()
        {
            var interfaceType = Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface3");

            // Same objects are created in default lifetime scope.
            var service1InMainScope = _diContainer.Resolve(interfaceType);
            var service2InMainScope = _diContainer.Resolve(interfaceType);

            Assert.AreSame(service1InMainScope, service2InMainScope);
            Assert.AreEqual("DynamicallyLoadedAssembly1.Implementations.Interface3_Impl1", service1InMainScope.GetType().FullName, false);

            object serviceInScope1;
            object serviceInScope2;

            using (var lifeTimeScope = _diContainer.StartLifeTimeScope())
            {
                serviceInScope1 = _diContainer.Resolve(interfaceType, lifeTimeScope);
                var service2InScope1 = _diContainer.Resolve(interfaceType, lifeTimeScope);

                Assert.AreSame(serviceInScope1, service2InScope1);
                Assert.AreNotSame(serviceInScope1, service1InMainScope);
            }

            using (var lifeTimeScope = _diContainer.StartLifeTimeScope())
            {
                serviceInScope2 = _diContainer.Resolve(interfaceType, lifeTimeScope);
                var service2InScope2 = _diContainer.Resolve(interfaceType, lifeTimeScope);

                Assert.AreSame(serviceInScope2, service2InScope2);
                Assert.AreNotSame(serviceInScope2, service1InMainScope);
            }

            Assert.AreNotSame(serviceInScope1, serviceInScope2);
        }


        [TestMethod]
        public void TestModuleConstructorParameters()
        {
            //var dependencyInjectionElement = _applicationBuilder.Configuration.DependencyInjection;
            var dependencyInjectionElement = _diContainer.Resolve<IConfiguration>().DependencyInjection;

            //var modulesList = new List<IModuleElement>(_applicationBuilder.Configuration.DependencyInjection.Modules.Modules);

            if (_diImplementationType == DiImplementationType.Autofac)
                Assert.AreEqual(1, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "Modules.Autofac.AutofacModule1").DiModule, "Property1"));
            else if (_diImplementationType == DiImplementationType.Ninject)
                Assert.AreEqual(3, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "Modules.Ninject.NinjectModule1").DiModule, "Property1"));

            Assert.AreEqual(2, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "Modules.IoC.DiModule1").DiModule, "Property1"));

            // Check modules for Plugin1
            dependencyInjectionElement = _diContainer.Resolve<IConfiguration>().PluginsSetup.AllPluginSetups.FirstOrDefault().DependencyInjection;

            if (_diImplementationType == DiImplementationType.Autofac)
                Assert.AreEqual(102, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "ModulesForPlugin1.Autofac.AutofacModule1").DiModule, "Property1"));
            else if (_diImplementationType == DiImplementationType.Ninject)
                Assert.AreEqual(101, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "ModulesForPlugin1.Ninject.NinjectModule1").DiModule, "Property1"));

            Assert.AreEqual(103, Helpers.GetPropertyValue<int>(GetModule(dependencyInjectionElement, "ModulesForPlugin1.IoC.DiModule1").DiModule, "Property1"));
        }

        [TestMethod]
        public void TestMultipleConstructors()
        {
            ConstructorInfo constructorInfo;
            string errorMessage;

            Assert.IsTrue(GlobalsCoreAmbientContext.Context.CheckTypeConstructorExistence(typeof(Interface8_Impl1), new Type[] { }, out constructorInfo, out errorMessage));
            Assert.IsTrue(GlobalsCoreAmbientContext.Context.CheckTypeConstructorExistence(typeof(Interface8_Impl1), new[] {typeof(Interface9_Impl1)}, out constructorInfo, out errorMessage));
            Assert.IsTrue(GlobalsCoreAmbientContext.Context.CheckTypeConstructorExistence(typeof(Interface8_Impl2), new Type[] { }, out constructorInfo, out errorMessage));
            Assert.IsTrue(GlobalsCoreAmbientContext.Context.CheckTypeConstructorExistence(typeof(Interface8_Impl2), new[] {typeof(Interface9_Impl1)}, out constructorInfo, out errorMessage));

            var injectionTester = _diContainer.Resolve<ClassToTestServicesInjection<IInterface8>>();

            Assert.AreEqual(2, injectionTester.Implmentations.Count);
            Assert.IsInstanceOfType(injectionTester.Implmentations[0], typeof(Interface8_Impl1));
            Assert.IsInstanceOfType(injectionTester.Implmentations[1], typeof(Interface8_Impl2));

            var implementation1 = (Interface8_Impl1) injectionTester.Implmentations[0];
            Assert.IsNull(implementation1.Property1, "Default constructor should have been used, therefore Property1 should have been null.");

            var implementation2 = (Interface8_Impl2) injectionTester.Implmentations[1];
            Assert.IsNotNull(implementation2.Property1, "Non default constructor should have been used, therefore Property1 should have been non-null.");
            Assert.IsInstanceOfType(implementation2.Property1, typeof(Interface9_Impl1));
        }

        [TestMethod]
        public void TestNestedLifetimeScopes()
        {
            var interfaceType = Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface3");

            using (var lifeTimeScope = _diContainer.StartLifeTimeScope())
            {
                var service1InScope1 = _diContainer.Resolve(interfaceType, lifeTimeScope);
                using (var lifeTimeScope2 = _diContainer.StartLifeTimeScope())
                {
                    var service1InScope2 = _diContainer.Resolve(interfaceType, lifeTimeScope2);
                    var service2InScope2 = _diContainer.Resolve(interfaceType, lifeTimeScope2);

                    Assert.AreSame(service1InScope2, service2InScope2);
                    Assert.AreNotSame(service1InScope2, service1InScope1);
                }

                var service2InScope1 = _diContainer.Resolve(interfaceType, lifeTimeScope);

                Assert.AreSame(service1InScope1, service2InScope1);
            }
        }

        [TestMethod]
        public void TestObjectTypeConstructorParameters()
        {
            var room = _diContainer.Resolve(Helpers.GetType("TestPluginAssembly1.Interfaces.IRoom"));
            var doorPropertyValue = Helpers.GetPropertyValue<object>(room, "Door1");

            // The serializer TestPluginAssembly1.Implementations.DoorSerializer will de-serialize constructor parameter door1 
            // into an object of type TestPluginAssembly1.Implementations.Door.
            Assert.IsTrue(Helpers.GetType("TestPluginAssembly1.Implementations.Door").IsAssignableFrom(doorPropertyValue.GetType()));
            Assert.AreEqual(Helpers.GetPropertyValue<int>(doorPropertyValue, "Color"), 5);
            Assert.AreEqual(Helpers.GetPropertyValue<double>(doorPropertyValue, "Height"), 185.1);
        }

        [TestMethod]
        public void TestObjectTypePropertyInjection()
        {
            var room = _diContainer.Resolve(Helpers.GetType("TestPluginAssembly1.Interfaces.IRoom"));
            var doorPropertyValue = Helpers.GetPropertyValue<object>(room, "Door2");

            // The serializer TestPluginAssembly1.Implementations.DoorSerializer will de-serialize property Door2 
            // into an object of type TestPluginAssembly1.Implementations.Door.
            Assert.IsTrue(Helpers.GetType("TestPluginAssembly1.Implementations.Door").IsAssignableFrom(doorPropertyValue.GetType()));
            Assert.AreEqual(Helpers.GetPropertyValue<int>(doorPropertyValue, "Color"), 7);
            Assert.AreEqual(Helpers.GetPropertyValue<double>(doorPropertyValue, "Height"), 187.3);
        }


        [TestMethod]
        public void TestPluginConstructorParameterInjections()
        {
            var pluginRepository = _diContainer.Resolve<IPluginDataRepository>();

            var pluginData = pluginRepository.GetPluginData("Plugin1");
            Assert.AreEqual(Helpers.GetPropertyValue<long>(pluginData.Plugin, "Property1"), 25, "Configuration file specifies param1=25 injected into constructor.");
        }

        [TestMethod]
        public void TestPluginPropertyInjection()
        {
            var pluginRepository = _diContainer.Resolve<IPluginDataRepository>();

            var pluginData = pluginRepository.GetPluginData("Plugin1");

            Assert.AreEqual(Helpers.GetPropertyValue<long>(pluginData.Plugin, "Property2"), 35, "Configuration file specifies 35 in property injection element.");
        }

        [TestMethod]
        public void TestRegisterIfNotRegistered()
        {
            var injectionTester1 = _diContainer.Resolve<ClassToTestServicesInjection<IInterface6>>();

            // This implementation was registered in module
            injectionTester1.ValidateHasImplementation(typeof(Interface6_Impl1));

            // This implementation was registered later in configuration file.
            injectionTester1.ValidateDoesNotHaveImplementation(typeof(Interface6_Impl2).FullName);

            var injectionTester2 = _diContainer.Resolve<ClassToTestServicesInjection<IInterface7>>();
            // This implementation was registered only in configuration
            injectionTester2.ValidateHasImplementation(typeof(Interface7_Impl1));
        }

        [TestMethod]
        public void TestRegisterIfNotRegisteredInSelfBoundService()
        {
            var configuration = _diContainer.Resolve<IConfiguration>();

            Assert.AreEqual(true,
                configuration.DependencyInjection.Services.AllServices.Where(x => x.ServiceType == typeof(SelfBoundService1))
                             .First().RegisterIfNotRegistered,
                $"The service should be registedr with {nameof(IServiceElement.RegisterIfNotRegistered)} set to true");
            Assert.IsNotNull(_diContainer.Resolve<SelfBoundService1>());
        }

        [TestMethod]
        public void TestSelfBoundServiceConstructorParametersInjection()
        {
            var serviceType = Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.SelfBoundService1");
            var resolvedObject = _diContainer.Resolve(serviceType);

            Assert.AreEqual(14, Helpers.GetPropertyValue<int>(resolvedObject, "Property1"));
            Assert.AreEqual(15.3, Helpers.GetPropertyValue<double>(resolvedObject, "Property2"));
            Assert.IsTrue(Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface1").IsAssignableFrom(Helpers.GetPropertyValue<object>(resolvedObject, "Property3").GetType()));
        }

        [TestMethod]
        public void TestSelfBoundServiceInLifetimeScope()
        {
            var serviceType = Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.SelfBoundService3");

            var service1InMainScope = _diContainer.Resolve(serviceType);
            var service2InMainScope = _diContainer.Resolve(serviceType);

            Assert.AreEqual(serviceType.FullName, service1InMainScope.GetType().FullName);
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

        [TestMethod]
        public void TestSelfBoundServiceInSingletoneScope()
        {
            var serviceType = Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.SelfBoundService1");

            var implementation1 = _diContainer.Resolve(serviceType);
            var implementation2 = _diContainer.Resolve(serviceType);

            Assert.AreSame(serviceType, implementation1.GetType());
            Assert.AreSame(implementation1, implementation2);
        }

        [TestMethod]
        public void TestSelfBoundServiceInTransientScope()
        {
            var serviceType = Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.SelfBoundService2");

            var service1 = _diContainer.Resolve(serviceType);
            var service2 = _diContainer.Resolve(serviceType);

            Assert.AreEqual(serviceType.FullName, service1.GetType().FullName);
            Assert.AreNotSame(service1, service2);
        }

        [TestMethod]
        public void TestSelfBoundServicePropertiesInjections()
        {
            var serviceType = Helpers.GetType("DynamicallyLoadedAssembly1.Implementations.SelfBoundService2");
            var resolvedObject = _diContainer.Resolve(serviceType);

            Assert.AreEqual(17, Helpers.GetPropertyValue<int>(resolvedObject, "Property1"));
            Assert.AreEqual(18.1, Helpers.GetPropertyValue<double>(resolvedObject, "Property2"));
            Assert.IsTrue(Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface1").IsAssignableFrom(Helpers.GetPropertyValue<object>(resolvedObject, "Property3").GetType()));
        }

        [TestMethod]
        public void TestServiceImplementationsInPlugins()
        {
            var servicesInjectiontester = _diContainer.Resolve<ClassToTestServicesInjection<IInterface5>>();

            servicesInjectiontester.ValidateHasImplementation(Helpers.GetType("SharedServices.Implementations.Interface5_Impl1"));
            servicesInjectiontester.ValidateHasImplementation(Helpers.GetType("TestPluginAssembly1.Implementations.Interface5_Plugin1Impl"));
            servicesInjectiontester.ValidateHasImplementation(Helpers.GetType("TestPluginAssembly2.Implementations.Interface5_Plugin2Impl"));

            // Plugin 3 is disabled, so its implementations are ignored.
            servicesInjectiontester.ValidateDoesNotHaveImplementation("TestPluginAssembly3.Implementations.Interface5_Plugin3Impl");
        }

        [TestMethod]
        public void TestServicesDefinedInPlugin()
        {
            var room = _diContainer.Resolve(Helpers.GetType("TestPluginAssembly1.Interfaces.IRoom"));
            Assert.AreEqual(room.GetType().FullName, "TestPluginAssembly1.Implementations.Room", false);
        }

        [TestMethod]
        public void TestSingletoneScope()
        {
            var implementation1 = _diContainer.Resolve(Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface1"));
            var implementation2 = _diContainer.Resolve(Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface1"));

            Assert.AreSame(implementation1, implementation2);
            Assert.AreEqual("DynamicallyLoadedAssembly1.Implementations.Interface1_Impl1", implementation1.GetType().FullName, false);
        }

        [TestMethod]
        public void TestTransientScope()
        {
            var implementation1 = _diContainer.Resolve(Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface2"));
            var implementation2 = _diContainer.Resolve(Helpers.GetType("DynamicallyLoadedAssembly1.Interfaces.IInterface2"));

            Assert.AreNotSame(implementation1, implementation2);

            Assert.AreEqual("DynamicallyLoadedAssembly1.Implementations.Interface2_Impl1", implementation1.GetType().FullName, false);
            Assert.AreEqual(implementation1.GetType().FullName, implementation2.GetType().FullName, false);
        }

        private void ValidateAutogeneratedFactoryImplementationResuls([NotNull] object factoryObject, [NotNull] string getObjectsMethodName,
                                                                      [NotNull] [ItemNotNull] Type[] parameterTypes, [NotNull] object[] parameterValues, [NotNull] Type[] expectedReturnTypes)
        {
            var getInstancesMethodInfo = factoryObject.GetType().GetMethod(getObjectsMethodName, parameterTypes);

            var returnedObjects = ((IEnumerable<object>) getInstancesMethodInfo.Invoke(factoryObject, parameterValues)).ToList();

            Assert.AreEqual(expectedReturnTypes.Length, returnedObjects.Count);

            for (var i = 0; i < returnedObjects.Count; ++i)
                Assert.IsInstanceOfType(returnedObjects[i], expectedReturnTypes[i]);
        }

        private static void ValidatePluginsState(bool expectsPluginsToBeDisposed)
        {
            Assert.AreEqual(2, _pluginsToTest.Count);

            foreach (var plugin in _pluginsToTest)
            {
                var pluginState = (IPluginState) plugin;
                Assert.IsTrue(pluginState.IsInitialized);
                Assert.AreEqual(expectsPluginsToBeDisposed, pluginState.IsDisposedOf);
            }
        }

        private static void ValidateStartupActions(StartupActionState expectedSartupActionState)
        {
            Assert.AreEqual(_startupActionsToTest.Count, 2);

            foreach (var startupAction in _startupActionsToTest)
                Assert.AreEqual(((IStartupActionState) startupAction).StartupActionState, expectedSartupActionState);
        }

        #endregion

        #region Nested Types

        public class TestModule1 : ModuleAbstr
        {
            #region Member Variables

            [NotNull]
            private readonly Dictionary<Type, object> _serviceTypeToMockedImplementationObjectMap;

            #endregion

            #region  Constructors

            public TestModule1([NotNull] Dictionary<Type, object> serviceTypeToMockedImplementationObjectMap)
            {
                _serviceTypeToMockedImplementationObjectMap = serviceTypeToMockedImplementationObjectMap;
            }

            #endregion

            #region Member Functions

            protected override void AddServiceRegistrations()
            {
                foreach (var keyValuePair in _serviceTypeToMockedImplementationObjectMap)
                    Bind(keyValuePair.Key).To(typeResolver => keyValuePair.Value);
            }

            #endregion
        }

        public class TestModule2 : ModuleAbstr
        {
            #region Member Functions

            protected override void AddServiceRegistrations()
            {
                Bind<ClassToTestServicesInjection<IInterface1>>().ToSelf();
                Bind<ClassToTestServicesInjection<IInterface2>>().ToSelf();
                Bind<ClassToTestServicesInjection<IInterface5>>().ToSelf();
                Bind<ClassToTestServicesInjection<IInterface6>>().ToSelf();
                Bind<ClassToTestServicesInjection<IInterface7>>().ToSelf();
                Bind<ClassToTestServicesInjection<IInterface8>>().ToSelf();
                Bind<ClassToTestServicesInjection<IStartupAction>>().ToSelf();
                Bind<ClassToTestServicesInjection<IPlugin>>().ToSelf();
            }

            #endregion
        }

        #endregion
    }
}