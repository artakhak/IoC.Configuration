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
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class DeserializedFromStringValueInitializer : IValueInitializer, IDeserializedValue
    {
        #region Member Variables

        [NotNull]
        private readonly IConfigurationFileElement _configurationFileElement;

        [NotNull]
        private readonly IDeserializedFromStringValueInitializerHelper _deserializedFromStringValueInitializerHelper;

        #endregion

        #region  Constructors

        public DeserializedFromStringValueInitializer([NotNull] IConfigurationFileElement configurationFileElement,
                                                      [NotNull] ITypeInfo valueTypeInfo,
                                                      [NotNull] string valueAsString,
                                                      [NotNull] IDeserializedFromStringValueInitializerHelper deserializedFromStringValueInitializerHelper)
        {
            _configurationFileElement = configurationFileElement;
            ValueTypeInfo = valueTypeInfo;
            ValueAsString = valueAsString;
            _deserializedFromStringValueInitializerHelper = deserializedFromStringValueInitializerHelper;
        }

        #endregion

        #region IDeserializedValue Interface Implementation

        /// <summary>
        ///     The value de-serialized from <see cref="IDeserializedValue.ValueAsString" /> to type in property
        ///     <see cref="ITypedItem.ValueTypeInfo" />.Type.
        ///     Note, the value is de-serialized each time a call to this property is made, to keep the memory usage low.
        /// </summary>
        public object DeserializedValue =>
            _deserializedFromStringValueInitializerHelper.GetDeserializedValue(_configurationFileElement, ValueTypeInfo, ValueAsString);

        /// <summary>
        ///     Gets the value as string. Examples are "2", "true", etc.
        /// </summary>
        /// <value>
        ///     The value as string.
        /// </value>
        [NotNull]
        public string ValueAsString { get; }

        #endregion

        #region IValueInitializer Interface Implementation

        /// <summary>
        ///     Generates a code that returns an instance of a value of type specified by property <see cref="ValueType" />.
        /// </summary>
        /// <param name="dynamicAssemblyBuilder">The dynamic assembly builder.</param>
        /// <returns></returns>
        public string GenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            return _deserializedFromStringValueInitializerHelper.GenerateValueCSharp(_configurationFileElement, ValueTypeInfo, ValueAsString, dynamicAssemblyBuilder);
        }

        [NotNull]
        public ITypeInfo ValueTypeInfo { get; }

        #endregion
    }
}