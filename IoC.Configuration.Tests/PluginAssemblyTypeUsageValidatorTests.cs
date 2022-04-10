using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OROptimizer.Diagnostics.Log;
using TestsSharedLibrary;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests
{
    [TestFixture]
    public class PluginAssemblyTypeUsageValidatorTests
    {
        private static readonly string TestFilesFolder = Helpers.GetTestFilesFolderPath();

        private const string OroptimizerSharedAssemblyAlias = "oroptimizer_shared";
        private const string IoCConfigAssemblyAlias = "ioc_config";
        private const string DynamicallyLoadedAssembly1Alias = "dynamic1";
        private const string DynamicallyLoadedAssembly2Alias = "dynamic2";
        private const string TestPluginAssembly1Alias = "Plugin1";
        private const string TestPluginAssembly2Alias = "Plugin2";
        private const string TestPluginAssembly3Alias = "Plugin3";

        private const string SharedTestServicesAssemblyAlias = "shared_services";
        private const string IoCConfigurationTestsAssemblyAlias = "tests";
        private const string MsCoreLibAssemblyAlias = "mscorlib";

        private PluginAssemblyTypeUsageValidator _pluginAssemblyTypeUsageValidator = new PluginAssemblyTypeUsageValidator();
   
        private ConfigurationMockHelper _configurationMockHelper;

        private ITypeInfo _plugin1TypeInfo;
        private ITypeInfo _plugin2TypeInfo;

        [TearDown]
        public void TestCleanup()
        {
            _configurationMockHelper.Dispose();
        }

        [SetUp]
        public void TestInitialize()
        {
            TestsHelper.SetupLogger();
            Log4Tests.LogLevel = LogLevel.Info;

            _configurationMockHelper = new ConfigurationMockHelper(new string[]
                {
                    "DynamicallyLoadedDlls"
                },
                new string[] { "Plugin1", "Plugin2", "Plugin3" },
                new ConfigurationMockHelper.AssemblyInfo[]
                {
                    new ConfigurationMockHelper.AssemblyInfo(OroptimizerSharedAssemblyAlias, "OROptimizer.Shared", Helpers.TestsEntryAssemblyFolder),
                    new ConfigurationMockHelper.AssemblyInfo(IoCConfigAssemblyAlias, "IoC.Configuration", Helpers.TestsEntryAssemblyFolder),
                    new ConfigurationMockHelper.AssemblyInfo(IoCConfigurationTestsAssemblyAlias, "IoC.Configuration.Tests", Helpers.TestsEntryAssemblyFolder),
                    new ConfigurationMockHelper.AssemblyInfo(SharedTestServicesAssemblyAlias, "TestProjects.SharedServices", Helpers.TestsEntryAssemblyFolder),

                    new ConfigurationMockHelper.AssemblyInfo(MsCoreLibAssemblyAlias, typeof(int).Assembly.GetName().Name, Path.GetDirectoryName(typeof(int).Assembly.Location)),

                    new ConfigurationMockHelper.AssemblyInfo(DynamicallyLoadedAssembly1Alias, "TestProjects.DynamicallyLoadedAssembly1", Path.Combine(TestFilesFolder, "DynamicallyLoadedDlls")),
                    new ConfigurationMockHelper.AssemblyInfo(DynamicallyLoadedAssembly2Alias, "TestProjects.DynamicallyLoadedAssembly2", Path.Combine(TestFilesFolder, "DynamicallyLoadedDlls")),

                    new ConfigurationMockHelper.AssemblyInfo(TestPluginAssembly1Alias, "TestProjects.TestPluginAssembly1", Path.Combine(TestFilesFolder, "PluginDlls", "Plugin1")),
                    new ConfigurationMockHelper.AssemblyInfo(TestPluginAssembly2Alias, "TestProjects.TestPluginAssembly2", Path.Combine(TestFilesFolder, "PluginDlls", "Plugin2")),
                    new ConfigurationMockHelper.AssemblyInfo(TestPluginAssembly3Alias, "TestProjects.TestPluginAssembly3", Path.Combine(TestFilesFolder, "PluginDlls", "Plugin3"))
                });


            TypeInfo.CreateNonArrayTypeInfo(Type.GetType("SharedServices.Interfaces.IInterface1, TestProjects.SharedServices"),
                _configurationMockHelper.ConfigurationMock.Object.Assemblies.GetAssemblyByAlias(SharedTestServicesAssemblyAlias), new ITypeInfo[] { });

            _plugin1TypeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("TestPluginAssembly1.Interfaces.IRoom, TestProjects.TestPluginAssembly1"),
                _configurationMockHelper.ConfigurationMock.Object.Assemblies.GetAssemblyByAlias(TestPluginAssembly1Alias), new ITypeInfo[] { });
            _plugin2TypeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("TestPluginAssembly2.Interfaces.ICar, TestProjects.TestPluginAssembly2"),
                _configurationMockHelper.ConfigurationMock.Object.Assemblies.GetAssemblyByAlias(TestPluginAssembly2Alias), new ITypeInfo[] { });
            TypeInfo.CreateNonArrayTypeInfo(Type.GetType("TestPluginAssembly3.Implementations.Plugin3, TestProjects.TestPluginAssembly3"),
                _configurationMockHelper.ConfigurationMock.Object.Assemblies.GetAssemblyByAlias(TestPluginAssembly3Alias), new ITypeInfo[] { });
        }

        [Test]
        public void ValidateInterfaceSubclassing()
        {
            Type[] canHaveChildElementsThatUsePluginTypeInNonPluginSectionImplementationTypes = new Type[]
            {
                typeof(ServiceElementBase),
                typeof(ParameterSerializersCollection),
                typeof(CollectionValueElementBase),
                typeof(TypeDefinitionsElement)
            };

            Type[] typedItems =
            {
                typeof(IServiceImplementationElement),
                typeof(IParameterSerializer),
                typeof(INamedTypeDefinitionElement),
                typeof(ITypeDefinitionElement)
            };

            foreach (var canHaveChildElementsThatUsePluginTypeInNonPluginSectionImplementationType in canHaveChildElementsThatUsePluginTypeInNonPluginSectionImplementationTypes)
            {
                Assert.IsNotNull(canHaveChildElementsThatUsePluginTypeInNonPluginSectionImplementationType.GetInterfaces().FirstOrDefault(
                    x => x == typeof(ICanHaveChildElementsThatUsePluginTypeInNonPluginSection)));
            }

            foreach (var typedItem in typedItems)
            {
                Assert.IsNotNull(typedItem.GetInterfaces().FirstOrDefault(
                    x => x == typeof(ITypedItem)));
            }
        }

        [Test]
        public void TypeHasNoPluginsValidationTest()
        {
            var typeInfoMock = CreateTypeInfoMock(typeof(SharedServices.Interfaces.IInterface1));

            var mockValues = SetupTypedElementMock(typeInfoMock.Object);
            _pluginAssemblyTypeUsageValidator.Validate(mockValues.valueElementMock.Object, typeInfoMock.Object);
        }


        [Test]
        public void TypeHasPluginsInNonPluginSectionThatDoesNotAllowPluginTypesTest()
        {
            var typeInfoMock = CreateTypeInfoMock(typeof(SharedServices.Interfaces.IInterface1), _plugin1TypeInfo);

            var mockValues = SetupTypedElementMock(typeInfoMock.Object);

            TestExpectedConfigurationParseException(() =>
            {
                _pluginAssemblyTypeUsageValidator.Validate(mockValues.valueElementMock.Object, typeInfoMock.Object);
            },
            mockValues.valueElementMock.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TypeHasPluginsInNonPluginSectionThatAllowsPluginTypesTest(bool typeUsesMultiplePlugins)
        {
            ITypeInfo[] pluginTypeInfos;

            if (typeUsesMultiplePlugins)
                pluginTypeInfos = new[] { _plugin1TypeInfo, _plugin2TypeInfo };
            else
                pluginTypeInfos = new[] { _plugin1TypeInfo };

            var typeInfoMock = CreateTypeInfoMock(typeof(SharedServices.Interfaces.IInterface1), pluginTypeInfos);

            var mockValues = SetupTypedElementMock(typeInfoMock.Object);

            mockValues.settingsElementMock.As<ICanHaveChildElementsThatUsePluginTypeInNonPluginSection>();

            if (typeUsesMultiplePlugins)
            {
                TestExpectedConfigurationParseException(() =>
                {
                    _pluginAssemblyTypeUsageValidator.Validate(mockValues.valueElementMock.Object, typeInfoMock.Object);
                },
                mockValues.valueElementMock.Object);
            }
            else
            {
                _pluginAssemblyTypeUsageValidator.Validate(mockValues.valueElementMock.Object, typeInfoMock.Object);
            }
        }

        [Flags]
        private enum PluginTypeUsageInNonPluginSectionScenarios : UInt64
        {
            None = 0,
            RootTypedElementImplementsTypedItemInterface = 1,
            RootTypedElementUsesPlugin = 2,
            TestedTypedElementUsesSamePluginAsRootTypedElement = 4,
            TestedTypedElementUsesSinglePlugin = 8,

            All = RootTypedElementImplementsTypedItemInterface |
             PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementUsesPlugin |
             PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSamePluginAsRootTypedElement |
             PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSinglePlugin
        }

        [Test]
        public void TypeHasPluginAndHasValidAncestorThatUsesSamePluginInNonPluginSectionThatAllowsPluginTypesTest()
        {
            PluginTypeUsageInNonPluginSectionScenarios[] testScenario1Values = new[] {
                PluginTypeUsageInNonPluginSectionScenarios.None,
                PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementImplementsTypedItemInterface };

            PluginTypeUsageInNonPluginSectionScenarios[] testScenario2Values = new[] {
                PluginTypeUsageInNonPluginSectionScenarios.None,
                PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementUsesPlugin };

            PluginTypeUsageInNonPluginSectionScenarios[] testScenario3Values = new[] {
                PluginTypeUsageInNonPluginSectionScenarios.None,
                PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSamePluginAsRootTypedElement };

            PluginTypeUsageInNonPluginSectionScenarios[] testScenario4Values = new[] {
                PluginTypeUsageInNonPluginSectionScenarios.None,
                PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSinglePlugin };

            foreach (var testScenario1 in testScenario1Values)
            {
                foreach (var testScenario2 in testScenario2Values)
                {
                    foreach (var testScenario3 in testScenario3Values)
                    {
                        foreach (var testScenario4 in testScenario4Values)
                        {
                            LogHelper.Context.Log.InfoFormat("{0}({1}={2},{3}={4}, {5}={6}, {7}={8})",
                                nameof(TypeHasPluginAndHasValidAncestorThatUsesSamePluginInNonPluginSectionThatAllowsPluginTypesTest),
                                    PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementImplementsTypedItemInterface,
                                    (int)(testScenario1 & PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementImplementsTypedItemInterface),

                                    PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementUsesPlugin,
                                    (int)(testScenario2 & PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementUsesPlugin),

                                    PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSamePluginAsRootTypedElement,
                                    (int)(testScenario3 & PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSamePluginAsRootTypedElement),

                                    PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSinglePlugin,
                                    (int)(testScenario4 & PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSinglePlugin));

                            TestInitialize();
                            for (var numOfLevelsDeep = 1; numOfLevelsDeep <= 2; ++numOfLevelsDeep)
                                TypeHasPluginAndHasValidAncestorThatUsesSamePluginInNonPluginSectionThatAllowsPluginTypesTest(
                                    testScenario1 | testScenario2 | testScenario3 | testScenario4, numOfLevelsDeep);
                        }
                    }
                }
            }
        }
             
        private void TypeHasPluginAndHasValidAncestorThatUsesSamePluginInNonPluginSectionThatAllowsPluginTypesTest(
        PluginTypeUsageInNonPluginSectionScenarios pluginTypeUsageInNonPluginSectionScenarios, int numOfLevelsDeep)
        {
            Assert.IsTrue(numOfLevelsDeep >= 1);

            var rootElementMock = _configurationMockHelper.CreateElementMock<ISettingsElement, IConfiguration>(ConfigurationFileElementNames.Settings,
                _configurationMockHelper.ConfigurationMock);

            // The scenario when plugin is used in a section that does not have ICanHaveChildElementsThatUsePluginTypeInNonPluginSection as 
            // a root element is already tested in test TypeHasPluginsInNonPluginSectionThatDoesNotAllowPluginTypesTest
            rootElementMock.As<ICanHaveChildElementsThatUsePluginTypeInNonPluginSection>();

            var rootTypedElementMock = _configurationMockHelper.CreateElementMock<IInjectedProperties, ISettingsElement>(
                ConfigurationFileElementNames.InjectedProperties, rootElementMock);

            if ((Int64)(pluginTypeUsageInNonPluginSectionScenarios & PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementImplementsTypedItemInterface) > 0)
            {
                var typedItemElementMock = rootTypedElementMock.As<ITypedItem>();

                ITypeInfo[] rootTypedElementPluginTypInfos;

                if ((Int64)(pluginTypeUsageInNonPluginSectionScenarios & PluginTypeUsageInNonPluginSectionScenarios.RootTypedElementUsesPlugin) > 0)
                    rootTypedElementPluginTypInfos = new ITypeInfo[] { _plugin1TypeInfo };
                else
                    rootTypedElementPluginTypInfos = new ITypeInfo[0];

                var rootElementTypeInfoMock = CreateTypeInfoMock(typeof(SharedServices.Interfaces.IInterface1), rootTypedElementPluginTypInfos);

                typedItemElementMock.Setup(x => x.ValueTypeInfo).Returns(() => rootElementTypeInfoMock.Object);
            }

            Mock<IInjectedProperties> currentParentElementMock = rootTypedElementMock;
            Mock<IInjectedProperties> currElementMock = null;

            for (int i = 0; i < numOfLevelsDeep; i++)
            {
                currElementMock = _configurationMockHelper.CreateElementMock<IInjectedProperties, IInjectedProperties>(
                    ConfigurationFileElementNames.InjectedProperties, currentParentElementMock);
                currentParentElementMock = currElementMock;
            }

            List<ITypeInfo> pluginTypeInfos = new List<ITypeInfo>();

            if ((pluginTypeUsageInNonPluginSectionScenarios & PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSamePluginAsRootTypedElement) > 0)
            {
                pluginTypeInfos.Add(_plugin1TypeInfo);
            }
            else
            {
                pluginTypeInfos.Add(_plugin2TypeInfo);
            }

            if ((pluginTypeUsageInNonPluginSectionScenarios & PluginTypeUsageInNonPluginSectionScenarios.TestedTypedElementUsesSinglePlugin) == 0)
                pluginTypeInfos.Add(pluginTypeInfos[0] == _plugin2TypeInfo ? _plugin1TypeInfo : _plugin2TypeInfo);

            var typeInfoMock = CreateTypeInfoMock(typeof(SharedServices.Interfaces.IInterface2), pluginTypeInfos.ToArray());

            if ((pluginTypeUsageInNonPluginSectionScenarios & PluginTypeUsageInNonPluginSectionScenarios.All) == PluginTypeUsageInNonPluginSectionScenarios.All)
            {
                _pluginAssemblyTypeUsageValidator.Validate(currElementMock.Object, typeInfoMock.Object);
            }
            else
            {
                TestExpectedConfigurationParseException(() =>
                {
                    _pluginAssemblyTypeUsageValidator.Validate(currElementMock.Object, typeInfoMock.Object);
                },
                currElementMock.Object);
            }
        }
     
        public enum UsingPluguinTypesInPluginSetupSectionTestScenarios
        {
            ElementUsesNoPlugin = 0,
            ElementUsesSamePlugin = 1,
            ElementUsesDifferentPlugin = 2,
            ElementUsesMultiplePlugins = 3
        }
        
        [TestCase(false, UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesNoPlugin)]
        [TestCase(false, UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesSamePlugin)]
        [TestCase(false, UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesDifferentPlugin)]
        [TestCase(false, UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesMultiplePlugins)]

        [TestCase(true, UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesNoPlugin)]
        [TestCase(true, UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesSamePlugin)]
        [TestCase(true, UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesDifferentPlugin)]
        [TestCase(true, UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesMultiplePlugins)]
        public void UsingPluguinTypesInPluginSetupSection(bool useTwoLevels, UsingPluguinTypesInPluginSetupSectionTestScenarios usingPluguinTypesInPluginSetupSectionTestScenarios)
        {
            var pluginData = _configurationMockHelper.GetPluginData("Plugin1");

            Mock<IInjectedProperties> currElementMock = null;

            if (useTwoLevels)
            {
                var parentElementMock = _configurationMockHelper.CreateElementMock<IInjectedProperties, IPluginSetup>(ConfigurationFileElementNames.InjectedProperties, pluginData.PluginSetupMock);
                currElementMock = _configurationMockHelper.CreateElementMock<IInjectedProperties, IInjectedProperties>(ConfigurationFileElementNames.InjectedProperties, parentElementMock);
            }
            else
            {
                currElementMock = _configurationMockHelper.CreateElementMock<IInjectedProperties, IPluginSetup>(ConfigurationFileElementNames.InjectedProperties, pluginData.PluginSetupMock);
            }

            List<ITypeInfo> pluginTypeInfos = new List<ITypeInfo>();

            if (usingPluguinTypesInPluginSetupSectionTestScenarios != UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesNoPlugin)
            {
                if (usingPluguinTypesInPluginSetupSectionTestScenarios == UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesSamePlugin ||
                    usingPluguinTypesInPluginSetupSectionTestScenarios == UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesMultiplePlugins)
                    pluginTypeInfos.Add(_plugin1TypeInfo);
                else
                    pluginTypeInfos.Add(_plugin2TypeInfo);

                if (usingPluguinTypesInPluginSetupSectionTestScenarios == UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesMultiplePlugins)
                    pluginTypeInfos.Add(pluginTypeInfos[0] == _plugin2TypeInfo ? _plugin1TypeInfo : _plugin2TypeInfo);
            }

            var typeInfoMock = CreateTypeInfoMock(typeof(SharedServices.Interfaces.IInterface2), pluginTypeInfos.ToArray());

            if (usingPluguinTypesInPluginSetupSectionTestScenarios == UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesNoPlugin ||
                usingPluguinTypesInPluginSetupSectionTestScenarios == UsingPluguinTypesInPluginSetupSectionTestScenarios.ElementUsesSamePlugin)
            {
                _pluginAssemblyTypeUsageValidator.Validate(currElementMock.Object, typeInfoMock.Object);
            }
            else
            {
                TestExpectedConfigurationParseException(() =>
                {
                    _pluginAssemblyTypeUsageValidator.Validate(currElementMock.Object, typeInfoMock.Object);
                },
                currElementMock.Object);
            }
        }

        private (Mock<ISettingsElement> settingsElementMock, Mock<IValueInitializerElement> valueElementMock) SetupTypedElementMock(ITypeInfo typeInfo)
        {
            var settingsElementMock = _configurationMockHelper.CreateElementMock<ISettingsElement, IConfiguration>(ConfigurationFileElementNames.Settings,
                _configurationMockHelper.ConfigurationMock);

            var valueElementMock = _configurationMockHelper.CreateElementMock<IValueInitializerElement, ISettingsElement>(
                ConfigurationFileElementNames.ValueInjectedObject, settingsElementMock);

            valueElementMock.Setup(x => x.ValueTypeInfo).Returns(typeInfo);

            return (settingsElementMock, valueElementMock);
        }

        private Mock<ITypeInfo> CreateTypeInfoMock([NotNull] Type rootType, [CanBeNull, ItemNotNull] params ITypeInfo[] uniquePluginTypes)
        {
            var typeInfoMock = new Mock<ITypeInfo>();

            var uniquePluginTypesList = uniquePluginTypes == null ? new List<ITypeInfo>() : new List<ITypeInfo>(uniquePluginTypes);

            typeInfoMock.SetupGet(x => x.Type).Returns(rootType);
            typeInfoMock.Setup(x => x.GetUniquePluginTypes()).Returns(uniquePluginTypesList);
            typeInfoMock.SetupGet(x => x.TypeCSharpFullName).Returns(rootType.FullName);
            typeInfoMock.Setup(x => x.Assembly).Returns(() => _configurationMockHelper.ConfigurationMock.Object.Assemblies.AllAssemblies.First(
                x => x.Name.Equals(rootType.Assembly.GetName().Name, StringComparison.OrdinalIgnoreCase)));

            var genericParameters = new List<ITypeInfo>();

            if (uniquePluginTypes?.Length > 0)
                genericParameters.AddRange(uniquePluginTypes);

            typeInfoMock.Setup(x => x.GenericTypeParameters).Returns(genericParameters);


            return typeInfoMock;
        }
        private void TestExpectedConfigurationParseException([NotNull] Action actionResultingInException,
                                                             [NotNull] IConfigurationFileElement elementWithInvalidTypeData,
                                                             [CanBeNull] Action<ConfigurationParseException> doAdditionalTests = null)
        {
            Helpers.TestExpectedConfigurationParseException<ConfigurationParseException>(actionResultingInException, elementWithInvalidTypeData.GetType(), null, false,
            (expectedException) =>
            {
                Assert.AreSame(elementWithInvalidTypeData, expectedException.ConfigurationFileElement);
                doAdditionalTests?.Invoke(expectedException);
            });
        }
    }
}
