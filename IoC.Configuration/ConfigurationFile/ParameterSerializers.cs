// This software is part of the IoC.Configuration library
// Copyright � 2018 IoC.Configuration Contributors
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

using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;
using System;
using System.Text;
using System.Xml;

namespace IoC.Configuration.ConfigurationFile
{
    public class ParameterSerializers : ConfigurationFileElementAbstr, IParameterSerializers
    {
        #region Member Variables

        [NotNull]
        private readonly ICreateInstanceFromTypeAndConstructorParameters _createInstanceFromTypeAndConstructorParameters;
       
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

        public ITypeBasedSimpleSerializerAggregator TypeBasedSimpleSerializerAggregator { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            var defaultSerializer = OROptimizer.Serializer.TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator();

            var serializerAggregatorObject = _createInstanceFromTypeAndConstructorParameters.CreateInstance(this,
                typeof(ITypeBasedSimpleSerializerAggregator), _serializerAggregatorType, _parameters?.AllParameters ?? Array.Empty<IParameterElement>());

            TypeBasedSimpleSerializerAggregator = (ITypeBasedSimpleSerializerAggregator)serializerAggregatorObject;

            if (_parameterSerializersCollection != null)
            {
                foreach (var parameterSerializer in _parameterSerializersCollection.AllSerializers)
                {
                    if (parameterSerializer.Serializer == null)
                    {
                        LogHelper.Context.Log.WarnFormat("The value of '{0}.{1}' should not be null.", typeof(IParameterSerializer).FullName, nameof(IParameterSerializer.Serializer));
                        continue;
                    }

                    var defaultSerializerForType = defaultSerializer.GetSerializerForType(parameterSerializer.Serializer.SerializedType);
                    if (defaultSerializerForType != null)
                    {
                        LogHelper.Context.Log.InfoFormat("Replacing default serializer for type '{0}' with a serializer '{1}'. The default serializer was '{2}'.",
                            parameterSerializer.Serializer.SerializedType.GetTypeNameInCSharpClass(),
                            parameterSerializer.Serializer.GetType().GetTypeNameInCSharpClass(),
                            defaultSerializerForType.GetType().GetTypeNameInCSharpClass());

                        TypeBasedSimpleSerializerAggregator.UnRegister(parameterSerializer.Serializer.SerializedType);
                    }

                    TypeBasedSimpleSerializerAggregator.Register(parameterSerializer.Serializer);
                }
            }

            foreach (var serializer in defaultSerializer.GetRegisteredSerializers())
            {
                if (!TypeBasedSimpleSerializerAggregator.HasSerializerForType(serializer.SerializedType))
                    TypeBasedSimpleSerializerAggregator.Register(serializer);
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