using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.Tests.TestTemplateFiles;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;
using SharedServices.Implementations;
using SharedServices.Implementations.Generic;
using SharedServices.Interfaces;
using SharedServices.Interfaces.Generic;
using TestsSharedLibrary;
using TestsSharedLibrary.DependencyInjection;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests.GenericTypesAndTypeReUse
{
    [TestClass]
    public class GenericTypesAndTypeReUseTests : IoCConfigurationTestsBase
    {
        #region Member Variables
        private const string Plugin1Name = "Plugin1";
        private static string _configurationRelativePath = Path.Combine(Helpers.TestsEntryAssemblyFolder, "IoCConfiguration_GenericTypesAndTypeReUse.xml");

        private System.Reflection.Assembly _modulesAssembly;
        private System.Reflection.Assembly _plugin1ModulesAssembly;

        private static ITypeDefinitionsElement _typeDefinitionsElement;
        private static ITypeDefinitionsElement _pluginTypeDefinitionsElement;
        private System.Reflection.Assembly _testPluginAssembly1;

        #endregion

        #region Member Functions
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestsHelper.SetupLogger();
            Log4Tests.LogLevel = LogLevel.Info;

            using (var containerInfo = new DiContainerBuilder.DiContainerBuilder()
                                  .StartFileBasedDi(
                                      new FileBasedConfigurationFileContentsProvider(_configurationRelativePath),
                                      Helpers.TestsEntryAssemblyFolder, null)
                                  .WithoutPresetDiContainer().RegisterModules().Start())
            {
                var configuration = containerInfo.DiContainer.Resolve<IConfiguration>();
                _typeDefinitionsElement = configuration.TypeDefinitions;
                _pluginTypeDefinitionsElement = configuration.PluginsSetup.GetPluginSetup(Plugin1Name).TypeDefinitions;
            }
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void GenericTypeTest1(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                Assert.IsNotNull(configuration.DependencyInjection.Services.AllServices.FirstOrDefault(x => x.ServiceTypeInfo.Type == typeof(IGeneric2_1<Generic3_1<int>>)));

                var implementation = diContainer.Resolve<IGeneric2_1<Generic3_1<int>>>();

                Assert.IsInstanceOfType(implementation, typeof(Generic2_1<Generic3_1<int>>));
                Assert.AreEqual(17, implementation.Value.Value);
            }, false);
        }


        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void GenericTypeTest2(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                Assert.IsNotNull(configuration.DependencyInjection.Services.AllServices.FirstOrDefault(x => x.ServiceTypeInfo.Type ==
                                                                                                            typeof(Generic4_2<Generic2_1<Interface1_Impl1>, Generic2_1<long>>)));

                var implementation = diContainer.Resolve<Generic4_2<Generic2_1<Interface1_Impl1>, Generic2_1<long>>>();

                Assert.IsNotNull(implementation);

                Assert.AreEqual(17, implementation.Value1.Value.Property1);
                Assert.AreEqual(19, implementation.Value2.Value);
            }, false);
        }


        protected override string GetConfigurationRelativePath()
        {
            return _configurationRelativePath;
        }

        private void LoadConfigurationFileAndRunTest(DiImplementationType diImplementationType,
                                                     Action<IDiContainer, IConfiguration> doTest,
                                                     bool testTypeAndAssemblyAttributeValuesToo = true,
                                                     Action<XmlDocument> modifyConfigurationFileOnLoad = null)
        {
            List<ConfigurationFileMutationType> configurationFileMutationTypes = new List<ConfigurationFileMutationType>();

            configurationFileMutationTypes.Add(ConfigurationFileMutationType.None);


            if (testTypeAndAssemblyAttributeValuesToo)
            {
                configurationFileMutationTypes.Add(ConfigurationFileMutationType.UseType);
                configurationFileMutationTypes.Add(ConfigurationFileMutationType.UseTypeAndAssembly);
            }

            foreach (var configurationFileMutationType in configurationFileMutationTypes)
            {
                LogHelper.Context.Log.Info($"Loading configuration file with {nameof(configurationFileMutationType)}={configurationFileMutationType}.");

                LoadConfigurationFile(diImplementationType,
                (container, configuration) =>
                {
                    _testPluginAssembly1 = AppDomain.CurrentDomain.GetAssemblies().First(
                        x => x.GetName().Name.Equals("TestProjects.TestPluginAssembly1", StringComparison.OrdinalIgnoreCase));

                    _modulesAssembly = AppDomain.CurrentDomain.GetAssemblies().First(
                        x => x.GetName().Name.Equals("TestProjects.Modules", StringComparison.OrdinalIgnoreCase));

                    _plugin1ModulesAssembly = AppDomain.CurrentDomain.GetAssemblies().First(
                        x => x.GetName().Name.Equals("TestProjects.ModulesForPlugin1", StringComparison.OrdinalIgnoreCase));

                    doTest(container, configuration);
                }, new IDiModule[] { new TestModule1() },
                (xmlDocument) =>
                {
                    if (configurationFileMutationType != ConfigurationFileMutationType.None)
                    {
                        ProcessXmlElements((XmlElement)xmlDocument.GetElementsByTagName(ConfigurationFileElementNames.RootElement).Item(0),
                            (xmlElement) =>
                            {
                                this.ReplaceTypRefAttributeWithTypeAndAssembly(xmlElement, configurationFileMutationType);
                            });
                    }

                    modifyConfigurationFileOnLoad?.Invoke(xmlDocument);
                });
            }
        }

        private void ProcessXmlElements(XmlElement xmlElement, Action<XmlElement> processElement)
        {
            processElement(xmlElement);

            foreach (var childXmlNode in xmlElement.ChildNodes)
            {
                var childXmlElement = childXmlNode as XmlElement;

                if (childXmlElement == null)
                    continue;

                ProcessXmlElements(childXmlElement, processElement);
            }
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TestAllTypeDefinitions(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("ReadOnlyListOf_IInterface1").ValueTypeInfo,
                    typeof(IReadOnlyList<IInterface1>),
                    "System.Collections.Generic.IReadOnlyList<SharedServices.Interfaces.IInterface1>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("IEnumerableOf_IInterface1").ValueTypeInfo,
                    typeof(IEnumerable<IInterface1>),
                    "System.Collections.Generic.IEnumerable<SharedServices.Interfaces.IInterface1>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("IGeneric1_1_of_Interface1_Impl1").ValueTypeInfo,
                    typeof(IGeneric1_1<Interface1_Impl1>),
                    "SharedServices.Interfaces.Generic.IGeneric1_1<SharedServices.Implementations.Interface1_Impl1>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("Generic1_1_of_Interface1_Impl1").ValueTypeInfo,
                    typeof(Generic1_1<Interface1_Impl1>),
                    "SharedServices.Implementations.Generic.Generic1_1<SharedServices.Implementations.Interface1_Impl1>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("Generic4_2_a").ValueTypeInfo,
                    typeof(Generic4_2<int, string>),
                    "SharedServices.Implementations.Generic.Generic4_2<System.Int32, System.String>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("Generic4_2_b").ValueTypeInfo,
                    typeof(Generic4_2<Generic2_1<Interface1_Impl1>, Generic2_1<long>>),
                    "SharedServices.Implementations.Generic.Generic4_2<SharedServices.Implementations.Generic.Generic2_1<SharedServices.Implementations.Interface1_Impl1>, SharedServices.Implementations.Generic.Generic2_1<System.Int64>>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("Generic4_2_c").ValueTypeInfo,
                    typeof(Generic4_2<IGeneric2_1<string>, IGeneric2_1<Interface1_Impl2>>),
                    "SharedServices.Implementations.Generic.Generic4_2<SharedServices.Interfaces.Generic.IGeneric2_1<System.String>, SharedServices.Interfaces.Generic.IGeneric2_1<SharedServices.Implementations.Interface1_Impl2>>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("Generic4_2_d").ValueTypeInfo,
                    typeof(Generic4_2<IGeneric2_1<string>, IGeneric2_1<double>>),
                    "SharedServices.Implementations.Generic.Generic4_2<SharedServices.Interfaces.Generic.IGeneric2_1<System.String>, SharedServices.Interfaces.Generic.IGeneric2_1<System.Double>>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("pluginTypeDef").ValueTypeInfo,
                    Type.GetType("TestPluginAssembly1.Implementations.Window,  TestProjects.TestPluginAssembly1"),
                    "TestPluginAssembly1.Implementations.Window");

                // Array type definitions
                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("arrayOfInterface1").ValueTypeInfo,
                    typeof(IInterface1[]), "SharedServices.Interfaces.IInterface1[]");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("enumerableOfArray").ValueTypeInfo,
                    typeof(System.Collections.Generic.IEnumerable<SharedServices.Interfaces.IInterface1[]>),
                    "System.Collections.Generic.IEnumerable<SharedServices.Interfaces.IInterface1[]>");

                Helpers.ValidateTypeInfo(configuration.TypeDefinitions.GetTypeDefinition("arraysOfGenericTypes").ValueTypeInfo,
                    typeof(SharedServices.Interfaces.Generic.IGeneric4_2<SharedServices.Interfaces.IInterface1[], SharedServices.Interfaces.IInterface2[]>[]),
                    "SharedServices.Interfaces.Generic.IGeneric4_2<SharedServices.Interfaces.IInterface1[], SharedServices.Interfaces.IInterface2[]>[]");

                // Plugin type definitions
                Helpers.ValidateTypeInfo(configuration.PluginsSetup.GetPluginSetup(Plugin1Name).TypeDefinitions.GetTypeDefinition("Generic1_1_of_Interface1_Impl1").ValueTypeInfo,
                    Type.GetType("SharedServices.Implementations.Generic.Generic1_1`1[[TestPluginAssembly1.Implementations.Interface1_Impl1, TestProjects.TestPluginAssembly1]], TestProjects.SharedServices"),
                    "SharedServices.Implementations.Generic.Generic1_1<TestPluginAssembly1.Implementations.Interface1_Impl1>");

                Helpers.ValidateTypeInfo(configuration.PluginsSetup.GetPluginSetup(Plugin1Name).TypeDefinitions.GetTypeDefinition("ReadOnlyListOfGenericType").ValueTypeInfo,
                    typeof(IReadOnlyList<Generic3_1<Interface1_Impl1>>),
                    "System.Collections.Generic.IReadOnlyList<SharedServices.Implementations.Generic.Generic3_1<SharedServices.Implementations.Interface1_Impl1>>");


            }, false);
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInModuleTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
                {
                    var expectedModuleType = _modulesAssembly.GetType("Modules.IoC.DiModule2");
                    var diModule = configuration.DependencyInjection.Modules.Modules.FirstOrDefault(x => x.DiModule.GetType() == expectedModuleType).DiModule;

                    Assert.IsNotNull(diModule);

                    Assert.AreSame(typeof(Interface1_Impl1),
                        diModule.GetType().GetProperty("Property1").GetValue(diModule).GetType());
                });
        }

        private void ReplaceTypRefAttributeWithTypeAndAssembly([NotNull] XmlElement xmlElement,
                                                               ConfigurationFileMutationType configurationFileMutationType)
        {
            if (configurationFileMutationType == ConfigurationFileMutationType.None)
                return;

            string typeRefAttributeName = null;
            string typeAttributeName = null;
            var assemblyAttributeName = ConfigurationFileAttributeNames.Assembly;

            if (xmlElement.HasAttribute(ConfigurationFileAttributeNames.TypeRef))
            {
                typeRefAttributeName = ConfigurationFileAttributeNames.TypeRef;
                typeAttributeName = ConfigurationFileAttributeNames.Type;
            }
            else if (xmlElement.HasAttribute(ConfigurationFileAttributeNames.InterfaceRef))
            {
                typeRefAttributeName = ConfigurationFileAttributeNames.InterfaceRef;
                typeAttributeName = ConfigurationFileAttributeNames.Interface;
            }
            else if (xmlElement.HasAttribute(ConfigurationFileAttributeNames.SerializerAggregatorTypeRef))
            {
                typeRefAttributeName = ConfigurationFileAttributeNames.SerializerAggregatorTypeRef;
                typeAttributeName = ConfigurationFileAttributeNames.SerializerAggregatorType;
            }

            if (typeRefAttributeName == null)
                return;

            var elementPath = GetElementPath(xmlElement);
            LogHelper.Context.Log.Info($"Started mutating element '{elementPath}'.");

            INamedTypeDefinitionElement namedTypeDefinitionElement = null;
            var typeRefAttributeValue = xmlElement.GetAttribute(typeRefAttributeName);

            xmlElement.RemoveAttribute(typeRefAttributeName);

            {
                var currentXmlElement = xmlElement;
                while (currentXmlElement != null && !currentXmlElement.Name.Equals("iocConfiguration", StringComparison.Ordinal))
                {
                    if (currentXmlElement.Name.Equals(ConfigurationFileElementNames.PluginSetup))
                    {
                        if (currentXmlElement.GetAttribute(ConfigurationFileAttributeNames.Plugin).Equals(Plugin1Name))
                        {
                            namedTypeDefinitionElement = _pluginTypeDefinitionsElement.GetTypeDefinition(typeRefAttributeValue);
                            break;
                        }

                        Assert.Fail("Test only Plugin1");
                    }
                    currentXmlElement = currentXmlElement.ParentNode as XmlElement;
                }
            }

            if (namedTypeDefinitionElement == null)
                namedTypeDefinitionElement = _typeDefinitionsElement.GetTypeDefinition(typeRefAttributeValue);

            Assert.IsNotNull(namedTypeDefinitionElement);

            var typeFullNameInConfigFile = GetConfigStyleTypeName(namedTypeDefinitionElement);

            xmlElement.SetAttributeValue(typeAttributeName, typeFullNameInConfigFile);


            LogHelper.Context.Log.Info($"Mutated element '{elementPath}': The value of attribute {typeRefAttributeName}='{typeRefAttributeValue}' was replaced with attribute {typeAttributeName}='{typeFullNameInConfigFile}'");

            if (configurationFileMutationType == ConfigurationFileMutationType.UseTypeAndAssembly)
            {
                xmlElement.SetAttributeValue(assemblyAttributeName, namedTypeDefinitionElement.ValueTypeInfo.Assembly.Alias);

                LogHelper.Context.Log.Info($"Mutated element '{elementPath}': Added attribute {assemblyAttributeName}='{namedTypeDefinitionElement.ValueTypeInfo.Assembly.Alias}'");
            }
        }

        private string GetElementPath(XmlElement xmlElement)
        {
            var pathStrBldr = new StringBuilder();

            XmlElement currXmlElement = xmlElement;
            LinkedList<XmlElement> xmlElementPaths = new LinkedList<XmlElement>();

            while (currXmlElement != null)
            {
                xmlElementPaths.AddFirst(currXmlElement);
                currXmlElement = currXmlElement.ParentNode as XmlElement;
            }

            return string.Join("/", xmlElementPaths.Select(x => x.Name));
        }

        private string GetConfigStyleTypeName(ITypeDefinitionElement typeDefinitionElement)
        {
            var typeNameStrBldr = new StringBuilder();

            typeNameStrBldr.Append(typeDefinitionElement.GetAttributeValue(ConfigurationFileAttributeNames.Type));

            if (typeDefinitionElement.GenericTypeParameters != null)
            {
                var childTypeDefinitionElements = typeDefinitionElement.GenericTypeParameters.TypeParameterElements;

                if (childTypeDefinitionElements.Count > 0)
                {
                    typeNameStrBldr.Append('[');

                    for (int i = 0; i < childTypeDefinitionElements.Count; ++i)
                    {
                        if (i > 0)
                            typeNameStrBldr.Append(", ");

                        typeNameStrBldr.Append(GetConfigStyleTypeName(childTypeDefinitionElements[i]));
                    }

                    typeNameStrBldr.Append(']');
                }
            }


            return typeNameStrBldr.ToString();
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInParametersSerializersEtcTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                Assert.IsInstanceOfType(configuration.ParameterSerializers.TypeBasedSimpleSerializerAggregator,
                    typeof(TypeBasedSimpleSerializerAggregator));

                var restTypeRefTestClass3Serializer = configuration.ParameterSerializers.TypeBasedSimpleSerializerAggregator.GetSerializerForType(typeof(TestTypeRefTestClass3));

                Assert.IsInstanceOfType(restTypeRefTestClass3Serializer, typeof(TestTypeRefTestClass3Serializer));

                var servicesInjectionTester = diContainer.Resolve<ClassToTestServicesInjection<TestTypeRefTestClass1>>();

                Assert.AreEqual(1, servicesInjectionTester.Implementations.Count);

                var testTypeRefTestClass1 = servicesInjectionTester.Implementations[0];


                Assert.IsInstanceOfType(testTypeRefTestClass1.Property1.Property1, typeof(Interface1_Impl1));
                Assert.IsInstanceOfType(testTypeRefTestClass1.Property3.Property1, typeof(Interface1_Impl1));

                Assert.AreEqual(5, testTypeRefTestClass1.Property2.Property1);
                Assert.AreEqual(7, testTypeRefTestClass1.Property4.Property1);
            }, true);
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInPluginModuleTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                var expectedModuleType = _plugin1ModulesAssembly.GetType("ModulesForPlugin1.IoC.DiModule2");
                var diModule = configuration.PluginsSetup.GetPluginSetup(Plugin1Name).DependencyInjection.Modules.Modules.FirstOrDefault(x => x.DiModule.GetType() == expectedModuleType).DiModule;

                Assert.IsNotNull(diModule);

                Assert.AreSame(_testPluginAssembly1.GetType("TestPluginAssembly1.Implementations.Door"),
                    diModule.GetType().GetProperty("Property1").GetValue(diModule).GetType());
            });
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInPluginSelfBoundServiceTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                var expectedServiceType = _testPluginAssembly1.GetType("TestPluginAssembly1.Interfaces.IDoor");
                var expectedImplementationType = _testPluginAssembly1.GetType("TestPluginAssembly1.Implementations.Door");

                var serviceElement = configuration.PluginsSetup.GetPluginSetup(Plugin1Name).DependencyInjection.Services.AllServices.First(
                    x => x.ServiceTypeInfo.Type == expectedServiceType);


                Assert.IsNotNull(serviceElement);

                Assert.AreEqual(1, serviceElement.Implementations.ToList().Count);
                Assert.IsNotNull(serviceElement.Implementations.FirstOrDefault(x => x.ValueTypeInfo.Type == expectedImplementationType));

                var door = diContainer.Resolve(expectedServiceType);
                Assert.AreEqual(door.GetType(), expectedImplementationType);
            });
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInPluginServiceAndImplementationsTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                var expectedServiceType = _testPluginAssembly1.GetType("TestPluginAssembly1.Interfaces.IDoor");
                var expectedImplementationType = _testPluginAssembly1.GetType("TestPluginAssembly1.Implementations.Door");

                var serviceElement = configuration.PluginsSetup.GetPluginSetup(Plugin1Name).DependencyInjection.Services.AllServices.FirstOrDefault(
                    x => x.ServiceTypeInfo.Type == expectedServiceType);

                Assert.IsNotNull(serviceElement);

                Assert.AreEqual(1, serviceElement.Implementations.ToList().Count);
                Assert.IsNotNull(serviceElement.Implementations.FirstOrDefault(x => x.ValueTypeInfo.Type == expectedImplementationType));

                var door = diContainer.Resolve(expectedServiceType);
                Assert.AreSame(door.GetType(), expectedImplementationType);
             });
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInPluginSettingsAndOverriddigTypeDefinitionTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                var settings = diContainer.Resolve<ISettings>();
                settings.GetSettingValueOrThrow<Generic1_1<Interface1_Impl1>>("GenericSetting1");

                var pluginDataRepository = diContainer.Resolve<IPluginDataRepository>();

                var expectedSettingType = Type.GetType("SharedServices.Implementations.Generic.Generic1_1`1[[TestPluginAssembly1.Implementations.Interface1_Impl1, TestProjects.TestPluginAssembly1]], TestProjects.SharedServices");
                Assert.AreSame(expectedSettingType,
                    pluginDataRepository.GetPluginData(Plugin1Name).Settings.GetSettingValueOrThrow<object>("PluginGenericSetting1").GetType());
            });
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInSelfBoundServiceTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                var serviceElement = configuration.DependencyInjection.Services.AllServices.First(
                    x => x.ServiceTypeInfo.Type == typeof(Interface1_Impl1));

                Assert.IsNotNull(serviceElement);
                Assert.AreSame(typeof(Interface1_Impl1), serviceElement.ServiceTypeInfo.Type);

                var servicesInjectionTester = diContainer.Resolve<ClassToTestServicesInjection<Interface1_Impl1>>();

                Assert.AreEqual(1, servicesInjectionTester.Implementations.Count);
                Assert.AreEqual(1, servicesInjectionTester.Implementations.Where(x => x is Interface1_Impl1).ToList().Count);
            });
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInServiceAndImplementationsTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                var serviceElement = configuration.DependencyInjection.Services.AllServices.First(x => x.ServiceTypeInfo.Type == typeof(IGeneric1_1<Interface1_Impl1>));

                var servicesInjectionTester = diContainer.Resolve<ClassToTestServicesInjection<IGeneric1_1<Interface1_Impl1>>>();

                Assert.AreEqual(3, servicesInjectionTester.Implementations.Count);
                Assert.AreEqual(3, servicesInjectionTester.Implementations.Where(x => x is Generic1_1<Interface1_Impl1>).ToList().Count);
            });
        }

        [DataTestMethod]
        [DataRow(DiImplementationType.Autofac)]
        [DataRow(DiImplementationType.Ninject)]
        public void TypeRefInSettingsTest(DiImplementationType diImplementationType)
        {
            LoadConfigurationFileAndRunTest(diImplementationType, (diContainer, configuration) =>
            {
                var settings = diContainer.Resolve<ISettings>();
                settings.GetSettingValueOrThrow<Generic1_1<Interface1_Impl1>>("GenericSetting1");
            });
        }

        #endregion

        #region Nested Types

        public enum ConfigurationFileMutationType
        {
            None,
            UseType,
            UseTypeAndAssembly
        }

        public class TestModule1 : ModuleAbstr
        {
            #region Member Functions

            protected override void AddServiceRegistrations()
            {
                Bind<ClassToTestServicesInjection<Interface1_Impl1>>().ToSelf();
                Bind<ClassToTestServicesInjection<IGeneric1_1<Interface1_Impl1>>>().ToSelf();
                Bind<ClassToTestServicesInjection<TestTypeRefTestClass1>>().ToSelf();
            }

            #endregion
        }

        public class TestTypeRefTestClass1
        {
            #region  Constructors

            public TestTypeRefTestClass1(TestTypeRefTestClass2 param1, TestTypeRefTestClass3 param2)
            {
                Property1 = param1;
                Property2 = param2;
            }

            #endregion

            #region Member Functions

            public TestTypeRefTestClass2 Property1 { get; }
            public TestTypeRefTestClass3 Property2 { get; }
            public TestTypeRefTestClass2 Property3 { get; set; }
            public TestTypeRefTestClass3 Property4 { get; set; }

            #endregion
        }

        public class TestTypeRefTestClass2
        {
            #region  Constructors

            public TestTypeRefTestClass2(IInterface1 param1)
            {
                Property1 = param1;
            }

            #endregion

            #region Member Functions

            public IInterface1 Property1 { get; }

            #endregion
        }

        public class TestTypeRefTestClass3
        {
            #region  Constructors

            public TestTypeRefTestClass3(int param1)
            {
                Property1 = param1;
            }

            #endregion

            #region Member Functions

            public int Property1 { get; }

            #endregion
        }

        public class TestTypeRefTestClass3Serializer : ITypeBasedSimpleSerializer
        {
            #region ITypeBasedSimpleSerializer Interface Implementation

            public Type SerializedType { get; } = typeof(TestTypeRefTestClass3);

            public bool TryDeserialize(string valueToDeserialize, out object deserializedValue)
            {
                if (int.TryParse(valueToDeserialize, out var intValue))
                {
                    deserializedValue = new TestTypeRefTestClass3(intValue);
                    return true;
                }

                deserializedValue = null;
                return false;
            }

            public bool TrySerialize(object valueToSerialize, out string serializedValue)
            {
                if (valueToSerialize is TestTypeRefTestClass3 testTypeRefTestClass3)
                {
                    serializedValue = testTypeRefTestClass3.Property1.ToString();
                    return true;
                }

                serializedValue = string.Empty;
                return false;
            }

            #endregion
        }

        #endregion
    }
}