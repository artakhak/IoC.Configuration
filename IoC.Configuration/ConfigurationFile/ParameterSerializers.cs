using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class ParameterSerializers : ConfigurationFileElementAbstr, IParameterSerializers
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [NotNull]
        private static readonly Dictionary<Type, ITypeBasedSimpleSerializer> _defaultSerializersThatCannotBeOverridden = new Dictionary<Type, ITypeBasedSimpleSerializer>();

        [CanBeNull]
        private IParameters _parameters;

        [CanBeNull]
        private IParameterSerializersCollection _parameterSerializersCollection;

        [NotNull]
        private Type _serializerAggregatorType;

        #endregion

        #region  Constructors

        static ParameterSerializers()
        {
            var defaultSerializer = OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator();

            Action<Type> addDefaultSerializer = serializedType =>
            {
                if (_defaultSerializersThatCannotBeOverridden.ContainsKey(serializedType))
                {
                    LogHelper.Context.Log.WarnFormat("A default serializer for type '{0}' was already added.", serializedType.FullName);
                    return;
                }

                _defaultSerializersThatCannotBeOverridden[serializedType] = defaultSerializer.GetSerializerForType(serializedType);
            };

            addDefaultSerializer(typeof(bool));
            addDefaultSerializer(typeof(byte));
            addDefaultSerializer(typeof(string));
            addDefaultSerializer(typeof(DateTime));
            addDefaultSerializer(typeof(short));
            addDefaultSerializer(typeof(int));
            addDefaultSerializer(typeof(long));
            addDefaultSerializer(typeof(double));
        }

        public ParameterSerializers([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                    [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IParameterSerializers Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters)
                _parameters = (IParameters) child;
            else if (child is IParameterSerializersCollection)
                _parameterSerializersCollection = (IParameterSerializersCollection) child;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.Assembly) && _xmlElement.HasAttribute(ConfigurationFileAttributeNames.SerializerAggregatorType))
            {
                var serializerAggregatorAssembly = Helpers.GetAssemblySettingByAssemblyAlias(this,
                    this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

                if (!serializerAggregatorAssembly.Enabled)
                    throw new ConfigurationParseException(this, $"Assembly '{serializerAggregatorAssembly.Alias}' used in element '{ConfigurationFileElementNames.ParameterSerializers}' is disabled.");

                _serializerAggregatorType = Helpers.GetTypeInAssembly(_assemblyLocator, this, serializerAggregatorAssembly, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.SerializerAggregatorType));
            }
            else
            {
                string presentAttributeName = null;
                string missingAttributeName = null;

                if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.Assembly))
                {
                    presentAttributeName = ConfigurationFileAttributeNames.Assembly;
                    missingAttributeName = ConfigurationFileAttributeNames.SerializerAggregatorType;
                }
                else if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.SerializerAggregatorType))
                {
                    presentAttributeName = ConfigurationFileAttributeNames.SerializerAggregatorType;
                    missingAttributeName = ConfigurationFileAttributeNames.Assembly;
                }

                if (presentAttributeName != null)
                    throw new ConfigurationParseException(this, $" Attribute '{presentAttributeName}' is present while attribute '{missingAttributeName}' is missing. Either both '{presentAttributeName}' and '{missingAttributeName}' attributes should be present, or both should be omitted.");

                // Use default aggregator.
                _serializerAggregatorType = OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator().GetType();

                LogHelper.Context.Log.InfoFormat("Since both attributes '{0}' and '{1}' are missing, falling back to default aggregator '{2}'.",
                    ConfigurationFileAttributeNames.SerializerAggregatorType, ConfigurationFileAttributeNames.Assembly, _serializerAggregatorType.FullName);
            }
        }

        [NotNull]
        public ITypeBasedSimpleSerializerAggregator TypeBasedSimpleSerializerAggregator { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (!GlobalsCoreAmbientContext.Context.TryCreateInstanceFromType(typeof(ITypeBasedSimpleSerializerAggregator), _serializerAggregatorType, _parameters == null ? new ParameterInfo[0] : _parameters.GetParameterValues(), out var serializerAggregatorObject, out var errorMessage))
                throw new ConfigurationParseException(this, errorMessage);

            TypeBasedSimpleSerializerAggregator = (ITypeBasedSimpleSerializerAggregator) serializerAggregatorObject;

            if (_parameterSerializersCollection != null)
                foreach (var parameterSerializer in _parameterSerializersCollection.AllSerializers)
                {
                    if (parameterSerializer.Serializer == null)
                    {
                        LogHelper.Context.Log.WarnFormat("The value of '{0}.{1}' should not be null.", typeof(IParameterSerializer).FullName, nameof(IParameterSerializer.Serializer));
                        continue;
                    }

                    if (_defaultSerializersThatCannotBeOverridden.TryGetValue(parameterSerializer.Serializer.SerializedType, out var defaultParameterSerializer) &&
                        defaultParameterSerializer.GetType() != parameterSerializer.Serializer.GetType())
                    {
                        var errorMessage2 = new StringBuilder();

                        errorMessage2.AppendLine($"Parameter serializer for type '{parameterSerializer.Serializer.SerializedType.FullName}' is pre-set to '{defaultParameterSerializer.GetType().FullName}', and it cannot be replaced with a different serializer.");
                        errorMessage2.AppendLine("Here are all the types, with pre-set serializers, for which the serializers cannot be replaced.");
                        foreach (var keyValuePair in _defaultSerializersThatCannotBeOverridden)
                            errorMessage2.AppendLine($"Type: '{keyValuePair.Key.FullName}', Preset serializer: '{keyValuePair.Value.GetType().FullName}'.");

                        throw new ConfigurationParseException(parameterSerializer, errorMessage2.ToString());
                    }

                    TypeBasedSimpleSerializerAggregator.Register(parameterSerializer.Serializer);
                }


            var defaultSerializer = OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator();

            if (defaultSerializer != TypeBasedSimpleSerializerAggregator)
                foreach (var serializer in defaultSerializer.GetRegisteredSerializers())
                    if (!TypeBasedSimpleSerializerAggregator.HasSerializerForType(serializer.SerializedType))
                    {
                        if (!_defaultSerializersThatCannotBeOverridden.ContainsKey(serializer.SerializedType))
                            LogHelper.Context.Log.InfoFormat("No '{0}' element for type '{1}' found. Default serializer '{2}' for this type will be used.",
                                ConfigurationFileElementNames.ParameterSerializer, serializer.SerializedType.FullName, serializer.GetType());

                        TypeBasedSimpleSerializerAggregator.Register(serializer);
                    }
        }

        #endregion
    }
}