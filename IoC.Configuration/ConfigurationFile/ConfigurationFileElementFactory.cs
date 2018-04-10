using System;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class ConfigurationFileElementFactory : IConfigurationFileElementFactory
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        #endregion

        #region  Constructors

        public ConfigurationFileElementFactory([NotNull] IAssemblyLocator assemblyLocator)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IConfigurationFileElementFactory Interface Implementation

        public IConfiguration CreateConfiguration(XmlElement xmlElement)
        {
            if (!ConfigurationFileElementNames.RootElement.Equals(xmlElement.Name, StringComparison.Ordinal))
                throw new ConfigurationParseException($"Root element should be an '{ConfigurationFileElementNames.RootElement}' element.");

            var configurationFileElement = new Configuration(xmlElement);
            configurationFileElement.Initialize();
            return configurationFileElement;
        }

        public IConfigurationFileElement CreateConfigurationFileElement(XmlElement xmlElement, IConfigurationFileElement parentConfigurationFileElement)
        {
            IConfigurationFileElement configurationFileElement = null;

            var serializerAggregator = parentConfigurationFileElement.Configuration?.ParameterSerializers?.TypeBasedSimpleSerializerAggregator;

            if (serializerAggregator == null)
                serializerAggregator = TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator();

            switch (xmlElement.Name)
            {
                case ConfigurationFileElementNames.AppDataDir:
                    configurationFileElement = new ApplicationDataDirectory(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.Plugins:
                    configurationFileElement = new Plugins(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.Plugin:
                    configurationFileElement = new PluginElement(xmlElement, Helpers.ConvertTo<IPlugins>(parentConfigurationFileElement));
                    break;

                case ConfigurationFileElementNames.AdditionalAssemblyProbingPaths:
                    configurationFileElement = new AdditionalAssemblyProbingPaths(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.ProbingPath:
                    configurationFileElement = new ProbingPath(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.Assemblies:
                    configurationFileElement = new Assemblies(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.Assembly:
                    configurationFileElement = new Assembly(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.ParameterSerializers:
                    configurationFileElement = new ParameterSerializers(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.Serializers:
                    configurationFileElement = new ParameterSerializersCollection(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.ParameterSerializer:
                    configurationFileElement = new ParameterSerializer(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.Parameters:
                    configurationFileElement = new Parameters(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.DiManagers:
                    configurationFileElement = new DiManagersElement(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.DiManager:
                    configurationFileElement = new DiManagerElement(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.Settings:
                    if (parentConfigurationFileElement.OwningPluginElement == null)
                        configurationFileElement = new SettingsElement(xmlElement, parentConfigurationFileElement);
                    else
                        configurationFileElement = new PluginSettingsElement(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.ValueByte:
                case ConfigurationFileElementNames.ValueInt16:
                case ConfigurationFileElementNames.ValueInt32:
                case ConfigurationFileElementNames.ValueInt64:
                case ConfigurationFileElementNames.ValueDouble:
                case ConfigurationFileElementNames.ValueString:
                case ConfigurationFileElementNames.ValueBoolean:
                case ConfigurationFileElementNames.ValueDateTime:
                case ConfigurationFileElementNames.ValueObject:
                case ConfigurationFileElementNames.ValueInjectedObject:

                    if (parentConfigurationFileElement is IParameters)
                        configurationFileElement = new ParameterElement(xmlElement, parentConfigurationFileElement, serializerAggregator, _assemblyLocator);
                    else if (parentConfigurationFileElement is ISettingsElement)
                        configurationFileElement = new SettingElement(xmlElement, parentConfigurationFileElement, serializerAggregator, _assemblyLocator);
                    else if (parentConfigurationFileElement is IInjectedProperties)
                        configurationFileElement = new InjectedPropertyElement(xmlElement, parentConfigurationFileElement, serializerAggregator, _assemblyLocator);

                    break;

                case ConfigurationFileElementNames.DependencyInjection:
                    configurationFileElement = new DependencyInjection(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.Modules:
                    configurationFileElement = new ModulesElement(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.Module:
                    configurationFileElement = new ModuleElement(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.Services:
                    configurationFileElement = new Services(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.Service:
                    configurationFileElement = new ServiceElement(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.SelfBoundService:
                    configurationFileElement = new SelfBoundServiceElement(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.Implementation:
                    configurationFileElement = new ServiceImplementationElement(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.InjectedProperties:
                    configurationFileElement = new InjectedProperties(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.AutoGeneratedServices:
                    configurationFileElement = new AutoGeneratedServices(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.TypeFactory:
                    var typesListFactoryTypeGenerator = IoCServiceFactoryAmbientContext.Context.CreateTypesListFactoryTypeGenerator(serializerAggregator);
                    configurationFileElement = new TypeFactory(xmlElement, parentConfigurationFileElement, _assemblyLocator, typesListFactoryTypeGenerator);
                    break;

                case ConfigurationFileElementNames.TypeFactoryReturnedTypesIfSelector:
                    configurationFileElement = new TypeFactoryReturnedTypesIfSelector(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.TypeFactoryReturnedTypesDefaultSelector:
                    configurationFileElement = new TypeFactoryReturnedTypesSelector(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.TypeFactoryReturnedType:
                    configurationFileElement = new TypeFactoryReturnedType(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.SettingsRequestor:
                    configurationFileElement = new SettingsRequestorImplementationElement(xmlElement, parentConfigurationFileElement,
                        _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.StartupActions:
                    configurationFileElement = new StartupActionsElement(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.StartupAction:
                    configurationFileElement = new StartupActionElement(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;

                case ConfigurationFileElementNames.PluginsSetup:
                    configurationFileElement = new PluginsSetup(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.PluginSetup:
                    configurationFileElement = new PluginSetup(xmlElement, parentConfigurationFileElement);
                    break;

                case ConfigurationFileElementNames.PluginImplementation:
                    configurationFileElement = new PluginImplementationElement(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                    break;
            }

            if (configurationFileElement == null)
                throw new ConfigurationParseException(parentConfigurationFileElement, $"Invalid element '{xmlElement.Name}' under '{parentConfigurationFileElement.ElementName}'.");

            configurationFileElement.Initialize();

            return configurationFileElement;
        }

        #endregion
    }
}