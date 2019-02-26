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

using System.Xml;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class ValueInitializerElementDeserializedFromString : ValueInitializerElement, IDeserializedValue
    {
        #region Member Variables

        [NotNull]
        private readonly IDeserializedFromStringValueInitializerHelper _deserializedFromStringValueInitializerHelper;

        #endregion

        #region  Constructors

        /// <inheritdoc />
        public ValueInitializerElementDeserializedFromString([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                                             [NotNull] ITypeHelper typeHelper,
                                                             [NotNull] IDeserializedFromStringValueInitializerHelper deserializedFromStringValueInitializerHelper) : base(xmlElement, parent, typeHelper)
        {
            _deserializedFromStringValueInitializerHelper = deserializedFromStringValueInitializerHelper;
        }

        #endregion

        #region IDeserializedValue Interface Implementation

        /// <summary>
        ///     The value deserialized from <see cref="IDeserializedValue.ValueAsString" /> to type in property
        ///     <see cref="ITypedItem.ValueTypeInfo" />.Type.
        ///     Note, the value is de-serialized each time a call to this property is made, to keep the memory usage low.
        /// </summary>
        public object DeserializedValue => _deserializedFromStringValueInitializerHelper.GetDeserializedValue(this, ValueTypeInfo, ValueAsString);

        /// <summary>
        ///     Gets the value as string. Examples are "2", "true", etc.
        /// </summary>
        /// <value>
        ///     The value as string.
        /// </value>
        public string ValueAsString { get; private set; }

        #endregion

        #region Member Functions

        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            return _deserializedFromStringValueInitializerHelper.GenerateValueCSharp(this, ValueTypeInfo, ValueAsString, dynamicAssemblyBuilder);
        }

        /// <summary>
        ///     Generates the value. Use this value only at early stages of loading the configuration,
        ///     when the DI container is not yet initialized.
        /// </summary>
        /// <returns></returns>
        public override object GenerateValue()
        {
            return DeserializedValue;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (ValueTypeInfo == null)
                throw new ConfigurationParseException(this, "Type information was not initialized.");

            ValueAsString = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Value);

            _deserializedFromStringValueInitializerHelper.GetDeserializedValue(this, ValueTypeInfo, ValueAsString);
        }

        public override bool IsResolvedFromDiContainer => false;

        #endregion
    }
}