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
using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class ConfigurationFileElementFactory : IConfigurationFileElementFactory
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [NotNull]
        private readonly IClassMemberValueInitializerHelper _classMemberValueInitializerHelper;


#if DEBUG
        //private static int _debugCounter;
#endif
        private IDeserializedFromStringValueInitializerHelper _deserializedFromStringValueInitializerHelper;

        private ITypeBasedSimpleSerializerAggregator _typeBasedSimpleSerializerAggregator;

        [NotNull]
        private readonly ITypeHelper _typeHelper;

        [NotNull]
        private readonly IValidateDiManagerCompatibility _validateDiManagerCompatibility = new ValidateDiManagerCompatibility();

        #endregion

        #region  Constructors

        public ConfigurationFileElementFactory([NotNull] IAssemblyLocator assemblyLocator)
        {
            _assemblyLocator = assemblyLocator;
            _typeHelper = IoCServiceFactoryAmbientContext.Context.CreateTypeHelper(_assemblyLocator);
            _classMemberValueInitializerHelper = IoCServiceFactoryAmbientContext.Context.CreateClassMemberValueInitializerHelper(_typeHelper);
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
#if DEBUG
            //++_debugCounter;
            //LogHelper.Context.Log.Debug($"_debugCounter={_debugCounter}");
#endif
            var serializerAggregator = GetTypeBasedSimpleSerializerAggregator(parentConfigurationFileElement);

            IDeserializedFromStringValueInitializerHelper getDeserializedFromStringValueInitializerHelper()
            {
                if (_deserializedFromStringValueInitializerHelper != null)
                    return _deserializedFromStringValueInitializerHelper;

                var deserializedFromStringValueInitializerHelper = IoCServiceFactoryAmbientContext.Context.CreateDeserializedFromStringValueInitializerHelper(serializerAggregator);

                if (_typeBasedSimpleSerializerAggregator != null && serializerAggregator == _typeBasedSimpleSerializerAggregator)
                    _deserializedFromStringValueInitializerHelper = deserializedFromStringValueInitializerHelper;

                return deserializedFromStringValueInitializerHelper;
            }

            IConfigurationFileElement configurationFileElement = null;

            if (parentConfigurationFileElement is IMethodSignatureElement methodSignatureElement)
            {
                configurationFileElement = new MethodSignatureParameterElement(xmlElement, methodSignatureElement, _typeHelper);
            }
            else if (parentConfigurationFileElement is IParameters)
            {
                configurationFileElement = new ParameterElement(CreateValueInitializerElement(xmlElement, parentConfigurationFileElement, 
                    getDeserializedFromStringValueInitializerHelper()));
            }
            else if (parentConfigurationFileElement is ISettingsElement)
            {
                configurationFileElement = new SettingElement(CreateValueInitializerElement(xmlElement, parentConfigurationFileElement, 
                        getDeserializedFromStringValueInitializerHelper()),
                    IoCServiceFactoryAmbientContext.Context.IdentifierValidator);
            }
            else if (parentConfigurationFileElement is IInjectedProperties)
            {
                configurationFileElement = new InjectedPropertyElement(CreateValueInitializerElement(xmlElement, parentConfigurationFileElement, 
                    getDeserializedFromStringValueInitializerHelper()));
            }
            else if (parentConfigurationFileElement is ICollectionValueElement ||
                     parentConfigurationFileElement is IValueInitializerElementDecorator valueInitializerElementDecorator &&
                     valueInitializerElementDecorator.DecoratedValueInitializerElement is ICollectionValueElement)
            {
                configurationFileElement = new CollectionItemValueElement(CreateValueInitializerElement(xmlElement, parentConfigurationFileElement, 
                    getDeserializedFromStringValueInitializerHelper()));
            }
            else if (parentConfigurationFileElement is IAutoGeneratedServicePropertyElement ||
                     parentConfigurationFileElement is IAutoGeneratedMemberReturnValuesSelectorElement ||
                     parentConfigurationFileElement is IAutoGeneratedMemberReturnValuesIfSelectorElement)
            {
                configurationFileElement = new ReturnValueElement(CreateValueInitializerElement(xmlElement, parentConfigurationFileElement, 
                    getDeserializedFromStringValueInitializerHelper()));
            }
            else if (parentConfigurationFileElement is IAutoGeneratedServiceElement autoGeneratedServiceElement)
            {
                switch (xmlElement.Name)
                {
                    case ConfigurationFileElementNames.AutoProperty:
                        configurationFileElement = new AutoGeneratedServicePropertyElement(xmlElement, autoGeneratedServiceElement, _typeHelper,
                            IoCServiceFactoryAmbientContext.Context.TypeMemberLookupHelper);
                        break;

                    case ConfigurationFileElementNames.AutoMethod:
                        configurationFileElement = new AutoGeneratedServiceMethodElement(xmlElement, autoGeneratedServiceElement, _typeHelper, IoCServiceFactoryAmbientContext.Context.TypeMemberLookupHelper);
                        break;
                }
            }
            else if (parentConfigurationFileElement is AutoGeneratedServiceMethodElement autoGeneratedServiceMethodElement)
            {
                switch (xmlElement.Name)
                {
                    case ConfigurationFileElementNames.MethodSignature:
                        configurationFileElement = new MethodSignatureElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.AutoGeneratedMemberReturnValuesIfSelector:
                        configurationFileElement = new AutoGeneratedMemberReturnValuesIfSelectorElement(xmlElement, autoGeneratedServiceMethodElement,
                            IoCServiceFactoryAmbientContext.Context.SettingValueInitializerHelper, getDeserializedFromStringValueInitializerHelper(),
                            _classMemberValueInitializerHelper);
                        break;

                    case ConfigurationFileElementNames.AutoGeneratedMemberReturnValuesDefaultSelector:
                        configurationFileElement = new AutoGeneratedMemberReturnValuesSelectorElement(xmlElement, autoGeneratedServiceMethodElement);
                        break;
                }
            }
            else
            {
                switch (xmlElement.Name)
                {
                    case ConfigurationFileElementNames.AppDataDir:
                        configurationFileElement = new ApplicationDataDirectory(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.Plugins:
                        configurationFileElement = new Plugins(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.Plugin:
                        configurationFileElement = new PluginElement(xmlElement, Helpers.ConvertTo<IPlugins>(parentConfigurationFileElement), IoCServiceFactoryAmbientContext.Context.IdentifierValidator);
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
                        configurationFileElement = new Assembly(xmlElement, parentConfigurationFileElement, _assemblyLocator, IoCServiceFactoryAmbientContext.Context.IdentifierValidator);
                        break;

                    case ConfigurationFileElementNames.TypeDefinitions:
                        configurationFileElement = new TypeDefinitionsElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.TypeDefinition:
                        if (parentConfigurationFileElement is ITypeDefinitionsElement)
                            configurationFileElement = new NamedTypeDefinitionElement(xmlElement, parentConfigurationFileElement, IoCServiceFactoryAmbientContext.Context.IdentifierValidator, _typeHelper);
                        else if (parentConfigurationFileElement is IGenericTypeParametersElement)
                            configurationFileElement = new TypeDefinitionElement(xmlElement, parentConfigurationFileElement, _typeHelper);

                        break;

                    case ConfigurationFileElementNames.GenericTypeParameters:
                        configurationFileElement = new GenericTypeParametersElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.ParameterSerializers:
                        configurationFileElement = new ParameterSerializers(xmlElement, parentConfigurationFileElement, _typeHelper, IoCServiceFactoryAmbientContext.Context.CreateInstanceFromTypeAndConstructorParameters);
                        break;

                    case ConfigurationFileElementNames.Serializers:
                        configurationFileElement = new ParameterSerializersCollection(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.ParameterSerializer:
                        configurationFileElement = new ParameterSerializer(xmlElement, parentConfigurationFileElement, _typeHelper, IoCServiceFactoryAmbientContext.Context.CreateInstanceFromTypeAndConstructorParameters);
                        break;

                    case ConfigurationFileElementNames.Parameters:
                        configurationFileElement = new Parameters(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.DiManagers:
                        configurationFileElement = new DiManagersElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.DiManager:
                        configurationFileElement = new DiManagerElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                            IoCServiceFactoryAmbientContext.Context.CreateInstanceFromTypeAndConstructorParameters, _validateDiManagerCompatibility);
                        break;

                    case ConfigurationFileElementNames.Settings:
                        if (parentConfigurationFileElement.OwningPluginElement == null)
                            configurationFileElement = new SettingsElement(xmlElement, parentConfigurationFileElement);
                        else
                            configurationFileElement = new PluginSettingsElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.WebApi:
                        configurationFileElement = new WebApi(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.ControllerAssemblies:
                        configurationFileElement = new WebApiControllerAssemblies(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.ControllerAssembly:
                        configurationFileElement = new WebApiControllerAssembly(xmlElement, parentConfigurationFileElement, _assemblyLocator);
                        break;

                    case ConfigurationFileElementNames.DependencyInjection:
                        configurationFileElement = new DependencyInjection(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.Modules:
                        configurationFileElement = new ModulesElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.Module:
                        configurationFileElement = new ModuleElement(xmlElement, parentConfigurationFileElement, _typeHelper, IoCServiceFactoryAmbientContext.Context.CreateInstanceFromTypeAndConstructorParameters);
                        break;

                    case ConfigurationFileElementNames.Services:
                        configurationFileElement = new Services(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.Service:
                        configurationFileElement = new ServiceElement(xmlElement, parentConfigurationFileElement, _typeHelper, IoCServiceFactoryAmbientContext.Context.ValidateServiceUsageInPlugin);
                        break;

                    case ConfigurationFileElementNames.ProxyService:
                        configurationFileElement = new ProxyServiceElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                            IoCServiceFactoryAmbientContext.Context.ValidateServiceUsageInPlugin);
                        break;
                    case ConfigurationFileElementNames.SelfBoundService:
                        configurationFileElement = new SelfBoundServiceElement(xmlElement, parentConfigurationFileElement, IoCServiceFactoryAmbientContext.Context.ImplementedTypeValidator,
                            IoCServiceFactoryAmbientContext.Context.InjectedPropertiesValidator, _typeHelper, 
                            IoCServiceFactoryAmbientContext.Context.ValidateServiceUsageInPlugin);
                        break;

                    case ConfigurationFileElementNames.Implementation:
                        configurationFileElement = new TypeBasedServiceImplementationElement(xmlElement, parentConfigurationFileElement,
                            IoCServiceFactoryAmbientContext.Context.ImplementedTypeValidator, IoCServiceFactoryAmbientContext.Context.InjectedPropertiesValidator,
                            _typeHelper);
                        break;

                    case ConfigurationFileElementNames.ServiceToProxy:
                        configurationFileElement = new ServiceToProxyImplementationElement(xmlElement, parentConfigurationFileElement,
                            _typeHelper);
                        break;

                    case ConfigurationFileElementNames.ValueImplementation:
                        if (parentConfigurationFileElement is IServiceElement serviceElement)
                            configurationFileElement = new ValueBasedServiceImplementationElement(xmlElement, serviceElement);
                        break;

                    case ConfigurationFileElementNames.InjectedProperties:
                        configurationFileElement = new InjectedProperties(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.AutoGeneratedServices:
                        configurationFileElement = new AutoGeneratedServicesElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.AutoService:
                        configurationFileElement = new AutoGeneratedServiceElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                            IoCServiceFactoryAmbientContext.Context.TypeMemberLookupHelper, IoCServiceFactoryAmbientContext.Context.ValidateServiceUsageInPlugin);
                        break;

                    case ConfigurationFileElementNames.AutoServiceCustom:
                        configurationFileElement = new CustomAutoGeneratedServiceElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                            IoCServiceFactoryAmbientContext.Context.TypeMemberLookupHelper, IoCServiceFactoryAmbientContext.Context.ValidateServiceUsageInPlugin);
                        break;

                    case ConfigurationFileElementNames.AutoServiceCodeGenerator:
                        configurationFileElement = new AutoServiceCodeGeneratorElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.SettingsRequestor:
                        configurationFileElement = new SettingsRequestorImplementationElement(xmlElement, parentConfigurationFileElement,
                            IoCServiceFactoryAmbientContext.Context.ImplementedTypeValidator, IoCServiceFactoryAmbientContext.Context.InjectedPropertiesValidator, _typeHelper);
                        break;

                    case ConfigurationFileElementNames.StartupActions:
                        configurationFileElement = new StartupActionsElement(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.StartupAction:
                        configurationFileElement = new StartupActionElement(xmlElement, parentConfigurationFileElement,
                            IoCServiceFactoryAmbientContext.Context.ImplementedTypeValidator, IoCServiceFactoryAmbientContext.Context.InjectedPropertiesValidator, _typeHelper);
                        break;

                    case ConfigurationFileElementNames.PluginsSetup:
                        configurationFileElement = new PluginsSetup(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.PluginSetup:
                        configurationFileElement = new PluginSetup(xmlElement, parentConfigurationFileElement);
                        break;

                    case ConfigurationFileElementNames.PluginImplementation:
                        configurationFileElement = new PluginImplementationElement(xmlElement, parentConfigurationFileElement,
                            IoCServiceFactoryAmbientContext.Context.ImplementedTypeValidator, IoCServiceFactoryAmbientContext.Context.InjectedPropertiesValidator, _typeHelper);
                        break;

                    case ConfigurationFileElementNames.Collection:
                        if (parentConfigurationFileElement is ICanHaveCollectionChildElement canHaveCollectionChildElement)
                            configurationFileElement = new ContextBasedCollectionValueElement(xmlElement, canHaveCollectionChildElement, _typeHelper, IoCServiceFactoryAmbientContext.Context.PluginAssemblyTypeUsageValidator);
                        break;

                    case ConfigurationFileElementNames.SettingValue:
                        configurationFileElement = new SettingValueElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                            IoCServiceFactoryAmbientContext.Context.SettingValueInitializerHelper);
                        break;

                    case ConfigurationFileElementNames.ValueObject:
                        configurationFileElement = new ValueInitializerElementDeserializedFromString(xmlElement, parentConfigurationFileElement,
                            _typeHelper, getDeserializedFromStringValueInitializerHelper());
                        break;
                    case ConfigurationFileElementNames.ClassMember:
                        configurationFileElement = new ClassMemberValueInitializerElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                            _classMemberValueInitializerHelper);
                        break;

                    case ConfigurationFileElementNames.ConstructedValue:
                        configurationFileElement = new ConstructedValueElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                            IoCServiceFactoryAmbientContext.Context.ImplementedTypeValidator, IoCServiceFactoryAmbientContext.Context.InjectedPropertiesValidator,
                            IoCServiceFactoryAmbientContext.Context.CreateInstanceFromTypeAndConstructorParameters);
                        break;

                    //case ConfigurationFileElementNames.ValueInjectedObject:
                    //    configurationFileElement =  new ValueInitializerElementResolvedFromDiContainer(xmlElement, parentConfigurationFileElement, _typeHelper);
                    //    break;

                    //case ConfigurationFileElementNames.AdditionalAssemblyReferences:
                    //    configurationFileElement = new ReferencedAssembliesElement(xmlElement, parentConfigurationFileElement);
                    //    break;

                    //case ConfigurationFileElementNames.AssemblyRef:
                    //    configurationFileElement = new ReferencedAssemblyElement(xmlElement, parentConfigurationFileElement);
                    //    break;
                }
            }


            if (configurationFileElement == null)
                throw new InvalidElementConfigurationParseException(xmlElement, parentConfigurationFileElement);

            if (parentConfigurationFileElement != null)
            {
                LogHelper.Context.Log.DebugFormat("Calling {0}.{1}(childElement) for child element '{2}' and parent '{3}'.",
                    typeof(IConfigurationFileElement).FullName,
                    nameof(IConfigurationFileElement.BeforeChildInitialize),
                    configurationFileElement.ElementName, parentConfigurationFileElement.ElementName);
                parentConfigurationFileElement.BeforeChildInitialize(configurationFileElement);
            }

            LogHelper.Context.Log.DebugFormat("Calling {0}.{1}() for element '{2}'.",
                typeof(IConfigurationFileElement).FullName,
                nameof(IConfigurationFileElement.Initialize),
                configurationFileElement.ElementName);
            parentConfigurationFileElement.BeforeChildInitialize(configurationFileElement);

            configurationFileElement.Initialize();

            return configurationFileElement;
        }

        #endregion

        #region Member Functions

        private IValueInitializerElement CreateValueInitializerElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parentConfigurationFileElement,
                                                                       [NotNull] IDeserializedFromStringValueInitializerHelper deserializedFromStringValueInitializerHelper)
        {
            switch (xmlElement.Name)
            {
                case ConfigurationFileElementNames.ValueByte:
                case ConfigurationFileElementNames.ValueInt16:
                case ConfigurationFileElementNames.ValueInt32:
                case ConfigurationFileElementNames.ValueInt64:
                case ConfigurationFileElementNames.ValueDouble:
                case ConfigurationFileElementNames.ValueString:
                case ConfigurationFileElementNames.ValueBoolean:
                case ConfigurationFileElementNames.ValueDateTime:
                case ConfigurationFileElementNames.ValueObject:
                    return new ValueInitializerElementDeserializedFromString(xmlElement, parentConfigurationFileElement,
                        _typeHelper, deserializedFromStringValueInitializerHelper);
                case ConfigurationFileElementNames.ValueInjectedObject:
                    return new ValueInitializerElementResolvedFromDiContainer(xmlElement, parentConfigurationFileElement, _typeHelper);
                case ConfigurationFileElementNames.ConstructedValue:
                    return new ConstructedValueElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                        IoCServiceFactoryAmbientContext.Context.ImplementedTypeValidator, IoCServiceFactoryAmbientContext.Context.InjectedPropertiesValidator,
                        IoCServiceFactoryAmbientContext.Context.CreateInstanceFromTypeAndConstructorParameters);
                case ConfigurationFileElementNames.SettingValue:
                    return new SettingValueElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                        IoCServiceFactoryAmbientContext.Context.SettingValueInitializerHelper);
                case ConfigurationFileElementNames.Collection:
                    if (parentConfigurationFileElement is IParameters || parentConfigurationFileElement is IInjectedProperties ||
                        parentConfigurationFileElement is ISettingsElement)
                        return new TypeBasedCollectionValueElement(xmlElement, parentConfigurationFileElement, _typeHelper,
                            IoCServiceFactoryAmbientContext.Context.PluginAssemblyTypeUsageValidator);
                    else if (parentConfigurationFileElement is ICanHaveCollectionChildElement canHaveCollectionChildElement)
                        return new ContextBasedCollectionValueElement(xmlElement, canHaveCollectionChildElement, _typeHelper, IoCServiceFactoryAmbientContext.Context.PluginAssemblyTypeUsageValidator);

                    throw new InvalidElementConfigurationParseException(xmlElement, parentConfigurationFileElement);

                case ConfigurationFileElementNames.ClassMember:
                    return new ClassMemberValueInitializerElement(xmlElement, parentConfigurationFileElement, _typeHelper, _classMemberValueInitializerHelper);

                case ConfigurationFileElementNames.ParameterValue:
                    return new ParameterValueInitializerElement(xmlElement, parentConfigurationFileElement, _typeHelper);
            }

            throw new InvalidElementConfigurationParseException(xmlElement, parentConfigurationFileElement);
        }


        private ITypeBasedSimpleSerializerAggregator GetTypeBasedSimpleSerializerAggregator([NotNull] IConfigurationFileElement parentConfigurationFileElement)
        {
            if (_typeBasedSimpleSerializerAggregator != null)
                return _typeBasedSimpleSerializerAggregator;

            var typeBasedSimpleSerializerAggregator = parentConfigurationFileElement.Configuration?.ParameterSerializers?.TypeBasedSimpleSerializerAggregator;

            if (typeBasedSimpleSerializerAggregator != null)
            {
                _typeBasedSimpleSerializerAggregator = typeBasedSimpleSerializerAggregator;
                return _typeBasedSimpleSerializerAggregator;
            }

            return TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator();
        }

        #endregion
    }
}