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
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DependencyInjection;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainer.BindingsForCode;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using IoC.Configuration.DynamicCode;
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.Emit;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.DynamicCode;
using OROptimizer.Serializer;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public class FileBasedConfiguration : DiContainerBuilderConfiguration
    {
        #region Member Variables

        // TODO: AH: 1/15/2019: _addAssemblyReferenciesToDynamicAssemblyInOneMethodOnly variable is added temporarily, until the method  
        // AddAssemblyReferencesFromAssembliesElementToDynamicAssembly() method is properly tested
        // After, enough time passes, and there are no regressions, the variable will be removed, and all the code
        // that this variable blocks will be removed.
        private readonly bool _addAssemblyReferenciesToDynamicAssemblyInOneMethodOnly = true;

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [NotNull]
        private readonly IConfigurationFileContentsProvider _configurationFileContentsProvider;

        [NotNull]
        private readonly IConfigurationFileElementFactory _configurationFileElementFactory;

        [CanBeNull]
        private readonly ConfigurationFileXmlDocumentLoadedEventHandler _configurationFileXmlDocumentLoaded;

        private static readonly Dictionary<string, HashSet<string>> _directoriesToCleanup = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
 
        private readonly string _dynamicallyGeneratedAssemblyFileName;

        private readonly string _dynamicImplementationsNamespace;

        private System.Reflection.Assembly _loadedDynamicAssembly;

        private readonly object _lockObject = new object();

        [CanBeNull]
        private IOnApplicationsStarted _onApplicationsStarted;

        #endregion

        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileBasedConfiguration" /> class.
        /// </summary>
        /// <param name="configurationFileContentsProvider">
        ///     The configuration file contents provider. An example implementation of
        ///     <see cref="IConfigurationFileContentsProvider" /> implementation is
        ///     <see cref="FileBasedConfigurationFileContentsProvider" />
        /// </param>
        /// <param name="entryAssemblyFolder">
        ///     The location where the executable is.
        ///     For non test projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> can be used as a value for this parameter.
        ///     However, for tests projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> might be
        ///     be the folder where the test execution library is, so a different value might need to be passed.
        /// </param>
        /// <param name="configurationFileXmlDocumentLoaded">
        ///     The configuration file XML document loaded.
        /// </param>
        public FileBasedConfiguration([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider,
                                      [NotNull] string entryAssemblyFolder,
                                      [CanBeNull] ConfigurationFileXmlDocumentLoadedEventHandler configurationFileXmlDocumentLoaded) : base(entryAssemblyFolder)
        {
            var assemblyLocator = IoCServiceFactoryAmbientContext.Context.CreateAssemblyLocator(() => Configuration, entryAssemblyFolder);

            _assemblyLocator = assemblyLocator;
            _configurationFileElementFactory = new ConfigurationFileElementFactory(assemblyLocator);

            _configurationFileContentsProvider = configurationFileContentsProvider;
            _configurationFileXmlDocumentLoaded = configurationFileXmlDocumentLoaded;

            _dynamicImplementationsNamespace = $"DynamicImplementations_{GlobalsCoreAmbientContext.Context.GenerateUniqueId()}";
            _dynamicallyGeneratedAssemblyFileName = $"DynamicallyGeneratedAssembly_{GlobalsCoreAmbientContext.Context.GenerateUniqueId()}.dll";
        }

        #endregion

        #region Member Functions

        private void AddAssemblyReferencesFromAssembliesElementToDynamicAssembly(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            // Instead of hunting for all assemblies across the configuration file, for now lets just add 
            // the enabled assemblies

            foreach (var assembly in Configuration.Assemblies.AllAssembliesIncludingAssembliesNotInConfiguration)
            {
                if (assembly.Plugin != null && !assembly.Plugin.Enabled)
                    continue;

                if (assembly is ConfigurationFile.IAssembly assemblyElement && !assemblyElement.Enabled)
                    continue;

                dynamicAssemblyBuilder.AddReferencedAssembly(assembly.AbsolutePath);
            }
        }

        private void AddAutoGeneratedServices([NotNull] IDependencyInjection dependencyInjection,
                                              [NotNull] IList<DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo> interfaceImplementationsInfo,
                                              [NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            foreach (var autoGeneratedServiceElement in dependencyInjection.AutoGeneratedServices.Services)
            {
                autoGeneratedServiceElement.GenerateAutoImplementedServiceClassCSharp(dynamicAssemblyBuilder, _dynamicImplementationsNamespace, out var generateClassFullName);
                interfaceImplementationsInfo.Add(new DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo(autoGeneratedServiceElement.ImplementedInterfaceTypeInfo.Type,
                    generateClassFullName));
            }
        }

        private void AddFileConfigurationElements(XmlNode xmlNode, IConfigurationFileElement parentElement)
        {
            foreach (var childXmlNode in xmlNode.ChildNodes)
            {
                var childXmlElement = childXmlNode as XmlElement;

                if (childXmlElement == null)
                    continue;

                LogHelper.Context.Log.InfoFormat("Processing element '{0}'", childXmlElement.XmlElementToString());

                var childElement = _configurationFileElementFactory.CreateConfigurationFileElement(childXmlElement, parentElement);

                AddFileConfigurationElements(childXmlElement, childElement);

                LogHelper.Context.Log.DebugFormat("Adding element '{0}' to parent '{1}'.", childElement.ElementName, parentElement.ElementName);
                parentElement.AddChild(childElement);
                LogHelper.Context.Log.DebugFormat("Finished processing element '{0}'", childXmlElement.Name);
            }

            LogHelper.Context.Log.DebugFormat("Validating element '{0}' after children added.", parentElement.ElementName);
            parentElement.ValidateAfterChildrenAdded();
            LogHelper.Context.Log.DebugFormat("Finished validating element '{0}' after children added.", parentElement.ElementName);
        }

        private void AddPluginBindings([NotNull] IList<BindingConfigurationForFile> servicesToAddBindingsTo, [NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
#pragma warning disable CS0612, CS0618
                                       [NotNull] ITypesListFactoryTypeGenerator typesListFactoryTypeGenerator,
#pragma warning restore CS0612, CS0618
                                       IList<DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo> interfaceImplementationsInfo,
                                       IList<ModuleInfo> allGeneratedModuleInfos)
        {
            var serviceImplementationElements = new List<IServiceImplementationElement>();

            foreach (var pluginSetup in Configuration.PluginsSetup.AllPluginSetups)
            {
                if (!pluginSetup.Enabled)
                    continue;

                serviceImplementationElements.Add(pluginSetup.PluginImplementationElement);

                if (!_addAssemblyReferenciesToDynamicAssemblyInOneMethodOnly)
                    dynamicAssemblyBuilder.AddReferencedAssembly(pluginSetup.PluginImplementationElement.ValueTypeInfo.Type);

                if (pluginSetup.DependencyInjection != null)
                    ProcessDependencyInjectionSection(dynamicAssemblyBuilder, typesListFactoryTypeGenerator, pluginSetup.DependencyInjection,
                        $"{_dynamicImplementationsNamespace}.{pluginSetup.Plugin.Name}", "PluginServicesModule",
                        interfaceImplementationsInfo, allGeneratedModuleInfos);
            }

            if (serviceImplementationElements.Count > 0)
                servicesToAddBindingsTo.Add(BindingConfigurationForFile.CreateBindingConfigurationForFile(typeof(IPlugin), false, serviceImplementationElements));
        }

        private void AddReferencedAssemblies([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
                                             [NotNull] [ItemNotNull] IEnumerable<BindingConfigurationForFile> servicesBindingConfigurations)
        {
            if (_addAssemblyReferenciesToDynamicAssemblyInOneMethodOnly)
                return;

            foreach (var servicesBindingConfiguration in servicesBindingConfigurations)
            {
                dynamicAssemblyBuilder.AddReferencedAssembly(servicesBindingConfiguration.ServiceType);

                AddReferencedAssemblies(dynamicAssemblyBuilder, servicesBindingConfiguration.Implementations);
            }
        }

        private void AddReferencedAssemblies([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
                                             [NotNull] [ItemNotNull] IEnumerable<BindingImplementationConfigurationForFile> bindingImplementationConfigurations)
        {
            foreach (var bindingImplementationConfiguration in bindingImplementationConfigurations)
            {
                dynamicAssemblyBuilder.AddReferencedAssembly(bindingImplementationConfiguration.ImplementationType);

                if (bindingImplementationConfiguration is TypeBasedBindingImplementationConfigurationForFile typeBasedBindingImplementationConfigurationForFile)
                {
                    if (typeBasedBindingImplementationConfigurationForFile.Parameters != null)
                        foreach (var parameter in typeBasedBindingImplementationConfigurationForFile.Parameters)
                            dynamicAssemblyBuilder.AddReferencedAssembly(parameter.ValueTypeInfo.Type);

                    if (typeBasedBindingImplementationConfigurationForFile.InjectedProperties != null)
                        foreach (var injectedProperty in typeBasedBindingImplementationConfigurationForFile.InjectedProperties)
                            dynamicAssemblyBuilder.AddReferencedAssembly(injectedProperty.ValueTypeInfo.Type);
                }
            }
        }

        private void AddSettingsRequestorBindings([NotNull] IList<BindingConfigurationForFile> servicesToAddBindingsTo, [NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            if (Configuration.SettingsRequestor == null || !Configuration.SettingsRequestor.Enabled)
                return;

            var serviceImplementationElements = new List<IServiceImplementationElement> {Configuration.SettingsRequestor};

            if (!_addAssemblyReferenciesToDynamicAssemblyInOneMethodOnly)
                dynamicAssemblyBuilder.AddReferencedAssembly(Configuration.SettingsRequestor.ValueTypeInfo.Type);

            servicesToAddBindingsTo.Add(BindingConfigurationForFile.CreateBindingConfigurationForFile(typeof(ISettingsRequestor), false, serviceImplementationElements));
        }

        private void AddStartupActionBindings([NotNull] IList<BindingConfigurationForFile> servicesToAddBindingsTo, [NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            if (Configuration.StartupActions == null)
                return;

            var serviceImplementationElements = new List<IServiceImplementationElement>();

            foreach (var startupAction in Configuration.StartupActions.StartupActions)
            {
                serviceImplementationElements.Add(startupAction);
                dynamicAssemblyBuilder.AddReferencedAssembly(startupAction.ValueTypeInfo.Type);
            }

            if (serviceImplementationElements.Count > 0)
                servicesToAddBindingsTo.Add(BindingConfigurationForFile.CreateBindingConfigurationForFile(typeof(IStartupAction), false, serviceImplementationElements));
        }

        [Obsolete("Will be removed after 5/31/2019")]
        private void AddTypeFactories([NotNull] IDependencyInjection dependencyInjection,
                                      [NotNull] IList<DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo> interfaceImplementationsInfo,
                                      [NotNull] ITypesListFactoryTypeGenerator typesListFactoryTypeGenerator,
                                      [NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            foreach (var typeFactory in dependencyInjection.AutoGeneratedServices.TypeFactories)
            {
                var selectorParameterValues = new LinkedList<string[]>();
                var selectorTypes = new LinkedList<IEnumerable<Type>>();

                foreach (var selectorTypesForIf in typeFactory.ReturnedIfTypeSelectorsForIfCase)
                {
                    selectorParameterValues.AddLast(selectorTypesForIf.ParameterValues.ToArray());
                    selectorTypes.AddLast(selectorTypesForIf.ReturnedTypes.Select(x => x.ReturnedType));
                }

                try
                {
                    var generatedTypeInfo = typesListFactoryTypeGenerator.GenerateType(dynamicAssemblyBuilder, typeFactory.ImplementedMethodInfo.DeclaringType,
                        _dynamicImplementationsNamespace,
                        typeFactory.ReturnedTypeSelectorForDefaultCase.ReturnedTypes.Select(x => x.ReturnedType),
                        selectorParameterValues, selectorTypes);

                    dynamicAssemblyBuilder.AddCSharpFile(generatedTypeInfo.CSharpFileContents);

                    interfaceImplementationsInfo.Add(new DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo(typeFactory.ImplementedMethodInfo.DeclaringType, generatedTypeInfo.TypeFullName));
                }
                catch (Exception e)
                {
                    string exceptionDetails = null;
                    if (e.Message.Length > 0)
                        exceptionDetails = $"{Environment.NewLine}Exception details: {e.Message}{Environment.NewLine}{e.StackTrace}";

                    throw new ConfigurationParseException(typeFactory, $"Failed to generate a C# class for the implementation of interface '{typeFactory.ImplementedMethodInfo.DeclaringType.FullName}'{exceptionDetails}.");
                }
            }
        }

        private static void CleanupDirectory([NotNull] string directoryPath)
        {
            try
            {
                var files = Directory.GetFiles(directoryPath);
                foreach (var filePath in files)
                {
                    var fileName = Path.GetFileName(filePath);

                    //if (file)

                    var regex = new Regex("^DynamicallyGeneratedAssembly_[0-9]*.(dll|pdb)$", RegexOptions.IgnoreCase);

                    if (regex.IsMatch(fileName))
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Context.Log.Error($"Failed to delete file '{filePath}'.", e);
                        }
                }
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error($"Failed to cleanup directory '{directoryPath}'.", e);
            }
        }

        /// <summary>
        ///     Gets the configuration loaded from the root iocConfiguration element. This property stores the entire
        ///     configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        [NotNull]
        public IConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public override void Dispose()
        {
            // base.Dispose() will dispose off DiContainer. So lets stop the startup applications, before we call  base.Dispose().
            if (_onApplicationsStarted != null)
                _onApplicationsStarted.Dispose();
            else
                LogHelper.Context.Log.ErrorFormat("{0} is not initialized.", nameof(_onApplicationsStarted));

            base.Dispose();

            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        protected override IEnumerable<object> GenerateAllNativeModules()
        {
            var additionalModules = NativeAndDiModules;

            IList<ModuleInfo> allGeneratedModuleInfos = new List<ModuleInfo>();

            LogHelper.Context.Log.InfoFormat("Generating dependency injection modules from configuration file.");

            var requiredBindingsModule = DiManager.GetRequiredBindingsModule();
            if (requiredBindingsModule != null)
                allGeneratedModuleInfos.Add(new ModuleInfo(this, _serviceRegistrationBuilder, requiredBindingsModule));

#pragma warning disable CS0612, CS0618
            var typesListFactoryTypeGenerator = IoCServiceFactoryAmbientContext.Context.CreateTypesListFactoryTypeGenerator(Configuration.ParameterSerializers.TypeBasedSimpleSerializerAggregator);
#pragma warning restore CS0612, CS0618

            IList<DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo> interfaceImplementationsInfo = new List<DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo>();

            var dynamicallyGeneratedAssemblyFilePath = GetDynamicallyGeneratedAssemblyFilePath();

            lock (_lockObject)
            {
                if (!_directoriesToCleanup.TryGetValue(Configuration.ApplicationDataDirectory.Path, out var allDynamicallyGeneratedFilesInFolder))
                {
                    allDynamicallyGeneratedFilesInFolder = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _directoriesToCleanup[Configuration.ApplicationDataDirectory.Path] = allDynamicallyGeneratedFilesInFolder;
                }

                // Note, this logic of cleaning up is not perfect, since we might try to delete DLLs created by some other application, if that 
                // application uses the same folder. This will be improved in the future
                if (allDynamicallyGeneratedFilesInFolder.Count == 0)
                    CleanupDirectory(Configuration.ApplicationDataDirectory.Path);

                allDynamicallyGeneratedFilesInFolder.Add(dynamicallyGeneratedAssemblyFilePath);
            }

            using (var dynamicAssemblyBuilder =
                GlobalsCoreAmbientContext.Context.StartDynamicAssemblyBuilder(dynamicallyGeneratedAssemblyFilePath, OnDynamicAssemblyBuildCompleted, true))
            {
                using (new DynamicHelperClassesStarter(dynamicAssemblyBuilder, Configuration))
                {
                    if (!_addAssemblyReferenciesToDynamicAssemblyInOneMethodOnly)
                        dynamicAssemblyBuilder.AddReferencedAssembly(typeof(ITypeBasedSimpleSerializerAggregator));

                    // Do not use IoC.Configuration in namespace, since it causes some naming conflicts with Autofac modules.

                    AddAssemblyReferencesFromAssembliesElementToDynamicAssembly(dynamicAssemblyBuilder);

                    //ProcessNonDependencyInjectionSections(dynamicAssemblyBuilder);

                    if (Configuration.DependencyInjection != null)
                        ProcessDependencyInjectionSection(dynamicAssemblyBuilder, typesListFactoryTypeGenerator, Configuration.DependencyInjection, _dynamicImplementationsNamespace, "ServicesModule", interfaceImplementationsInfo, allGeneratedModuleInfos);

                    var additionalServices = new List<BindingConfigurationForFile>();

                    if (!_addAssemblyReferenciesToDynamicAssemblyInOneMethodOnly)
                    {
                        // Lets add a reference to this assembly
                        dynamicAssemblyBuilder.AddReferencedAssembly(typeof(IPlugin));
                    }

                    AddSettingsRequestorBindings(additionalServices, dynamicAssemblyBuilder);
                    AddStartupActionBindings(additionalServices, dynamicAssemblyBuilder);
                    AddPluginBindings(additionalServices, dynamicAssemblyBuilder, typesListFactoryTypeGenerator, interfaceImplementationsInfo, allGeneratedModuleInfos);

                    AddReferencedAssemblies(dynamicAssemblyBuilder, additionalServices);

                    // Add additional bindings after plugin bindings are handled (such as plugin implementations)
                    var additionalServicesClassName = "AdditionalServices";

                    var additionalServicesModuleClassContent =
                        Configuration.DiManagers.ActiveDiManagerElement.DiManager.GenerateModuleClassCode(dynamicAssemblyBuilder, _assemblyLocator,
                            _dynamicImplementationsNamespace, additionalServicesClassName, additionalServices);

                    dynamicAssemblyBuilder.AddCSharpFile(additionalServicesModuleClassContent);
                    allGeneratedModuleInfos.Add(new ModuleInfo(this, $"{_dynamicImplementationsNamespace}.{additionalServicesClassName}"));
                }
            }

            allGeneratedModuleInfos.Add(new ModuleInfo(this, _serviceRegistrationBuilder, new DynamicallyGeneratedImplementationsModule(interfaceImplementationsInfo, GetDynamicallyGeneratedAssemblyFilePath())));
            allGeneratedModuleInfos.Add(new ModuleInfo(this, _serviceRegistrationBuilder, new DefaultModule(this)));

            var diModules = additionalModules.Where(x => x is IDiModule).Select(x => (IDiModule) x);
            allGeneratedModuleInfos.Add(new ModuleInfo(this, _serviceRegistrationBuilder, new ConcreteClassesRegistrationsModule(Configuration, diModules)));

            if (additionalModules != null)
                foreach (var module in additionalModules)
                    allGeneratedModuleInfos.Add(new ModuleInfo(this, _serviceRegistrationBuilder, module));

            LogHelper.Context.Log.InfoFormat("Finished generating dependency injection modules from configuration file.");
            return allGeneratedModuleInfos.Select(moduleInfo => moduleInfo.GetNativeModule());
        }

        private string GetDynamicallyGeneratedAssemblyFilePath()
        {
            if (Configuration?.ApplicationDataDirectory?.Path == null)
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"Call the method '{nameof(GetDynamicallyGeneratedAssemblyFilePath)}' only after the configuration was successfully loaded from xml file.");

            return Path.Combine(Configuration?.ApplicationDataDirectory?.Path, _dynamicallyGeneratedAssemblyFileName);
        }

        /// <summary>
        ///     This method should be called after the object is constructed.
        /// </summary>
        public override void Init()
        {
            base.Init();

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            Load(_configurationFileContentsProvider);
        }

        private void Load([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider)
        {
            LogHelper.Context.Log.InfoFormat("Loading configuration started.");
            var schemaSet = new XmlSchemaSet();

            Action<Exception, Func<string, Exception>> logAnErrorAndRethrowException = (originalException, createRethrownException) =>
            {
                LogHelper.Context.Log.Fatal(originalException);
                throw createRethrownException($"Failed to parse the xml file '{configurationFileContentsProvider.ConfigurationFileSourceDetails}'.");
            };

            try
            {
                // Load XML schema file
                var xmlSchemaPath = Path.Combine(_entryAssemblyFolder, HelpersIoC.SchemaFileFolderRelativeLocation, HelpersIoC.IoCConfigurationSchemaName);

                LogHelper.Context.Log.InfoFormat("Loading xml schema from '{0}'", xmlSchemaPath);
                var reader = new XmlTextReader(xmlSchemaPath);
                var xmlSchema = XmlSchema.Read(reader, (sender, e) =>
                {
                    if (e.Exception != null)
                        throw e.Exception;

                    if (e.Severity == XmlSeverityType.Error)
                        throw new Exception($"Error in XML Schema file: '{xmlSchemaPath}'. Schema load message: {e.Message}");
                });

                schemaSet.Add(xmlSchema);

                // Load XML file
                var xmlDocument = new XmlDocument {Schemas = schemaSet};

                LogHelper.Context.Log.InfoFormat("Loading XML configuration file from '{0}'", configurationFileContentsProvider.ConfigurationFileSourceDetails);

                using (var stringReader = new StringReader(configurationFileContentsProvider.LoadConfigurationFileContents()))
                {
                    xmlDocument.Load(stringReader);
                }

                _configurationFileXmlDocumentLoaded?.Invoke(this, new ConfigurationFileXmlDocumentLoadedEventArgs(xmlDocument));

                xmlDocument.Validate((sender, e) =>
                {
                    if (e.Exception != null)
                        throw e.Exception;

                    if (e.Severity == XmlSeverityType.Error)
                        throw new Exception($"Errors in XML file: '{configurationFileContentsProvider.ConfigurationFileSourceDetails}'.");
                });

                // Process loaded XML
                var nodes = xmlDocument.GetElementsByTagName(ConfigurationFileElementNames.RootElement);

                if (nodes.Count != 1 || !(nodes.Item(0) is XmlElement))
                    throw new Exception($"Expected '{ConfigurationFileElementNames.RootElement}' element as a root element in configuration file.");

                var rootElement = (XmlElement) nodes.Item(0);

                LogHelper.Context.Log.InfoFormat("Processing element: {0}", rootElement.XmlElementToString());

                Configuration = _configurationFileElementFactory.CreateConfiguration(rootElement);

                AddFileConfigurationElements(rootElement, Configuration);

                Configuration.ProcessTree(ProcessConfigurationFileElementAfterTreeConstructed);

                DiManager = Configuration.DiManagers.ActiveDiManagerElement.DiManager;

                LogHelper.Context.Log.InfoFormat("Finished processing element: {0}", rootElement.Name);
                LogHelper.Context.Log.InfoFormat("Loading configuration completed.");
            }
            catch (ConfigurationParseException e)
            {
                logAnErrorAndRethrowException(e, message => new ConfigurationParseException(e.ConfigurationFileElement, message, e.ParentConfigurationFileElement, false));
            }
            catch (Exception e)
            {
                logAnErrorAndRethrowException(e, message => new ConfigurationParseException(message));
            }
        }

        private System.Reflection.Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Action<string, object[]> logger = null;

            //This is an example of args.Name value: "TestPluginAssembly2, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null";
            LogHelper.Context.Log.DebugFormat("Resolving assembly '{0}'.", args.Name);
            var splitItems = args.Name.Split(',');

            if (splitItems != null && splitItems.Length >= 1)
            {
                var assemblyName = splitItems[0];

                var assemblyPath = _assemblyLocator.FindAssemblyPathInAllPluginFolders(assemblyName, args.RequestingAssembly.Location);

                LogHelper.Context.Log.DebugFormat("Loading assembly '{0}' to '{1}'.", args.Name, assemblyPath);
                if (assemblyPath != null)
                {
                    var resolvedAssembly = _assemblyLocator.LoadAssembly(Path.GetFileName(assemblyPath), Path.GetDirectoryName(assemblyPath));
                    LogHelper.Context.Log.DebugFormat("Loaded assembly '{0}' from '{1}'.", args.Name, assemblyPath);

                    if (string.Compare(args.Name, resolvedAssembly.GetName().FullName, StringComparison.OrdinalIgnoreCase) != 0)
                        LogHelper.Context.Log.WarnFormat("The assembly being resolved has a different version from the loaded assembly. The assembly being resolved is '{0}'. The loaded assembly is '{1}' loaded from '{2}'.",
                            args.Name, resolvedAssembly.GetName().FullName, assemblyPath);

                    return resolvedAssembly;
                }

                if (assemblyName.EndsWith(".resources"))
                    logger = LogHelper.Context.Log.WarnFormat;
            }

            if (logger == null)
                logger = LogHelper.Context.Log.FatalFormat;

            logger("Failed to resolve assembly '{0}'.", new object[] {args.Name});
            return null;
        }

        protected override void OnContainerStarted()
        {
            base.OnContainerStarted();

            ValidateRequiredSettings(DiContainer);

            _onApplicationsStarted = DiContainer.Resolve<IOnApplicationsStarted>();
            _onApplicationsStarted.StartStartupActions();
            LogHelper.Context.Log.Info("Applications and plugins started.");
        }

        private void OnDynamicAssemblyBuildCompleted(string assemblyPath, bool isSuccess, EmitResult emitResult)
        {
            if (!isSuccess)
                throw new ConfigurationParseException("Dynamic code generation failed.");
        }

        private void ProcessConfigurationFileElementAfterTreeConstructed(IConfigurationFileElement configurationFileElement, ref bool stopProcessing)
        {
            LogHelper.Context.Log.InfoFormat("Post processing element '{0}' after tree constructed.", configurationFileElement);
            configurationFileElement.ValidateOnTreeConstructed();
            LogHelper.Context.Log.InfoFormat("Finished processing element '{0}' after tree constructed.", configurationFileElement.ElementName);
        }

        private void ProcessDependencyInjectionSection([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
#pragma warning disable CS0612, CS0618
                                                       [NotNull] ITypesListFactoryTypeGenerator typesListFactoryTypeGenerator,
#pragma warning restore CS0612, CS0618
                                                       [NotNull] IDependencyInjection dependencyInjection,
                                                       [NotNull] string servicesModuleNamespace, [NotNull] string servicesModuleClassName,
                                                       [NotNull] IList<DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo> interfaceImplementationsInfo,
                                                       [NotNull] IList<ModuleInfo> allGeneratedModuleInfos)
        {
            foreach (var moduleElement in dependencyInjection.Modules.Modules)
                allGeneratedModuleInfos.Add(new ModuleInfo(this, _serviceRegistrationBuilder, moduleElement.DiModule));

            var services = new List<BindingConfigurationForFile>();

            foreach (var service in dependencyInjection.Services.AllServices)
            {
                if (!_addAssemblyReferenciesToDynamicAssemblyInOneMethodOnly)
                    dynamicAssemblyBuilder.AddReferencedAssembly(service.ServiceTypeInfo.Type);

                services.Add(BindingConfigurationForFile.CreateBindingConfigurationForFile(service.ServiceTypeInfo.Type, service.RegisterIfNotRegistered, service.Implementations));
            }

            AddReferencedAssemblies(dynamicAssemblyBuilder, services);

            var servicesModuleClassContent = Configuration.DiManagers.ActiveDiManagerElement.DiManager.GenerateModuleClassCode(dynamicAssemblyBuilder, _assemblyLocator,
                servicesModuleNamespace, servicesModuleClassName, services);

            dynamicAssemblyBuilder.AddCSharpFile(servicesModuleClassContent);
            allGeneratedModuleInfos.Add(new ModuleInfo(this, $"{servicesModuleNamespace}.{servicesModuleClassName}"));


            AddAutoGeneratedServices(dependencyInjection, interfaceImplementationsInfo, dynamicAssemblyBuilder);
#pragma warning disable CS0612, CS0618
            AddTypeFactories(dependencyInjection, interfaceImplementationsInfo, typesListFactoryTypeGenerator, dynamicAssemblyBuilder);
#pragma warning restore CS0612, CS0618
        }

        private void ValidateRequiredSettings([NotNull] IDiContainer diContainer)
        {
            LogHelper.Context.Log.DebugFormat("Validating required settings");

            ValidateRequiredSettings(diContainer.Resolve<ISettings>(), diContainer.Resolve<ISettingsRequestor>());

            var pluginDataRepository = diContainer.Resolve<IPluginDataRepository>();

            foreach (var pluginData in pluginDataRepository.Plugins)
                ValidateRequiredSettings(pluginData.Settings, pluginData.Plugin);

            LogHelper.Context.Log.DebugFormat("Completed validating required settings");
        }

        private void ValidateRequiredSettings([NotNull] ISettings settings, [NotNull] ISettingsRequestor settingsRequestor)
        {
            if (settingsRequestor.RequiredSettings == null)
                return;

            foreach (var requierdSetting in settingsRequestor.RequiredSettings)
            {
                var setting = settings.GetSetting(requierdSetting.Name);

                if (setting == null ||
                    !requierdSetting.ValueDataType.IsTypeAssignableFrom(setting.ValueTypeInfo.Type))
                {
                    ISettingsElement settingsElement = null;

                    var errorMessage = new StringBuilder();

                    if (setting == null)
                        errorMessage.Append($"Required setting '{requierdSetting.Name}' with value type of '{requierdSetting.ValueDataType.FullName}' is missing in ");
                    else
                        errorMessage.Append($"Required setting '{requierdSetting.Name}' should be of type '{requierdSetting.ValueDataType.FullName}'. Actual type is '{setting.ValueTypeInfo.TypeCSharpFullName}'. The setting is in ");

                    if (settingsRequestor is IPlugin plugin)
                    {
                        errorMessage.Append($"plugin settings for plugin '{plugin.PluginData.PluginName}'.");
                        settingsElement = Configuration.PluginsSetup.GetPluginSetup(plugin.PluginData.PluginName).SettingsElement;
                    }
                    else
                    {
                        errorMessage.Append("global settings.");
                        settingsElement = Configuration.SettingsElement;
                    }

                    errorMessage.Append($" To fix the issue, either modify the implementation of method '{nameof(ISettingsRequestor.RequiredSettings)}' in class '{settingsRequestor.GetType().FullName}', or add the setting.");

                    var errorMessageToString = errorMessage.ToString();
                    if (setting == null)
                        throw new ConfigurationParseException(settingsElement, errorMessageToString);

                    var settingElement = settingsElement.AllSettings.Where(x => string.Compare(requierdSetting.Name, x.Name, StringComparison.Ordinal) == 0).FirstOrDefault();

                    if (settingElement == null)
                    {
                        LogHelper.Context.Log.DebugFormat("Setting '{0}' is missing. This should never happen.", setting.Name);
                        throw new ConfigurationParseException(settingsElement, errorMessageToString);
                    }

                    throw new ConfigurationParseException(settingElement, errorMessageToString, settingsElement);
                }
            }
        }

        #endregion

        #region Nested Types

        private class ConcreteClassesRegistrationsModule : ModuleAbstr
        {
            #region Member Variables

            [NotNull]
            private readonly IConfiguration _configuration;

            [NotNull]
            [ItemNotNull]
            private readonly HashSet<Type> _registeredSelfBoundServices = new HashSet<Type>();

            #endregion

            #region  Constructors

            public ConcreteClassesRegistrationsModule([NotNull] IConfiguration configuration, [CanBeNull] [ItemNotNull] IEnumerable<IDiModule> additionalDiModules)
            {
                _configuration = configuration;

                if (additionalDiModules != null)
                    foreach (var diModule in additionalDiModules)
                        AccountForRegistrationsInModule(diModule);

                AccountForRegistrationsInDependencyInjectionElements(new[] {_configuration.DependencyInjection});
                AccountForRegistrationsInDependencyInjectionElements(_configuration.PluginsSetup.AllPluginSetups.Select(x => x.DependencyInjection).Where(x => x.Enabled));
            }

            #endregion

            #region Member Functions

            private void AccountForRegistrationsInDependencyInjectionElements(IEnumerable<IDependencyInjection> dependencyInjectionElements)
            {
                foreach (var dependencyInjection in dependencyInjectionElements)
                {
                    foreach (var moduleElement in dependencyInjection.Modules.Modules)
                        if (moduleElement.DiModule is IDiModule diModule)
                            AccountForRegistrationsInModule((IDiModule) moduleElement.DiModule);

                    foreach (var service in dependencyInjection.Services.AllServices)
                        if (service.ServiceTypeInfo.Type.IsClass && !_registeredSelfBoundServices.Contains(service.ServiceTypeInfo.Type))
                            _registeredSelfBoundServices.Add(service.ServiceTypeInfo.Type);
                }
            }

            private void AccountForRegistrationsInModule([NotNull] IDiModule diModule)
            {
                foreach (var serviceBindingConfiguration in diModule.ServiceBindingConfigurations)
                    if (serviceBindingConfiguration.ServiceType.IsClass && !_registeredSelfBoundServices.Contains(serviceBindingConfiguration.ServiceType))
                        _registeredSelfBoundServices.Add(serviceBindingConfiguration.ServiceType);
            }

            protected override void AddServiceRegistrations()
            {
                _configuration.ProcessTree(ProcessConfigurationFileElement);
            }

            private void ProcessConfigurationFileElement([NotNull] IConfigurationFileElement configurationFileElement, ref bool stopProcessing)
            {
                if (configurationFileElement is IValueInitializerElement valueInitializerElement &&
                    valueInitializerElement.IsResolvedFromDiContainer)
                {
                    RegisterSelfBoundService(valueInitializerElement.ValueTypeInfo.Type);
                }
                else if (configurationFileElement is AutoGeneratedMemberReturnValuesIfSelectorElement autoGeneratedMemberReturnValuesIfSelectorElement)
                {
                    foreach (var parameterValueInitializer in autoGeneratedMemberReturnValuesIfSelectorElement.ParameterValueInitializers)
                    {
                        if (parameterValueInitializer.ValueInitializer is IClassMemberValueInitializer classMemberValueInitializer &&
                            classMemberValueInitializer.ClassMemberData.IsInjectedClassMember &&
                            classMemberValueInitializer.ClassMemberData.ClassMemberInfo.DeclaringType.IsClass)
                        {
                            RegisterSelfBoundService(classMemberValueInitializer.ClassMemberData.ClassMemberInfo.DeclaringType);
                        }
                    }
                }

#pragma warning disable CS0612, CS0618
                if (configurationFileElement is ITypeFactoryReturnedType)
                {
                    RegisterSelfBoundService(((ITypeFactoryReturnedType) configurationFileElement).ReturnedType);
                }
#pragma warning restore CS0612, CS0618
            }


            private void RegisterSelfBoundService([NotNull] Type type)
            {
                if (_registeredSelfBoundServices.Contains(type))
                    return;

                if (!type.IsClass || type.IsAbstract || type.GetConstructors().FirstOrDefault(x => x.IsPublic) == null)
                    return;

                Bind(type).OnlyIfNotRegistered().ToSelf();
                _registeredSelfBoundServices.Add(type);
            }

            #endregion
        }

        private class DefaultModule : ModuleAbstr
        {
            #region Member Variables

            [NotNull]
            private readonly FileBasedConfiguration _fileBasedConfiguration;

            #endregion

            #region  Constructors

            public DefaultModule([NotNull] FileBasedConfiguration fileBasedConfiguration)
            {
                _fileBasedConfiguration = fileBasedConfiguration;
            }

            #endregion

            #region Member Functions

            /// <summary>
            ///     Use OnlyIfNotRegistered with all binding configurations, to use custom binding that the user might have specified
            ///     in configuration
            ///     file or in modules.
            /// </summary>
            protected override void AddServiceRegistrations()
            {
                Bind<ITypeBasedSimpleSerializerAggregator>().OnlyIfNotRegistered().To(typeResolver => _fileBasedConfiguration.Configuration.ParameterSerializers.TypeBasedSimpleSerializerAggregator)
                                                            .SetResolutionScope(DiResolutionScope.Singleton);

                Bind<IConfiguration>().OnlyIfNotRegistered().To(typeResolver => _fileBasedConfiguration.Configuration).SetResolutionScope(DiResolutionScope.Singleton);

                Bind<IDiContainer>().OnlyIfNotRegistered().To(typeResolver => _fileBasedConfiguration.DiContainer).SetResolutionScope(DiResolutionScope.Singleton);

                Bind<IPluginsSetup>().OnlyIfNotRegistered().To(typeResolver => _fileBasedConfiguration.Configuration.PluginsSetup).SetResolutionScope(DiResolutionScope.Singleton);
                Bind<ISettingsElement>().OnlyIfNotRegistered().To(typeResolver => _fileBasedConfiguration.Configuration.SettingsElement).SetResolutionScope(DiResolutionScope.Singleton);
                Bind<ISettings>().OnlyIfNotRegistered().To<Settings>().SetResolutionScope(DiResolutionScope.Singleton);
                Bind<ISettingsRequestor>().OnlyIfNotRegistered().To<SettingsRequestorDefault>().SetResolutionScope(DiResolutionScope.Singleton);
                Bind<IPluginDataRepository>().OnlyIfNotRegistered().To<PluginDataRepository>().SetResolutionScope(DiResolutionScope.Singleton);
                Bind<IOnApplicationsStarted>().OnlyIfNotRegistered().To<OnApplicationsStarted>().SetResolutionScope(DiResolutionScope.Singleton);
            }

            #endregion
        }

        private class DynamicHelperClassesStarter : IDisposable
        {
            #region Member Variables

            private readonly List<IDynamicallyGeneratedClass> _dynamicallyGeneratedClasses = new List<IDynamicallyGeneratedClass>(10);

            [NotNull]
            private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;

            #endregion

            #region  Constructors

            public DynamicHelperClassesStarter([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder, IConfiguration configuration)
            {
                _dynamicAssemblyBuilder = dynamicAssemblyBuilder;

                _dynamicallyGeneratedClasses.Add(
                    _dynamicAssemblyBuilder.StartDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.DynamicImplementationsClassName));

                _dynamicallyGeneratedClasses.Add(
                    _dynamicAssemblyBuilder.StartDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.ClassMembersClassName));

                if (configuration.SettingsElement != null)
                    _dynamicallyGeneratedClasses.Add(
                        _dynamicAssemblyBuilder.StartDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.SettingValuesClassName));

                foreach (var pluginSetup in configuration.PluginsSetup.AllPluginSetups)
                {
                    if (!pluginSetup.Enabled)
                        continue;

                    _dynamicallyGeneratedClasses.Add(
                        _dynamicAssemblyBuilder.StartDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.GetPluginSettingValuesClassName(pluginSetup.OwningPluginElement.Name)));
                }
            }

            #endregion

            #region IDisposable Interface Implementation

            public void Dispose()
            {
                foreach (var dynamicallyGeneratedClass in _dynamicallyGeneratedClasses)
                    _dynamicAssemblyBuilder.FinalizeDynamicallyGeneratedClass(dynamicallyGeneratedClass.ClassName);
            }

            #endregion
        }

        /// <summary>
        ///     This class stores details about native and non-native modules, and initializes the modules at a later time,
        ///     when dynamic assembly is created.
        /// </summary>
        private class ModuleInfo
        {
            #region Member Variables

            [NotNull]
            private readonly FileBasedConfiguration _fileBasedConfiguration;

            private readonly string _moduleClassFullName;

            #endregion

            #region  Constructors

            /// <summary>
            ///     Use this constructor for either native modules (e.g., Autofac or Ninject module), or modules of type
            ///     <see cref="IDiModule" />.
            /// </summary>
            public ModuleInfo([NotNull] FileBasedConfiguration fileBasedConfiguration,
                              [NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                              [NotNull] object nativeOrDiModule)
            {
                _fileBasedConfiguration = fileBasedConfiguration;

                if (nativeOrDiModule is IDiModule diModule)
                {
                    diModule.Init(serviceRegistrationBuilder);
                    NativeModuleObject = _fileBasedConfiguration.DiManager.GenerateNativeModule(diModule);
                }
                else
                {
                    NativeModuleObject = nativeOrDiModule;
                }
            }

            /// <summary>
            ///     This constructor specifies a class full name for a native module.
            /// </summary>
            /// <param name="fileBasedConfiguration"></param>
            /// <param name="moduleClassFullName"></param>
            public ModuleInfo([NotNull] FileBasedConfiguration fileBasedConfiguration, string moduleClassFullName)
            {
                _fileBasedConfiguration = fileBasedConfiguration;
                _moduleClassFullName = moduleClassFullName;
            }

            #endregion

            #region Member Functions

            public object GetNativeModule()
            {
                if (NativeModuleObject != null)
                    return NativeModuleObject;

                if (_moduleClassFullName != null)
                {
                    if (_fileBasedConfiguration._loadedDynamicAssembly == null)
                        _fileBasedConfiguration._loadedDynamicAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(
                            _fileBasedConfiguration.GetDynamicallyGeneratedAssemblyFilePath());

                    var nativeModuleType = _fileBasedConfiguration._loadedDynamicAssembly.GetType(_moduleClassFullName);

                    var moduleClassConstructor = nativeModuleType.GetConstructor(new Type[] { });

                    if (moduleClassConstructor == null)
                        throw new Exception($"Class '{_moduleClassFullName}' is expected to have a constructor with a single parameter of type '{typeof(IDiContainer).FullName}'");

                    NativeModuleObject = moduleClassConstructor.Invoke(new object[] { });
                }
                else
                {
                    // This should never happen.
                    throw new Exception($"Object '{this}' is not properly initialized.");
                }

                return NativeModuleObject;
            }

            public object NativeModuleObject { get; private set; }

            public override string ToString()
            {
                if (_moduleClassFullName != null)
                    return $"{GetType()}-{_moduleClassFullName}";
                return $"{GetType()}-{NativeModuleObject.GetType().FullName}";
            }

            #endregion
        }

        #endregion
    }
}