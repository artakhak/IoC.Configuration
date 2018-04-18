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
using OROptimizer.Serializer;

namespace IoC.Configuration.ConfigurationFile
{
    public class ParameterElement : ConfigurationFileElementAbstr, IParameterElement
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        [NotNull]
        private readonly ITypeBasedSimpleSerializerAggregator _typeBasedSimpleSerializerAggregator;

        #endregion

        #region  Constructors

        public ParameterElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                [NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator,
                                [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _typeBasedSimpleSerializerAggregator = typeBasedSimpleSerializerAggregator;
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IParameterElement Interface Implementation

        public object DeserializedValue { get; private set; }

        public override bool Enabled => base.Enabled && ValueType != null;

        public override void Initialize()
        {
            base.Initialize();

            Name = this.GetNameAttributeValue();
            ValueInstantiationType = ValueInstantiationType.DeserializeFromStringValue;

            if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueObject, StringComparison.OrdinalIgnoreCase) ||
                _xmlElement.Name.Equals(ConfigurationFileElementNames.ValueInjectedObject, StringComparison.OrdinalIgnoreCase))
            {
                ValueAsString = _xmlElement.GetAttribute(ConfigurationFileAttributeNames.Value).Trim();

                var assemblySetting = Helpers.GetAssemblySettingByAssemblyAlias(this, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

                if (assemblySetting.Enabled)
                    ValueType = Helpers.GetTypeInAssembly(_assemblyLocator, this, assemblySetting, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Type));
                else if (Parent?.Enabled ?? false)
                    throw new ConfigurationParseException(this, $"The referenced assembly with alias '{assemblySetting.Alias}' is disabled.");

                if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueInjectedObject, StringComparison.OrdinalIgnoreCase))
                    ValueInstantiationType = ValueInstantiationType.ResolveFromDiContext;
            }
            else
            {
                ValueAsString = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Value);

                if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueByte, StringComparison.OrdinalIgnoreCase))
                    ValueType = typeof(byte);
                else if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueInt16, StringComparison.OrdinalIgnoreCase))
                    ValueType = typeof(short);
                else if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueInt32, StringComparison.OrdinalIgnoreCase))
                    ValueType = typeof(int);
                else if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueInt64, StringComparison.OrdinalIgnoreCase))
                    ValueType = typeof(long);
                else if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueDouble, StringComparison.OrdinalIgnoreCase))
                    ValueType = typeof(double);
                else if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueBoolean, StringComparison.OrdinalIgnoreCase))
                    ValueType = typeof(bool);
                else if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueString, StringComparison.OrdinalIgnoreCase))
                    ValueType = typeof(string);
                else if (_xmlElement.Name.Equals(ConfigurationFileElementNames.ValueDateTime, StringComparison.OrdinalIgnoreCase))
                    ValueType = typeof(DateTime);
                else
                    throw new ConfigurationParseException(this, $"Unknown type element '{_xmlElement.Name}'.");
            }

            if (ValueType != null && ValueInstantiationType != ValueInstantiationType.ResolveFromDiContext)
            {
                if (!_typeBasedSimpleSerializerAggregator.TryDeserialize(ValueType, ValueAsString, out var deserializedValue))
                {
                    if (!_typeBasedSimpleSerializerAggregator.HasSerializerForType(ValueType))
                        throw new ConfigurationParseException(this, $"No serializer for type '{ValueType.FullName}' was registered in '{ConfigurationFileElementNames.ParameterSerializers}' element.");

                    throw new ConfigurationParseException(this, $"The parameter serializer '{_typeBasedSimpleSerializerAggregator.GetSerializerForType(ValueType).GetType().FullName}' failed to convert '{ValueAsString}' to type '{ValueType.FullName}'.");
                }

                DeserializedValue = deserializedValue;
            }
        }

        public string Name { get; private set; }
        public string ValueAsString { get; private set; }
        public ValueInstantiationType ValueInstantiationType { get; private set; }
        public Type ValueType { get; private set; }

        #endregion
    }
}