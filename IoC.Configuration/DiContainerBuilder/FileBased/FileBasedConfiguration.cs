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

using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DependencyInjection;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainer.BindingsForCode;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.Emit;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.DynamicCode;
using OROptimizer.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public class FileBasedConfiguration : DiContainerBuilderConfiguration
    {
        [NotNull] private readonly FileBasedConfigurationParameters _fileBasedConfigurationParameters;

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [NotNull]
        private readonly IConfigurationFileElementFactory _configurationFileElementFactory;

        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<string, HashSet<string>> _directoriesToCleanup = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
 
        private readonly string _dynamicallyGeneratedAssemblyFileName;

        private readonly string _dynamicImplementationsNamespace;

        private System.Reflection.Assembly _loadedDynamicAssembly;

        private readonly object _lockObject = new object();

        [CanBeNull]
        private IOnApplicationsStarted _onApplicationsStarted;

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
        ///     A delegate executed when the configuration file XML document is loaded.
        /// </param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public FileBasedConfiguration([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider,
                                      [NotNull] string entryAssemblyFolder,
                                      [CanBeNull] ConfigurationFileXmlDocumentLoadedEventHandler configurationFileXmlDocumentLoaded) : 
            this(configurationFileContentsProvider, entryAssemblyFolder, new AllLoadedAssemblies(), configurationFileXmlDocumentLoaded)
        {
        }

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
        ///     A delegate executed when the configuration file XML document is loaded.
        /// </param>
        /// <param name="loadedAssemblies"> Instance of <see cref="ILoadedAssemblies"/> used to add add all or some of currently
        ///                     loaded assemblies as dependencies for  dynamically generated assemblies.
        ///                     Use an instance of <see cref="AllLoadedAssemblies"/> to add references to all assemblies loaded into current application
        ///                     domain to the dynamically generated assembly. Use <see cref="NoLoadedAssemblies"/> to not add any additional assemblies
        ///                     references to any additional assemblies as dependencies for  dynamically generated assemblies.
        ///                     Provide your own implementation to add only some of loaded assemblies as dependencies.
        /// </param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public FileBasedConfiguration([NotNull] IConfigurationFileContentsProvider configurationFileContentsProvider,
                                      [NotNull] string entryAssemblyFolder, [NotNull] ILoadedAssemblies loadedAssemblies,
                                      [CanBeNull] ConfigurationFileXmlDocumentLoadedEventHandler configurationFileXmlDocumentLoaded) : 
            this(new FileBasedConfigurationParameters(configurationFileContentsProvider, entryAssemblyFolder, loadedAssemblies)
            {
                ConfigurationFileXmlDocumentLoaded = configurationFileXmlDocumentLoaded
            })
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileBasedConfiguration" /> class.
        /// </summary>
        /// <param name="fileBasedConfigurationParameters">An instance of <see cref="FileBasedConfigurationParameters"/> used to load and process the configuration file.</param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public FileBasedConfiguration([NotNull] FileBasedConfigurationParameters fileBasedConfigurationParameters) : 
            base(fileBasedConfigurationParameters.EntryAssemblyFolder)
        {
            _fileBasedConfigurationParameters = fileBasedConfigurationParameters;

            var assemblyLocator = IoCServiceFactoryAmbientContext.Context.CreateAssemblyLocator(() => Configuration, fileBasedConfigurationParameters.EntryAssemblyFolder);

            _assemblyLocator = assemblyLocator;
            _configurationFileElementFactory = new ConfigurationFileElementFactory(assemblyLocator);

            var uniqueId = GlobalsCoreAmbientContext.Context.GenerateUniqueId();
            _dynamicImplementationsNamespace = $"DynamicImplementations_{uniqueId}";
            _dynamicallyGeneratedAssemblyFileName = $"DynamicallyGeneratedAssembly_{uniqueId}.dll";
        }

        private void AddAssemblyReferencesFromAssembliesElementToDynamicAssembly([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder)
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
                try
                {
                    autoGeneratedServiceElement.GenerateAutoImplementedServiceClassCSharp(dynamicAssemblyBuilder, _dynamicImplementationsNamespace, out var generateClassFullName);
                    interfaceImplementationsInfo.Add(new DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo(autoGeneratedServiceElement.ImplementedInterfaceTypeInfo.Type,
                        generateClassFullName));
                }
                catch (ConfigurationParseException)
                {
                    throw;
                }
                catch(Exception e)
                {
                    LogHelper.Context.Log.Error(e);

                    throw new ConfigurationParseException(autoGeneratedServiceElement, $"Call to '{typeof(IAutoGeneratedServiceElementBase)}.{nameof(IAutoGeneratedServiceElementBase.GenerateAutoImplementedServiceClassCSharp)}' failed.");
                }
            }
        }

        private delegate void ProcessElementDelegate([NotNull] XmlElement xmlElement, [NotNull] string elementPath);

        private void ProcessXmlElements([NotNull] XmlElement xmlElement, [NotNull] string elementPath, [NotNull] ProcessElementDelegate processElementDelegate)
        {
            processElementDelegate(xmlElement, elementPath);

           
            foreach (var childXmlNode in xmlElement.ChildNodes)
            {
                if (!(childXmlNode is XmlElement childXmlElement))
                    continue;

                ProcessXmlElements(childXmlElement, $"{elementPath}/{childXmlElement.Name}", processElementDelegate);
            }
        }

        private void AddFileConfigurationElements([NotNull] XmlNode xmlNode, [NotNull] IConfigurationFileElement parentElement)
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
                                       IList<DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo> interfaceImplementationsInfo,
                                       IList<ModuleInfo> allGeneratedModuleInfos)
        {
            var serviceImplementationElements = new List<IServiceImplementationElement>();

            foreach (var pluginSetup in Configuration.PluginsSetup.AllPluginSetups)
            {
                if (!pluginSetup.Enabled)
                    continue;

                serviceImplementationElements.Add(pluginSetup.PluginImplementationElement);

                if (pluginSetup.DependencyInjection != null)
                    ProcessDependencyInjectionSection(dynamicAssemblyBuilder, pluginSetup.DependencyInjection,
                        $"{_dynamicImplementationsNamespace}.{pluginSetup.Plugin.Name}_{OROptimizer.GlobalsCoreAmbientContext.Context.GenerateUniqueId()}", "PluginServicesModule",
                        interfaceImplementationsInfo, allGeneratedModuleInfos);
            }

            if (serviceImplementationElements.Count > 0)
                servicesToAddBindingsTo.Add(BindingConfigurationForFile.CreateBindingConfigurationForFile(typeof(IPlugin), false, serviceImplementationElements));
        }

        //private void AddReferencedAssemblies([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
        //                                     [NotNull] [ItemNotNull] IEnumerable<BindingImplementationConfigurationForFile> bindingImplementationConfigurations)
        //{
        //    foreach (var bindingImplementationConfiguration in bindingImplementationConfigurations)
        //    {
        //        dynamicAssemblyBuilder.AddReferencedAssembly(bindingImplementationConfiguration.ImplementationType);

        //        if (bindingImplementationConfiguration is TypeBasedBindingImplementationConfigurationForFile typeBasedBindingImplementationConfigurationForFile)
        //        {
        //            if (typeBasedBindingImplementationConfigurationForFile.Parameters != null)
        //                foreach (var parameter in typeBasedBindingImplementationConfigurationForFile.Parameters)
        //                    dynamicAssemblyBuilder.AddReferencedAssembly(parameter.ValueTypeInfo.Type);

        //            if (typeBasedBindingImplementationConfigurationForFile.InjectedProperties != null)
        //                foreach (var injectedProperty in typeBasedBindingImplementationConfigurationForFile.InjectedProperties)
        //                    dynamicAssemblyBuilder.AddReferencedAssembly(injectedProperty.ValueTypeInfo.Type);
        //        }
        //    }
        //}

        private void AddSettingsRequestorBindings([NotNull] IList<BindingConfigurationForFile> servicesToAddBindingsTo, [NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            if (Configuration.SettingsRequestor == null || !Configuration.SettingsRequestor.Enabled)
                return;

            var serviceImplementationElements = new List<IServiceImplementationElement> {Configuration.SettingsRequestor};
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

            EmitResult dynamicAssemblyEmitResult = null;

            void OnDynamicAssemblyBuildCompleted(string assemblyPath, bool isSuccess, EmitResult emitResult)
            {
                _fileBasedConfigurationParameters.OnDynamicAssemblyEmitComplete?.Invoke(assemblyPath, isSuccess, emitResult);

                dynamicAssemblyEmitResult = emitResult;
            }

            using (var dynamicAssemblyBuilder =
                GlobalsCoreAmbientContext.Context.StartDynamicAssemblyBuilder(dynamicallyGeneratedAssemblyFilePath, OnDynamicAssemblyBuildCompleted, 
                    _fileBasedConfigurationParameters.LoadedAssemblies,
                    _fileBasedConfigurationParameters.AdditionalReferencedAssemblies?.ToArray()))
            {
                dynamicAssemblyBuilder.StartDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.DynamicImplementationsClassName);
                dynamicAssemblyBuilder.StartDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.ClassMembersClassName);

                if (Configuration.SettingsElement != null)
                    dynamicAssemblyBuilder.StartDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.SettingValuesClassName);

                foreach (var pluginSetup in Configuration.PluginsSetup.AllPluginSetups)
                {
                    if (!pluginSetup.Enabled)
                        continue;

                    dynamicAssemblyBuilder.StartDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.GetPluginSettingValuesClassName(pluginSetup.OwningPluginElement.Name));
                }

                // Do not use IoC.Configuration in namespace, since it causes some naming conflicts with Autofac modules.

                AddAssemblyReferencesFromAssembliesElementToDynamicAssembly(dynamicAssemblyBuilder);

                if (Configuration.DependencyInjection != null)
                    ProcessDependencyInjectionSection(dynamicAssemblyBuilder, Configuration.DependencyInjection, _dynamicImplementationsNamespace, "ServicesModule", interfaceImplementationsInfo, allGeneratedModuleInfos);

                var additionalServices = new List<BindingConfigurationForFile>();

                AddSettingsRequestorBindings(additionalServices, dynamicAssemblyBuilder);
                AddStartupActionBindings(additionalServices, dynamicAssemblyBuilder);
                AddPluginBindings(additionalServices, dynamicAssemblyBuilder, interfaceImplementationsInfo, allGeneratedModuleInfos);

                // Add additional bindings after plugin bindings are handled (such as plugin implementations)
                var additionalServicesClassName = "AdditionalServices";

                var additionalServicesModuleClassContent =
                    Configuration.DiManagers.ActiveDiManagerElement.DiManager.GenerateModuleClassCode(dynamicAssemblyBuilder, _assemblyLocator,
                        _dynamicImplementationsNamespace, additionalServicesClassName, additionalServices);

                dynamicAssemblyBuilder.AddCSharpFile(additionalServicesModuleClassContent);
                allGeneratedModuleInfos.Add(new ModuleInfo(this, $"{_dynamicImplementationsNamespace}.{additionalServicesClassName}"));
            }

            if (dynamicAssemblyEmitResult == null || !dynamicAssemblyEmitResult.Success)
                throw new DynamicCodeGenerationException(dynamicallyGeneratedAssemblyFilePath, dynamicAssemblyEmitResult);

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
            Load();
        }

        private void Load()
        {
            LogHelper.Context.Log.InfoFormat("Loading configuration started.");
            var schemaSet = new XmlSchemaSet();

            void LogAnErrorAndRethrowException(Exception originalException, Func<string, Exception> createRethrownException)
            {
                LogHelper.Context.Log.Fatal(originalException);
                throw createRethrownException($"Failed to parse the xml file '{_fileBasedConfigurationParameters.ConfigurationFileContentsProvider.ConfigurationFileSourceDetails}'.");
            }

            try
            {
                // Load XML schema file
                XmlSchema xmlSchema = null;

                LogHelper.Context.Log.InfoFormat("Loading xml schema '{0}'", HelpersIoC.IoCConfigurationSchemaName);

                var resourceName = $"IoC.Configuration.EmbeddedResources.{HelpersIoC.IoCConfigurationSchemaName}";

                using (var stream = GetType().Assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new Exception($"Failed to load the XML Schema from resource '{resourceName}'.");
                    
                    xmlSchema = XmlSchema.Read(stream, (sender, e) =>
                    {
                        if (e.Exception != null)
                            throw e.Exception;

                        if (e.Severity == XmlSeverityType.Error)
                            throw new Exception($"Error in XML Schema : '{HelpersIoC.IoCConfigurationSchemaName}'. Schema load message: {e.Message}");
                    });
                }

                schemaSet.Add(xmlSchema);

                // Load XML file
                var xmlDocument = new XmlDocument {Schemas = schemaSet};

                LogHelper.Context.Log.InfoFormat("Loading XML configuration file from '{0}'.", _fileBasedConfigurationParameters.ConfigurationFileContentsProvider.ConfigurationFileSourceDetails);
                
                using (var stringReader = new StringReader(_fileBasedConfigurationParameters.ConfigurationFileContentsProvider.LoadConfigurationFileContents()))
                {
                    xmlDocument.Load(stringReader);
                }

                // Process loaded XML
                var nodes = xmlDocument.GetElementsByTagName(ConfigurationFileElementNames.RootElement);

                if (nodes.Count != 1 || !(nodes.Item(0) is XmlElement rootElement))
                    throw new Exception($"Expected '{ConfigurationFileElementNames.RootElement}' element as a root element in configuration file.");

                var attributeValueTransformers = _fileBasedConfigurationParameters.AttributeValueTransformers;
                if (attributeValueTransformers != null)
                {
                    var attributeValueTransformersList = attributeValueTransformers.ToList();

                    if (attributeValueTransformersList.Count > 0)
                    {
                        ProcessXmlElements(rootElement, $"/{ConfigurationFileElementNames.RootElement}",
                            (xmlElement, xmlElementPath) =>
                        {
                            foreach (var attributeObject in xmlElement.Attributes)
                            {
                                if (!(attributeObject is XmlAttribute xmlAttribute))
                                    continue;

                                foreach (var attributeValueProvider in attributeValueTransformersList)
                                {
                                    if (attributeValueProvider.TryGetAttributeValue(xmlElementPath, xmlAttribute, out var attributeValue))
                                        xmlElement.SetAttribute(xmlAttribute.Name, attributeValue ?? string.Empty);
                                }
                            }
                        });
                    }
                }

                _fileBasedConfigurationParameters.ConfigurationFileXmlDocumentLoaded?.Invoke(this, new ConfigurationFileXmlDocumentLoadedEventArgs(xmlDocument));

                LogHelper.Context.Log.InfoFormat("Validating the configuration file against the schema '{0}'.",
                    HelpersIoC.IoCConfigurationSchemaName);

                xmlDocument.Validate((sender, e) =>
                {
                    if (e.Exception != null)
                        throw e.Exception;

                    if (e.Severity == XmlSeverityType.Error)
                        throw new Exception($"Errors in XML file: '{_fileBasedConfigurationParameters.ConfigurationFileContentsProvider.ConfigurationFileSourceDetails}'.");
                });

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
                LogAnErrorAndRethrowException(e, message => new ConfigurationParseException(e.ConfigurationFileElement, message, e.ParentConfigurationFileElement, false));
            }
            catch (Exception e)
            {
                LogAnErrorAndRethrowException(e, message => new ConfigurationParseException(message));
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

            void LogAnErrorAndRethrowException(Exception originalException, Func<string, Exception> createRethrownException)
            {
                LogHelper.Context.Log.Fatal(originalException);
                throw createRethrownException($"Validation of xml file '{_fileBasedConfigurationParameters.ConfigurationFileContentsProvider.ConfigurationFileSourceDetails}' failed after the DI container was loaded.");
            }

            try
            {
                ValidateConfigurationFileElementsOnContainerLoaded(DiContainer);

                ValidateRequiredSettings(DiContainer);

                _onApplicationsStarted = DiContainer.Resolve<IOnApplicationsStarted>();
                _onApplicationsStarted.StartStartupActions();
                LogHelper.Context.Log.Info("Applications and plugins started.");
            }
            catch (ConfigurationParseException e)
            {
                LogAnErrorAndRethrowException(e, message => new ConfigurationParseException(e.ConfigurationFileElement, message, e.ParentConfigurationFileElement, false));
            }
            catch (Exception e)
            {
                LogAnErrorAndRethrowException(e, message => new ConfigurationParseException(message));
            }
        }

        private void ProcessConfigurationFileElementAfterTreeConstructed(IConfigurationFileElement configurationFileElement, ref bool stopProcessing)
        {
            LogHelper.Context.Log.InfoFormat("Post processing element '{0}' after tree constructed.", configurationFileElement);
            configurationFileElement.ValidateOnTreeConstructed();
            LogHelper.Context.Log.InfoFormat("Finished processing element '{0}' after tree constructed.", configurationFileElement.ElementName);
        }

        private void ProcessDependencyInjectionSection([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
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
                services.Add(BindingConfigurationForFile.CreateBindingConfigurationForFile(service.ServiceTypeInfo.Type, service.RegisterIfNotRegistered, service.Implementations));
            }

            var servicesModuleClassContent = Configuration.DiManagers.ActiveDiManagerElement.DiManager.GenerateModuleClassCode(dynamicAssemblyBuilder, _assemblyLocator,
                servicesModuleNamespace, servicesModuleClassName, services);

            dynamicAssemblyBuilder.AddCSharpFile(servicesModuleClassContent);
            allGeneratedModuleInfos.Add(new ModuleInfo(this, $"{servicesModuleNamespace}.{servicesModuleClassName}"));

            AddAutoGeneratedServices(dependencyInjection, interfaceImplementationsInfo, dynamicAssemblyBuilder);
        }

        private void ValidateConfigurationFileElementsOnContainerLoaded([NotNull] IDiContainer diContainer)
        {
            LogHelper.Context.Log.DebugFormat("Validating configuration file elements on container loaded.");

            void ValidateConfigurationElement(IConfigurationFileElement configurationFileElement, ref bool stopProcessing)
            {
                configurationFileElement.ValidateOnContainerLoaded(diContainer);
            }

            Configuration.ProcessTree(ValidateConfigurationElement);

            LogHelper.Context.Log.DebugFormat("Completed validating configuration file elements on container loaded.");
        }

        private void ValidateRequiredSettings([NotNull] IDiContainer diContainer)
        {
            LogHelper.Context.Log.DebugFormat("Validating required settings on container loaded.");

            ValidateRequiredSettings(diContainer.Resolve<ISettings>(), diContainer.Resolve<ISettingsRequestor>());

            var pluginDataRepository = diContainer.Resolve<IPluginDataRepository>();

            foreach (var pluginData in pluginDataRepository.Plugins)
                ValidateRequiredSettings(pluginData.Settings, pluginData.Plugin);

            LogHelper.Context.Log.DebugFormat("Completed validating required settings on container loaded.");
        }

        private void ValidateRequiredSettings([NotNull] ISettings settings, [NotNull] ISettingsRequestor settingsRequestor)
        {
            if (settingsRequestor.RequiredSettings == null)
                return;

            foreach (var requiredSetting in settingsRequestor.RequiredSettings)
            {
                var setting = settings.GetSetting(requiredSetting.Name);

                if (setting == null ||
                    !requiredSetting.ValueDataType.IsTypeAssignableFrom(setting.ValueTypeInfo.Type))
                {
                    ISettingsElement settingsElement = null;

                    var errorMessage = new StringBuilder();

                    if (setting == null)
                        errorMessage.Append($"Required setting '{requiredSetting.Name}' with value type of '{requiredSetting.ValueDataType.FullName}' is missing in ");
                    else
                        errorMessage.Append($"Required setting '{requiredSetting.Name}' should be of type '{requiredSetting.ValueDataType.FullName}'. Actual type is '{setting.ValueTypeInfo.TypeCSharpFullName}'. The setting is in ");

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

                    var settingElement = settingsElement.AllSettings.FirstOrDefault(x => string.Compare(requiredSetting.Name, x.Name, StringComparison.Ordinal) == 0);

                    if (settingElement == null)
                    {
                        LogHelper.Context.Log.DebugFormat("Setting '{0}' is missing. This should never happen.", setting.Name);
                        throw new ConfigurationParseException(settingsElement, errorMessageToString);
                    }

                    throw new ConfigurationParseException(settingElement, errorMessageToString, settingsElement);
                }
            }
        }

        private class ConcreteClassesRegistrationsModule : ModuleAbstr
        {
            [NotNull]
            private readonly IConfiguration _configuration;

            [NotNull]
            [ItemNotNull]
            private readonly HashSet<Type> _registeredSelfBoundServices = new HashSet<Type>();

           

         

            public ConcreteClassesRegistrationsModule([NotNull] IConfiguration configuration, [CanBeNull] [ItemNotNull] IEnumerable<IDiModule> additionalDiModules)
            {
                _configuration = configuration;

                if (additionalDiModules != null)
                    foreach (var diModule in additionalDiModules)

                        AccountForRegistrationsInModule(diModule);

                AccountForRegistrationsInDependencyInjectionElements(new[] {_configuration.DependencyInjection});
                AccountForRegistrationsInDependencyInjectionElements(_configuration.PluginsSetup.AllPluginSetups.Select(x => x.DependencyInjection).Where(x => x.Enabled));
            }

            private void AccountForRegistrationsInDependencyInjectionElements(IEnumerable<IDependencyInjection> dependencyInjectionElements)
            {
                foreach (var dependencyInjection in dependencyInjectionElements)
                {
                    foreach (var moduleElement in dependencyInjection.Modules.Modules)
                        if (moduleElement.DiModule is IDiModule diModule)
                            AccountForRegistrationsInModule(diModule);

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
            }


            private void RegisterSelfBoundService([NotNull] Type type)
            {
                if (_registeredSelfBoundServices.Contains(type))
                    return;

                if (!type.IsClass || type.IsAbstract || type.GetConstructors().FirstOrDefault(x => x.IsPublic) == null)
                    return;

                Bind(type).OnlyIfNotRegistered().ToSelf().SetResolutionScope(DiResolutionScope.Singleton);
                _registeredSelfBoundServices.Add(type);
            }
        }

        private class DefaultModule : ModuleAbstr
        {
            [NotNull]
            private readonly FileBasedConfiguration _fileBasedConfiguration;
            public DefaultModule([NotNull] FileBasedConfiguration fileBasedConfiguration)
            {
                _fileBasedConfiguration = fileBasedConfiguration;
            }

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
        }

        /// <summary>
        ///     This class stores details about native and non-native modules, and initializes the modules at a later time,
        ///     when dynamic assembly is created.
        /// </summary>
        private class ModuleInfo
        {
            [NotNull]
            private readonly FileBasedConfiguration _fileBasedConfiguration;

            private readonly string _moduleClassFullName;

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

            public object GetNativeModule()
            {
                if (NativeModuleObject != null)
                    return NativeModuleObject;

                if (_moduleClassFullName != null)
                {
                    if (_fileBasedConfiguration._loadedDynamicAssembly == null)
                        _fileBasedConfiguration._loadedDynamicAssembly = GlobalsCoreAmbientContext.Context.LoadAssembly(
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

        }
    }
}