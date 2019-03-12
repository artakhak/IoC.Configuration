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
        private readonly ICreateInstanceFromTypeAndConstructorParameters _createInstanceFromTypeAndConstructorParameters;

        [NotNull]
        private static readonly Dictionary<Type, ITypeBasedSimpleSerializer> _defaultSerializersThatCannotBeOverridden = new Dictionary<Type, ITypeBasedSimpleSerializer>();

        [CanBeNull]
        private IParameters _parameters;

        [CanBeNull]
        private IParameterSerializersCollection _parameterSerializersCollection;

        [NotNull]
        private Type _serializerAggregatorType;

        [NotNull]
        private readonly ITypeHelper _typeHelper;

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
                                    [NotNull] ITypeHelper typeHelper,
                                    [NotNull] ICreateInstanceFromTypeAndConstructorParameters createInstanceFromTypeAndConstructorParameters) : base(xmlElement, parent)
        {
            _typeHelper = typeHelper;
            _createInstanceFromTypeAndConstructorParameters = createInstanceFromTypeAndConstructorParameters;
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

            if (_xmlElement.HasAttribute(ConfigurationFileAttributeNames.SerializerAggregatorType) ||
                _xmlElement.HasAttribute(ConfigurationFileAttributeNames.Assembly) ||
                _xmlElement.HasAttribute(ConfigurationFileAttributeNames.SerializerAggregatorTypeRef))
            {
                _serializerAggregatorType = _typeHelper.GetTypeInfo(this, ConfigurationFileAttributeNames.SerializerAggregatorType,
                    ConfigurationFileAttributeNames.Assembly,
                    ConfigurationFileAttributeNames.SerializerAggregatorTypeRef).Type;
            }
            else
            {
                _serializerAggregatorType =
                    OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator().GetType();

                LogHelper.Context.Log.InfoFormat("Since both attributes '{0}' and '{1}' are missing, falling back to default aggregator '{2}'.",
                    ConfigurationFileAttributeNames.SerializerAggregatorType, ConfigurationFileAttributeNames.Assembly, _serializerAggregatorType.FullName);
            }
        }

        [NotNull]
        public ITypeBasedSimpleSerializerAggregator TypeBasedSimpleSerializerAggregator { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            var serializerAggregatorObject = _createInstanceFromTypeAndConstructorParameters.CreateInstance(this,
                typeof(ITypeBasedSimpleSerializerAggregator), _serializerAggregatorType, _parameters?.AllParameters ?? new IParameterElement[0]);

            TypeBasedSimpleSerializerAggregator = (ITypeBasedSimpleSerializerAggregator) serializerAggregatorObject;

            if (_parameterSerializersCollection != null)
            {
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
            }

            var defaultSerializer = OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator();

            if (defaultSerializer != TypeBasedSimpleSerializerAggregator)
            {
                foreach (var serializer in defaultSerializer.GetRegisteredSerializers())
                {
                    if (!TypeBasedSimpleSerializerAggregator.HasSerializerForType(serializer.SerializedType))
                    {
                        if (!_defaultSerializersThatCannotBeOverridden.ContainsKey(serializer.SerializedType))
                            LogHelper.Context.Log.InfoFormat("No '{0}' element for type '{1}' found. Default serializer '{2}' for this type will be used.",
                                ConfigurationFileElementNames.ParameterSerializer, serializer.SerializedType.FullName, serializer.GetType());

                        TypeBasedSimpleSerializerAggregator.Register(serializer);
                    }
                }
            }

            LogSerializersData();
        }

        #endregion

        #region Member Functions

        private void LogSerializersData()
        {
            if (!LogHelper.Context.Log.IsInfoEnabled)
                return;

            var serializersLog = new StringBuilder();

            serializersLog.AppendLine("Registered type serializers:");

            foreach (var serializer in TypeBasedSimpleSerializerAggregator.GetRegisteredSerializers())
                serializersLog.AppendLine($"Serializer for type '{serializer.SerializedType.GetTypeNameInCSharpClass()}' is '{serializer.GetType().GetTypeNameInCSharpClass()}'.");

            LogHelper.Context.Log.Info(serializersLog.ToString());
        }

        #endregion
    }
}