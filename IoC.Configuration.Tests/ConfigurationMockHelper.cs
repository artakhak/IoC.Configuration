using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;
using Moq;
using OROptimizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests
{
    public class ConfigurationMockHelper : IDisposable
    {
        private static readonly string TestDllsFolder = Helpers.GetTestDllsFolderPath();

        private TypeHelper _typeHelper;
        private AssemblyResolver _assemblyResolver;
        private IAssemblyLocator _assemblyLocator;
        private Mock<IConfiguration> _configurationMock;
        private Dictionary<object, List<Func<IConfigurationFileElement>>> _parentMockToChildrenFunctionsMap = new Dictionary<object, List<Func<IConfigurationFileElement>>>();

        private Dictionary<string, AssemblyElementData> _assemblyAliasToAssemblyData = new Dictionary<string, AssemblyElementData>(StringComparer.Ordinal);

        private Dictionary<string, PluginData> _pluginNameToPluginData = new Dictionary<string, PluginData>(StringComparer.Ordinal);

        public ConfigurationMockHelper(
        [NotNull, ItemNotNull] IEnumerable<string> additionalProbingPathsRelativeToTestDllsFolder,
        [NotNull, ItemNotNull] IEnumerable<string> pluginNames,
        [NotNull, ItemNotNull] IEnumerable<AssemblyInfo> assemblyElementInfos)
        {
            HashSet<string> allProbingPaths = new HashSet<string>();

            foreach (var relativeProbingPath in additionalProbingPathsRelativeToTestDllsFolder)
            {
                var absoluteProbingPath = $"{Path.Combine(TestDllsFolder, relativeProbingPath)}";
                if (!allProbingPaths.Contains(absoluteProbingPath))
                    allProbingPaths.Add(absoluteProbingPath);
            }

            _configurationMock = CreateElementMock<IConfiguration>("iocConfiguration");

            _assemblyLocator = new AssemblyLocator(() => _configurationMock.Object, Helpers.TestsEntryAssemblyFolder);
            _typeHelper = new TypeHelper(_assemblyLocator, new TypeParser(),
                new PluginAssemblyTypeUsageValidator());

            ConfigureAdditionalProbingPaths(allProbingPaths);
            ConfigurePlugins(pluginNames, allProbingPaths);
            ConfigureAssemblies(assemblyElementInfos);

            _assemblyResolver = new AssemblyResolver(allProbingPaths.ToArray());
        }

        public void Dispose()
        {
            _assemblyResolver.Dispose();
        }

        private void ConfigureAdditionalProbingPaths([NotNull, ItemNotNull] IEnumerable<string> additionalProbingPaths)
        {
            var additionalAssemblyProbingPathsMock = CreateElementMock<IAdditionalAssemblyProbingPaths>(ConfigurationFileElementNames.AdditionalAssemblyProbingPaths);
            additionalAssemblyProbingPathsMock.SetupGet(x => x.Parent).Returns(() => _configurationMock.Object);
            _configurationMock.SetupGet(x => x.AdditionalAssemblyProbingPaths).Returns(() => additionalAssemblyProbingPathsMock.Object);

            List<IProbingPath> additionalProbingPathsList = new List<IProbingPath>();

            void AddProbingPath(string path)
            {
                var probingPathMock = CreateElementMock<IProbingPath>(ConfigurationFileElementNames.ProbingPath);
                probingPathMock.SetupGet(x => x.Parent).Returns(additionalAssemblyProbingPathsMock.Object);
                probingPathMock.SetupGet(x => x.Path).Returns(path);

                additionalProbingPathsList.Add(probingPathMock.Object);
            }

            foreach (var probingPath in additionalProbingPaths)
                AddProbingPath(probingPath);

            additionalAssemblyProbingPathsMock.SetupGet(x => x.ProbingPaths).Returns(additionalProbingPathsList);
        }
        private void ConfigurePlugins([NotNull, ItemNotNull] IEnumerable<string> pluginNames,
                                      HashSet<string> allProbingPaths)
        {
            List<IPluginElement> allPlugins = new List<IPluginElement>();
            List<IPluginSetup> allPluginSetups = new List<IPluginSetup>();

            var pluginsMock = CreateElementMock<IPlugins>(ConfigurationFileElementNames.Plugins);
            pluginsMock.SetupGet(x => x.Parent).Returns(() => _configurationMock.Object);
            _configurationMock.SetupGet(x => x.Plugins).Returns(() => pluginsMock.Object);

            pluginsMock.SetupGet(x => x.PluginsDirectory).Returns($"{Path.Combine(TestDllsFolder, "PluginDlls")}");
            pluginsMock.SetupGet(x => x.AllPlugins).Returns(allPlugins);

            var pluginsSetupMock = CreateElementMock<IPluginsSetup>(ConfigurationFileElementNames.PluginsSetup);
            pluginsSetupMock.SetupGet(x => x.Parent).Returns(() => _configurationMock.Object);
            _configurationMock.SetupGet(x => x.PluginsSetup).Returns(() => pluginsSetupMock.Object);
            pluginsSetupMock.SetupGet(x => x.AllPluginSetups).Returns(() => allPluginSetups);

            if (pluginNames.Any())
            {
                foreach (var pluginName in pluginNames)
                {
                    var pluginDirectory = $"{Path.Combine(TestDllsFolder, "PluginDlls", pluginName)}";

                    if (!allProbingPaths.Contains(pluginDirectory))
                        allProbingPaths.Add(pluginDirectory);

                    var pluginMock = CreateElementMock<IPluginElement>(ConfigurationFileElementNames.Plugin);
                    allPlugins.Add(pluginMock.Object);

                    pluginMock.SetupGet(x => x.Parent).Returns(pluginsMock.Object);
                    pluginMock.SetupGet(x => x.Name).Returns(pluginName);
                    pluginMock.Setup(x => x.GetPluginDirectory()).Returns(pluginDirectory);
                    pluginMock.SetupGet(x => x.Enabled).Returns(() => _pluginNameToPluginData[pluginName].Enabled);

                    pluginsMock.Setup(x => x.GetPlugin(pluginName)).Returns(pluginMock.Object);

                    var pluginSetupMock = CreateElementMock<IPluginSetup>(ConfigurationFileElementNames.PluginSetup);
                    allPluginSetups.Add(pluginSetupMock.Object);

                    pluginSetupMock.SetupGet(x => x.Parent).Returns(pluginsSetupMock.Object);
                    pluginSetupMock.SetupGet(x => x.Plugin).Returns(pluginMock.Object);
                    pluginSetupMock.SetupGet(x => x.OwningPluginElement).Returns(pluginMock.Object);

                    pluginMock.SetupGet(x => x.Enabled).Returns(() => _pluginNameToPluginData[pluginName].Enabled);

                    _pluginNameToPluginData[pluginName] = new PluginData(pluginName, pluginMock, pluginSetupMock);
                }
            }
        }

        public PluginData GetPluginData(string pluginName) => _pluginNameToPluginData.TryGetValue(pluginName, out var pluginData) ? pluginData : null;
        public AssemblyElementData GetAssemblyElementData(string assemblyAlias) => _assemblyAliasToAssemblyData.TryGetValue(assemblyAlias, out var assemblyElementData) ? assemblyElementData : null;
        public Mock<IConfiguration> ConfigurationMock => _configurationMock;
        private void ConfigureAssemblies([NotNull, ItemNotNull] IEnumerable<AssemblyInfo> assemblyElementInfos)
        {
            ConfigurationFile.IAssembly AddAssembly(AssemblyInfo assemblyInfo)
            {
                var assemblyPath = Path.Combine(assemblyInfo.AssemblyFolder, $"{assemblyInfo.AssemblyName}.dll");

                System.Reflection.Assembly assembly =
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => assemblyInfo.AssemblyName.Equals(x.GetName().Name, StringComparison.OrdinalIgnoreCase));

                if (assembly == null)
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

                var assemblyMock = CreateElementMock<ConfigurationFile.IAssembly>(ConfigurationFileElementNames.Assembly);
                assemblyMock.SetupGet(x => x.Alias).Returns(assemblyInfo.AssemblyName);
                assemblyMock.SetupGet(x => x.Name).Returns(assemblyInfo.AssemblyName);
                assemblyMock.SetupGet(x => x.AbsolutePath).Returns(assemblyPath);
                assemblyMock.Setup(x => x.ToString()).Returns($"[alias={assemblyInfo.AssemblyAlias}, name={assemblyInfo.AssemblyName}]");

                _assemblyAliasToAssemblyData[assemblyInfo.AssemblyAlias] = new AssemblyElementData(assemblyMock.Object);

                var dirPathFolders = assemblyInfo.AssemblyFolder.Split(Path.DirectorySeparatorChar).ToList();

                var indexOfPluginDlls = dirPathFolders.IndexOf("PluginDlls");

                if (indexOfPluginDlls >= 0)
                {
                    var pluginName = dirPathFolders[indexOfPluginDlls + 1];
                    var pluginData = _pluginNameToPluginData[pluginName];

                    assemblyMock.SetupGet(x => x.Plugin).Returns(pluginData.PluginMock.Object);
                    assemblyMock.SetupGet(x => x.OwningPluginElement).Returns(pluginData.PluginMock.Object);

                    assemblyMock.SetupGet(x => x.Enabled).Returns(pluginData.PluginMock.Object.Enabled);
                }

                return assemblyMock.Object;
            }


            foreach (var assemblyElementInfo in assemblyElementInfos)
            {
                AddAssembly(assemblyElementInfo);
            }

            var assembliesMock = CreateElementMock<IAssemblies>(ConfigurationFileElementNames.Assemblies);
            assembliesMock.Setup(x => x.Parent).Returns(_configurationMock.Object);
            _configurationMock.Setup(x => x.Assemblies).Returns(assembliesMock.Object);


            assembliesMock.Setup(x => x.AllAssemblies).Returns(
                () => _assemblyAliasToAssemblyData.Values.Where(x => !x.IsExcluded).Select(x => x.Assembly));

            foreach (var assemblyInfo in assemblyElementInfos)
            {
                var assemblyData = _assemblyAliasToAssemblyData[assemblyInfo.AssemblyAlias];
                assembliesMock.Setup(x => x.GetAssemblyByAlias(assemblyInfo.AssemblyAlias)).Returns(() =>
                    assemblyData.IsExcluded ? null : assemblyData.Assembly);
            }

            

            IAssembly CreateAssembly(Type typeInAssembly)
            {
                var assembly = typeInAssembly.Assembly;
                return new Assembly(assembly.Location, assembly.GetName().Name, null);
            }

            assembliesMock.SetupGet(x => x.MsCorlibAssembly).Returns(CreateAssembly(typeof(int)));
            assembliesMock.SetupGet(x => x.IoCConfigurationAssembly).Returns(CreateAssembly(typeof(IoC.Configuration.IAssembly)));
            assembliesMock.SetupGet(x => x.OROptimizerSharedAssembly).Returns(CreateAssembly(typeof(OROptimizer.IGlobalsCore)));

            IEnumerable<IAssembly> getAllAssembliesIncludingAssembliesNotInConfiguration()
            {
                foreach (var assembly in assembliesMock.Object.AllAssemblies)
                    yield return assembly;
                         
                yield return assembliesMock.Object.MsCorlibAssembly;
                yield return assembliesMock.Object.IoCConfigurationAssembly;
                yield return assembliesMock.Object.OROptimizerSharedAssembly;
            }

            assembliesMock.SetupGet(x => x.AllAssembliesIncludingAssembliesNotInConfiguration).Returns(getAllAssembliesIncludingAssembliesNotInConfiguration);
        }

        /// <summary>
        /// Configures the type definitions.
        /// </summary>
        /// <param name="TypeDefinitionInfos">The type definition infos.</param>
        public void ConfigureTypeDefinitions([NotNull, ItemNotNull] IEnumerable<TypeDefinitionInfo> typeDefinitionInfos)
        {
            var typeDefinitionsMock = CreateElementMock<ITypeDefinitionsElement>(ConfigurationFileElementNames.TypeDefinitions);
            typeDefinitionsMock.SetupGet(x => x.Parent).Returns(_configurationMock.Object);
            _configurationMock.SetupGet(x => x.TypeDefinitions).Returns(typeDefinitionsMock.Object);

            Dictionary<string, INamedTypeDefinitionElement> aliasToTypeDefinitionElement = new Dictionary<string, INamedTypeDefinitionElement>(StringComparer.InvariantCultureIgnoreCase);

            void AddTypeDefinition(TypeDefinitionInfo typeDefinitionInfo)
            {
                var typeDefinitionMock = CreateTypedElementMock<INamedTypeDefinitionElement, ITypeDefinitionsElement>(
                    ConfigurationFileElementNames.TypeDefinition,
                    typeDefinitionsMock, null,
                    new AttributeInfo(ConfigurationFileAttributeNames.Type, typeDefinitionInfo.TypeFullNameInConfigFile), null);

                var typeInfo = _typeHelper.GetTypeInfo(typeDefinitionMock.Object, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly, ConfigurationFileAttributeNames.TypeRef);
                typeDefinitionMock.SetupGet(x => x.ValueTypeInfo).Returns(typeInfo);

                aliasToTypeDefinitionElement[typeDefinitionInfo.Alias] = typeDefinitionMock.Object;
                typeDefinitionsMock.Setup(x => x.GetTypeDefinition(typeDefinitionInfo.Alias)).Returns(typeDefinitionMock.Object);
            }

            foreach (var typeDefinitionInfo in typeDefinitionInfos)
                AddTypeDefinition(typeDefinitionInfo);

            typeDefinitionsMock.SetupGet(x => x.AllTypeDefinitions).Returns(aliasToTypeDefinitionElement.Values.ToList());
        }

        private void SetupParentChildRelationships<TElement, TParentElement>(Mock<TElement> elementMock, Mock<TParentElement> parentElementMock)
            where TElement : class, IConfigurationFileElement
            where TParentElement : class, IConfigurationFileElement
        {
            elementMock.SetupGet(x => x.Parent).Returns(() => parentElementMock.Object);
            _parentMockToChildrenFunctionsMap[parentElementMock].Add(() => elementMock.Object);
        }
        
        public Mock<TElement> CreateTypedElementMock<TElement, TParentElement>(string elementName,
                                                                              Mock<TParentElement> parentElementMock,
                                                                              AttributeInfo typeRefAttributeInfo,
                                                                              AttributeInfo typeAttributeInfo,
                                                                              AttributeInfo assemblyAttributeInfo)
         where TElement : class, IConfigurationFileElement
         where TParentElement : class, IConfigurationFileElement
        {
            var elementMock = CreateElementMock<TElement, TParentElement>(elementName, parentElementMock);
            elementMock.SetupGet(x => x.ElementName).Returns(elementName);

            var elementToStringBldr = new StringBuilder();
            elementToStringBldr.Append($"<{elementName} ");

            if (typeAttributeInfo != null)
            {
                elementToStringBldr.Append($"{typeAttributeInfo.Name}=\"{typeAttributeInfo.Value}\" ");

                elementMock.Setup(x => x.HasAttribute(typeAttributeInfo.Name)).Returns(true);
                elementMock.Setup(x => x.GetAttributeValue(typeAttributeInfo.Name)).Returns(typeAttributeInfo.Value);
            }

            if (typeRefAttributeInfo != null)
            {
                elementToStringBldr.Append($"{typeRefAttributeInfo.Name}=\"{typeRefAttributeInfo.Value}\" ");

                elementMock.Setup(x => x.HasAttribute(typeRefAttributeInfo.Name)).Returns(true);
                elementMock.Setup(x => x.GetAttributeValue(typeRefAttributeInfo.Name)).Returns(typeRefAttributeInfo.Value);
            }

            if (assemblyAttributeInfo != null)
            {
                elementToStringBldr.Append($"{assemblyAttributeInfo.Name}=\"{assemblyAttributeInfo.Value}\" ");
                elementMock.Setup(x => x.HasAttribute(assemblyAttributeInfo.Name)).Returns(true);
                elementMock.Setup(x => x.GetAttributeValue(assemblyAttributeInfo.Name)).Returns(assemblyAttributeInfo.Value);
            }

            elementToStringBldr.Append(">");
           
            elementMock.Setup(x => x.XmlElementToString()).Returns(elementToStringBldr.ToString());

            return elementMock;
        }

        public Mock<TElement> CreateElementMock<TElement, TParentElement>(string elementName, Mock<TParentElement> parentElementMock)
            where TElement : class, IConfigurationFileElement
            where TParentElement : class, IConfigurationFileElement
        {
            var elementMock = CreateElementMock<TElement>(elementName);

            SetupParentChildRelationships(elementMock, parentElementMock);
            return elementMock;
        }

        public Mock<TElement> CreateElementMock<TElement>(string elementName)
            where TElement : class, IConfigurationFileElement
        {
            var elementToString = $"<{elementName}>";

            var elementMock = new Mock<TElement>();
            elementMock.Setup(x => x.ElementName).Returns(elementName);
            elementMock.Setup(x => x.XmlElementToString()).Returns(elementToString);
            elementMock.Setup(x => x.ToString()).Returns(() => elementMock.Object.XmlElementToString());

            IConfiguration configuration = null;

            if (_configurationMock == null)
            {
                if (typeof(TElement) == typeof(IConfiguration))
                    configuration = (IConfiguration)elementMock.Object;
                else
                    Assert.Fail();
            }
            else
            {
                configuration = _configurationMock.Object;
            }

            elementMock.SetupGet(x => x.Configuration).Returns(configuration);

            elementMock.Setup(x => x.GetPluginSetupElement()).Returns(() =>
            {
                if (elementMock.Object is IPluginSetup pluginSetup)
                    return pluginSetup;

                if (elementMock is IConfiguration || elementMock.Object.Parent == null)
                    return null;

                return elementMock.Object.Parent.GetPluginSetupElement();
            });

            elementMock.Setup(x => x.OwningPluginElement).Returns(() =>
            {
                if (elementMock is ConfigurationFile.IAssembly assembly)
                    return assembly.Plugin;
                
                return elementMock.Object.GetPluginSetupElement()?.OwningPluginElement;
            });


            var getChildrenFunctions = new List<Func<IConfigurationFileElement>>();
            _parentMockToChildrenFunctionsMap[elementMock] = getChildrenFunctions;

            elementMock.SetupGet(x => x.Children).Returns(() =>
                getChildrenFunctions.Select(x => x.Invoke()).ToList()
            );

            return elementMock;
        }

        

        public class TypeDefinitionInfo
        {
            public string Alias { get; }
            public string TypeFullNameInConfigFile { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="TypeDefinitionInfo"/> class.
            /// </summary>
            /// <param name="alias">The alias.</param>
            /// <param name="typeFullNameInConfigFile">
            /// The type full names used in configuration file. Examples are:
            /// System.Collections.Generic.IEnumerable[SharedServices.Interfaces.IInterface1]
            /// SharedServices.Interfaces.IInterface1.
            /// </param>
            public TypeDefinitionInfo(string alias, string typeFullNameInConfigFile)
            {
                Alias = alias;
                TypeFullNameInConfigFile = typeFullNameInConfigFile;
            }
        }

        public class AssemblyInfo
        {
            public AssemblyInfo(string assemblyAlias, string assemblyName, string assemblyFolder)
            {
                AssemblyAlias = assemblyAlias;
                AssemblyName = assemblyName;
                AssemblyFolder = assemblyFolder;
            }

            public string AssemblyAlias { get; }
            public string AssemblyName { get; }
            public string AssemblyFolder { get; }
        }

        public class PluginData
        {
            public PluginData(string pluginName, Mock<IPluginElement> pluginMock, Mock<IPluginSetup> pluginSetupMock)
            {
                PluginName = pluginName;
                PluginMock = pluginMock;
                PluginSetupMock = pluginSetupMock;
            }

            public string PluginName { get; }
            public Mock<IPluginElement> PluginMock { get; }
            public Mock<IPluginSetup> PluginSetupMock { get; }

            public bool Enabled { get; set; } = true;
        }

        public class AssemblyElementData
        {
            public AssemblyElementData(ConfigurationFile.IAssembly assembly)
            {
                Assembly = assembly;
            }

            public ConfigurationFile.IAssembly Assembly { get; }

            public bool IsExcluded { get; set; } = false;
        }
    }
}
