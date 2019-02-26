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
using IoC.Configuration.DiContainerBuilder;
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.DynamicCode;
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class DeserializedFromStringValueInitializerHelper : IDeserializedFromStringValueInitializerHelper
    {
        #region Member Variables

        [NotNull]
        private readonly ITypeBasedSimpleSerializerAggregator _typeBasedSimpleSerializerAggregator;

        #endregion

        #region  Constructors

        public DeserializedFromStringValueInitializerHelper([NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            _typeBasedSimpleSerializerAggregator = typeBasedSimpleSerializerAggregator;
        }

        #endregion

        #region IDeserializedFromStringValueInitializerHelper Interface Implementation

        public string GenerateValueCSharp(IConfigurationFileElement requestingConfigurationFileElement, ITypeInfo valueTypeInfo, string valueAsString, IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            var deserializer = _typeBasedSimpleSerializerAggregator.GetSerializerForType(valueTypeInfo.Type);

            if (deserializer is IValueToCSharpCodeConverter valueToCSharpCodeConverter && deserializer.TryDeserialize(valueAsString, out var deserializedvalue))
                return valueToCSharpCodeConverter.GenerateCSharpCode(deserializedvalue);

#pragma warning disable CS0612, CS0618
            return $"{typeof(DiContainerBuilderConfiguration).FullName}.{nameof(DiContainerBuilderConfiguration.SerializerAggregatorStatic)}.{nameof(DiContainerBuilderConfiguration.SerializerAggregatorStatic.Deserialize)}<{valueTypeInfo.TypeCSharpFullName}>(@\"{valueAsString}\")";
#pragma warning restore CS0612, CS0618
        }

        public object GetDeserializedValue(IConfigurationFileElement requestingConfigurationFileElement, ITypeInfo valueTypeInfo, string valueAsString)
        {
            if (string.IsNullOrEmpty(valueAsString))
                throw new ConfigurationParseException(requestingConfigurationFileElement, "The value to de-serialize cannot be empty.");

            var deserializer = _typeBasedSimpleSerializerAggregator.GetSerializerForType(valueTypeInfo.Type);
            if (deserializer == null)
                throw new ConfigurationParseException(requestingConfigurationFileElement,
                    $"No serializer is registered for type '{valueTypeInfo.TypeCSharpFullName}' in section '{ConfigurationFileElementNames.RootElement}/{ConfigurationFileElementNames.ParameterSerializers}'. To fix an issue specify a serializer of type '{typeof(ITypeBasedSimpleSerializer).GetTypeNameInCSharpClass()}' in this section for the type.");

            if (!deserializer.TryDeserialize(valueAsString, out var deserializedValue))
                throw new ConfigurationParseException(requestingConfigurationFileElement, $"The parameter serializer '{deserializer.GetType().GetTypeNameInCSharpClass()}' failed to convert text '{valueAsString}' to a value of type '{valueTypeInfo.TypeCSharpFullName}'.");

            if (deserializer is IValueToCSharpCodeConverter valueToCSharpCodeConverter)
            {
                var cSharpCode = valueToCSharpCodeConverter.GenerateCSharpCode(deserializedValue);

                if (string.IsNullOrWhiteSpace(cSharpCode))
                {
                    throw new ConfigurationParseException(requestingConfigurationFileElement,
                        string.Format(
                            "Error in type serializer '{0}' for type '{1}'. The call to '{2}(\"{3}\")' returned a null or an empty string.",
                            deserializer.GetType().GetTypeNameInCSharpClass(),
                            valueTypeInfo.TypeCSharpFullName,
                            nameof(IValueToCSharpCodeConverter.GenerateCSharpCode),
                            valueAsString));
                }

                // TODO: In diagnostics mode only get the value from C# code and make sure it is equal to deserializedValue.
            }
            else
            {
                LogHelper.Context.Log.WarnFormat("The serializer '{0}' for type '{1}' does not implement interface '{2}'. This is OK, however, code will be slightly faster if type serializers implement this interface.",
                    deserializer.GetType().GetTypeNameInCSharpClass(),
                    valueTypeInfo.TypeCSharpFullName,
                    typeof(ITypeBasedSimpleSerializer).GetTypeNameInCSharpClass());
            }

            return deserializedValue;
        }

        #endregion

        #region Nested Types

        private class ConvertCSharpStringToValueTypeInfo
        {
            #region  Constructors

            public ConvertCSharpStringToValueTypeInfo([CanBeNull] Type convertCSharpStringToValueType)
            {
                ConvertCSharpStringToValueType = convertCSharpStringToValueType;
            }

            #endregion

            #region Member Functions

            [CanBeNull]
            public Type ConvertCSharpStringToValueType { get; }

            #endregion
        }

        #endregion
    }
}