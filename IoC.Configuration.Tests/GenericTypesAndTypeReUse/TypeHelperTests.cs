using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using SharedServices.Interfaces;
using System;
using System.IO;
using System.Linq;
using TestsHelper = TestsSharedLibrary.TestsHelper;

namespace IoC.Configuration.Tests.GenericTypesAndTypeReUse
{
    [TestFixture]
    public class TypeHelperTests
    {
        private static readonly string TestFilesFolder = Helpers.GetTestFilesFolderPath();

        private const string TypeDef1 = "TypeDef1";
        private const string TypeDef2 = "TypeDef2";

        private const string OroptimizerSharedAssemblyAlias = "oroptimizer_shared";
        private const string IoCConfigAssemblyAlias = "ioc_config";
        private const string DynamicallyLoadedAssembly1Alias = "dynamic1";
        private const string DynamicallyLoadedAssembly2Alias = "dynamic2";
        private const string TestPluginAssembly1Alias = "Plugin1";
        private const string TestPluginAssembly2Alias = "Plugin2";

        private const string SharedTestServicesAssemblyAlias = "shared_services";
        private const string IoCConfigurationTestsAssemblyAlias = "tests";
        private const string MsCoreLibAssemblyAlias = "mscorlib";

        private AssemblyLocator _assemblyLocator;
        private TypeHelper _typeHelper;

        private ConfigurationMockHelper _configurationMockHelper;

        [TearDown]
        public void TestCleanup()
        {
            _configurationMockHelper.Dispose();
        }

        [SetUp]
        public void TestInitialize()
        {
            TestsHelper.SetupLogger();

            _configurationMockHelper = new ConfigurationMockHelper(new string[]
                {
                    "DynamicallyLoadedDlls"
                },
                new string[] { "Plugin1", "Plugin2" },
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
                    new ConfigurationMockHelper.AssemblyInfo(TestPluginAssembly2Alias, "TestProjects.TestPluginAssembly2", Path.Combine(TestFilesFolder, "PluginDlls", "Plugin2"))
                });

            _configurationMockHelper.ConfigureTypeDefinitions(new ConfigurationMockHelper.TypeDefinitionInfo[]
            {
                new ConfigurationMockHelper.TypeDefinitionInfo(TypeDef1, typeof(IInterface1).FullName),
                new ConfigurationMockHelper.TypeDefinitionInfo(TypeDef2, "System.Collections.Generic.IEnumerable[SharedServices.Interfaces.IInterface1]"),
            });

            var pluginAssemblyTypeUsageValidatorMock = new Mock<IPluginAssemblyTypeUsageValidator>();
            pluginAssemblyTypeUsageValidatorMock.Setup(x => x.Validate(It.IsAny<IConfigurationFileElement>(), It.IsAny<ITypeInfo>()));

            _assemblyLocator = new AssemblyLocator(() => _configurationMockHelper.ConfigurationMock.Object, Helpers.TestsEntryAssemblyFolder);
            _typeHelper = new TypeHelper(_assemblyLocator, new TypeParser(), pluginAssemblyTypeUsageValidatorMock.Object);
        }

        [Test]
        public void UsingTypesInMsCorLibAssemblyWithoutAddingAssemblyTests()
        {
            UsingTypesInPredefinedAssemblyWithoutAddingAssemblyTests(typeof(int), MsCoreLibAssemblyAlias);
        }

        [Test]
        public void UsingTypesInIoCConfigAssemblyWithoutAddingAssemblyTests()
        {
            UsingTypesInPredefinedAssemblyWithoutAddingAssemblyTests(typeof(IoC.Configuration.IAssembly), IoCConfigAssemblyAlias);
        }

        [Test]
        public void UsingTypesInOroptimizerSharedAssemblyWithoutAddingAssemblyTests()
        {
            UsingTypesInPredefinedAssemblyWithoutAddingAssemblyTests(typeof(OROptimizer.IGlobalsCore), OroptimizerSharedAssemblyAlias);
        }

        private void UsingTypesInPredefinedAssemblyWithoutAddingAssemblyTests(Type typeInPredefinedAssembly, string assemblyAliasToExclude)
        {
            string typeFullName = typeInPredefinedAssembly.FullName;
           
            // First lets test that we can use assembly alias, if it is in assemblies element
            Assert.IsNotNull(_configurationMockHelper.ConfigurationMock.Object.Assemblies.GetAssemblyByAlias(assemblyAliasToExclude));
            Assert.IsNotNull(_configurationMockHelper.ConfigurationMock.Object.Assemblies.AllAssemblies.FirstOrDefault(x =>
                x.Name.Equals(typeInPredefinedAssembly.Assembly.GetName().Name)));

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, typeFullName),
                new AttributeInfo(ConfigurationFileAttributeNames.Assembly, assemblyAliasToExclude));

            var typeInfo = _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            Helpers.ValidateAreEqual(typeInfo, typeInPredefinedAssembly);

            // Now lets make sure the assemblies is not listed in assemblies element and lets make sure
            // we can still reference the type, if we do not use assembly attribute
            _configurationMockHelper.GetAssemblyElementData(assemblyAliasToExclude).IsExcluded = true;

            
            Assert.IsNull(_configurationMockHelper.ConfigurationMock.Object.Assemblies.GetAssemblyByAlias(assemblyAliasToExclude));
            Assert.IsNull(_configurationMockHelper.ConfigurationMock.Object.Assemblies.AllAssemblies.FirstOrDefault(x =>
                x.Name.Equals(typeInPredefinedAssembly.Assembly.GetName().Name)));

            elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, typeFullName), null);

            typeInfo = _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            Helpers.ValidateAreEqual(typeInfo, typeInPredefinedAssembly);

            // Now lets use assembly attribute again, and lets make sure that this results in failure
            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    null,
                    new AttributeInfo(ConfigurationFileAttributeNames.Type, typeFullName),
                    new AttributeInfo(ConfigurationFileAttributeNames.Assembly, assemblyAliasToExclude));

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);

        }

        
        [TestCase(ConfigurationFileElementNames.ValueBoolean, typeof(bool))]
        [TestCase(ConfigurationFileElementNames.ValueByte, typeof(byte))]
        [TestCase(ConfigurationFileElementNames.ValueDateTime, typeof(DateTime))]
        [TestCase(ConfigurationFileElementNames.ValueDouble, typeof(double))]
        [TestCase(ConfigurationFileElementNames.ValueInt16, typeof(Int16))]
        [TestCase(ConfigurationFileElementNames.ValueInt32, typeof(Int32))]
        [TestCase(ConfigurationFileElementNames.ValueInt64, typeof(Int64))]
        [TestCase(ConfigurationFileElementNames.ValueString, typeof(string))]
        public void PrimitiveTypeElementTests(string elementName, Type expectedType)
        {
            var elementMock = CreateTypedElementMockWithParent<IParameterElement, IParameters>(elementName, null, null, null);
            var typeInfo = _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            Helpers.ValidateAreEqual(typeInfo, expectedType);
        }


        [Test]
        public void UsingNonStandardTypeAttributeTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo("typeAttr1", typeFullName),
                null);

            var typeInfo = _typeHelper.GetTypeInfo(elementMock.Object, "typeAttr1", ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            Helpers.ValidateAreEqual(typeInfo, typeof(IInterface1));

            TestExpectedConfigurationParseException(() =>
            {
                _typeHelper.GetTypeInfo(elementMock.Object, "typeAttr2", ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);
        }

        [Test]
        public void UsingNonStandardTypeRefAttributeTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                new AttributeInfo("typeRefAttr1", TypeDef1), null, null);

            var typeInfo = _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                "typeRefAttr1");

            Helpers.ValidateAreEqual(typeInfo, typeof(IInterface1));

            TestExpectedConfigurationParseException(() =>
            {
                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    "typeRefAttr2");
            }, typeof(IServiceElement), null);
        }

        [Test]
        public void UsingNonStandardAssemblyAttributeTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo("typeAttr1", typeFullName),
                new AttributeInfo("assemblyAttr1", SharedTestServicesAssemblyAlias));

            var typeInfo = _typeHelper.GetTypeInfo(elementMock.Object, "typeAttr1", "assemblyAttr1",
                ConfigurationFileAttributeNames.TypeRef);

            Helpers.ValidateAreEqual(typeInfo, typeof(IInterface1));

            TestExpectedConfigurationParseException(() =>
            {
                // Lets tried to find the type in wrong assembly 'dynamic1'.
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    null,
                    new AttributeInfo("typeAttr1", typeFullName),
                    new AttributeInfo("assemblyAttr1", DynamicallyLoadedAssembly1Alias));

                _typeHelper.GetTypeInfo(elementMock.Object, "typeAttr1", "assemblyAttr1",
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);
        }

        [Test]
        public void InvalidPluginAssemblyTypeUsageDetectorFailureTest()
        {
            Mock<IServiceElement> CreateMockElement()
            {
                return CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    new AttributeInfo(ConfigurationFileAttributeNames.TypeRef, TypeDef1),
                    null, null);
            }

            var elementMock = CreateMockElement();

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                var pluginAssemblyTypeUsageValidatorMock = new Mock<IPluginAssemblyTypeUsageValidator>();
                pluginAssemblyTypeUsageValidatorMock.Setup(x => x.Validate(It.IsAny<IConfigurationFileElement>(), It.IsAny<ITypeInfo>())).Throws(
                    new ConfigurationParseException(elementMock.Object, "Plugin type usage validation failed."));

                _typeHelper = new TypeHelper(_assemblyLocator, new TypeParser(), pluginAssemblyTypeUsageValidatorMock.Object);

                elementMock = CreateMockElement();
                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void TypeParserFailureTest(bool useTypeParserMockThatAlwaysFails)
        {
            string typeFullName = "System.Collections.Generic.IEnumerable[SharedServices.Interfaces.IInterface1]";

            Mock<IServiceElement> CreateMockElement()
            {
                return CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    null,
                    new AttributeInfo(ConfigurationFileAttributeNames.Type, typeFullName), null);
            }

            var elementMock = CreateMockElement();

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                var pluginAssemblyTypeUsageValidatorMock = new Mock<IPluginAssemblyTypeUsageValidator>();

                ITypeParser typeParser;

                if (useTypeParserMockThatAlwaysFails)
                {
                    var typeParserMock = new Mock<ITypeParser>();
                    typeParserMock.Setup(x => x.Parse(typeFullName)).Throws(new ParseTypeException($"Failed to parse the type '{typeFullName}'", 0));
                    typeParser = typeParserMock.Object;
                }
                else
                {
                    typeParser = new TypeParser();
                    typeFullName = typeFullName.Substring(0, typeFullName.LastIndexOf(']'));
                }

                _typeHelper = new TypeHelper(_assemblyLocator, typeParser, pluginAssemblyTypeUsageValidatorMock.Object);

                elementMock = CreateMockElement();
                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);
        }

        [Test]
        public void InvalidTypeRefValueFailureTests()
        {
            string typeFullName = typeof(IInterface1).FullName;
         
            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service, 
                new AttributeInfo(ConfigurationFileAttributeNames.TypeRef, TypeDef1),
                null, null);

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    new AttributeInfo(ConfigurationFileAttributeNames.TypeRef, "invalidTypeDef"), null, null);

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);
        }

        [Test]
        public void TypeRefAndTypeAttributesUsedTogetherFailureTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                new AttributeInfo(ConfigurationFileAttributeNames.TypeRef, TypeDef1),
                null, null);

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    new AttributeInfo(ConfigurationFileAttributeNames.TypeRef, TypeDef1),
                    new AttributeInfo(ConfigurationFileAttributeNames.Type, typeFullName), null);

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);
        }

        [Test]
        public void TypeRefAndAssemblyAttributesUsedTogetherFailureTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                new AttributeInfo(ConfigurationFileAttributeNames.TypeRef, TypeDef1),
                null, null);

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    new AttributeInfo(ConfigurationFileAttributeNames.TypeRef, TypeDef1),
                    null,
                    new AttributeInfo(ConfigurationFileAttributeNames.Assembly, SharedTestServicesAssemblyAlias));

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);
        }
        
        [Test]
        public void AssemblyWithoutTypeFailureTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, typeFullName),
                new AttributeInfo(ConfigurationFileAttributeNames.Assembly, SharedTestServicesAssemblyAlias));

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    null,
                    null,
                    new AttributeInfo(ConfigurationFileAttributeNames.Assembly, SharedTestServicesAssemblyAlias));

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);
            }, typeof(IServiceElement), null);
        }


        [Test]
        public void BothTypeAndTypeRefMissingFailureTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                new AttributeInfo(ConfigurationFileAttributeNames.TypeRef, TypeDef1),
                null, null);

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    null, null , null);

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);

            }, typeof(IServiceElement), null);
        }

        [Test]
        public void InvalidTypeSpecifiedFailureTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, typeof(IInterface1).FullName),
                null);

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    null,
                    new AttributeInfo(ConfigurationFileAttributeNames.Type, $"{typeof(IInterface1).FullName}Invalid"),
                    null);

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);

            }, typeof(IServiceElement), null);
        }

        [Test]
        public void NonExistentAssemblySpecifiedFailureTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, typeof(IInterface1).FullName),
                new AttributeInfo(ConfigurationFileAttributeNames.Assembly, SharedTestServicesAssemblyAlias));

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    null,
                    new AttributeInfo(ConfigurationFileAttributeNames.Type, typeof(IInterface1).FullName),
                    new AttributeInfo(ConfigurationFileAttributeNames.Assembly, "invalid"));

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);

            }, typeof(IServiceElement), null);
        }

        [Test]
        public void InvalidAssemblySpecifiedFailureTests()
        {
            string typeFullName = typeof(IInterface1).FullName;

            var elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, typeof(IInterface1).FullName),
                new AttributeInfo(ConfigurationFileAttributeNames.Assembly, SharedTestServicesAssemblyAlias));

            _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            TestExpectedConfigurationParseException(() =>
            {
                elementMock = CreateTypedElementMockWithParent<IServiceElement, IServices>(ConfigurationFileElementNames.Service,
                    null,
                    new AttributeInfo(ConfigurationFileAttributeNames.Type, typeof(IInterface1).FullName),
                    new AttributeInfo(ConfigurationFileAttributeNames.Assembly, DynamicallyLoadedAssembly1Alias));

                _typeHelper.GetTypeInfo(elementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.TypeRef);

            }, typeof(IServiceElement), null);
        }

        [Test]
        public void GenericTypeWithAssemblySpecifiedTest1()
        {
            var typeDefinitionElementMock = CreateTypedElementMockWithParent<ITypeDefinitionsElement, ITypeDefinitionsElement>(ConfigurationFileElementNames.TypeDefinition,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, "SharedServices.Implementations.Generic.Generic1_1[TestPluginAssembly1.Implementations.Interface1_Impl1]"),
                new AttributeInfo(ConfigurationFileAttributeNames.Assembly, "shared_services"));

            var typeInfo = _typeHelper.GetTypeInfo(typeDefinitionElementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            Helpers.ValidateTypeInfo(typeInfo, Type.GetType("SharedServices.Implementations.Generic.Generic1_1`1[[TestPluginAssembly1.Implementations.Interface1_Impl1, TestProjects.TestPluginAssembly1]], TestProjects.SharedServices"),
                "SharedServices.Implementations.Generic.Generic1_1<TestPluginAssembly1.Implementations.Interface1_Impl1>");
        }

        [Test]
        public void GenericTypeWithAssemblySpecifiedTest2()
        {
            var typeDefinitionElementMock = CreateTypedElementMockWithParent<ITypeDefinitionsElement, ITypeDefinitionsElement>(ConfigurationFileElementNames.TypeDefinition,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, "SharedServices.Implementations.Generic.Generic1_1[SharedServices.Implementations.Interface1_Impl1]"),
                new AttributeInfo(ConfigurationFileAttributeNames.Assembly, "shared_services"));

            var typeInfo = _typeHelper.GetTypeInfo(typeDefinitionElementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            Helpers.ValidateTypeInfo(typeInfo, Type.GetType("SharedServices.Implementations.Generic.Generic1_1`1[[SharedServices.Implementations.Interface1_Impl1, TestProjects.SharedServices]], TestProjects.SharedServices"),
                "SharedServices.Implementations.Generic.Generic1_1<SharedServices.Implementations.Interface1_Impl1>");
        }

        [Test]
        public void GenericLocalType()
        {
            var serviceElementMock = CreateTypedElementMockWithParent<IServices, IServiceElement>(ConfigurationFileElementNames.Service,
                null,
                new AttributeInfo(ConfigurationFileAttributeNames.Type, "IoC.Configuration.Tests.GenericTypesAndTypeReUse.TypeHelperTests.LocalClass1[IoC.Configuration.Tests.GenericTypesAndTypeReUse.TypeHelperTests.LocalClass2]"),
                null);

            var typeInfo = _typeHelper.GetTypeInfo(serviceElementMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly,
                ConfigurationFileAttributeNames.TypeRef);

            Helpers.ValidateTypeInfo(typeInfo, typeof(LocalClass1<LocalClass2>),
                "IoC.Configuration.Tests.GenericTypesAndTypeReUse.TypeHelperTests.LocalClass1<IoC.Configuration.Tests.GenericTypesAndTypeReUse.TypeHelperTests.LocalClass2>");
        }

        private Mock<TElement> CreateTypedElementMockWithParent<TElement, TParentElement>(string elementName,
                                                                                AttributeInfo typeRefAttributeInfo,
                                                                                AttributeInfo typeAttributeInfo,
                                                                                AttributeInfo assemblyAttributeInfo)
            where TElement : class, IConfigurationFileElement
            where TParentElement : class, IConfigurationFileElement
        {
            var parentElementMock = _configurationMockHelper.CreateElementMock<TParentElement>($"{elementName}Parent");
            return _configurationMockHelper.CreateTypedElementMock<TElement, TParentElement>(elementName, parentElementMock,
                typeRefAttributeInfo, typeAttributeInfo, assemblyAttributeInfo);
        }

        private void TestExpectedConfigurationParseException([NotNull] Action actionResultingInException,
                                                             [CanBeNull] Type expectedConfigurationFileElementTypeAtError,
                                                             [CanBeNull] Type expectedParentConfigurationFileElementTypeAtError = null)
        {
            Helpers.TestExpectedConfigurationParseException<ConfigurationParseException>(actionResultingInException, 
                expectedConfigurationFileElementTypeAtError, expectedParentConfigurationFileElementTypeAtError);
        }

        public class LocalClass1<T>
        {
           
        }

        public class LocalClass2
        {

        }



    }
}
