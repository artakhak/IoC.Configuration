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
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public abstract class NamedValue : INamedValue
    {
        #region Member Variables

        [NotNull]
        private readonly INamedValueElement _namedValueElement;

        #endregion

        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NamedValue" /> class.
        /// </summary>
        /// <param name="namedValueElement">The named value element.</param>
        protected NamedValue([NotNull] INamedValueElement namedValueElement)
        {
            _namedValueElement = namedValueElement;
        }

        #endregion

        #region INamedValue Interface Implementation

        public string GenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            return _namedValueElement.GenerateValueCSharp(dynamicAssemblyBuilder);
        }

        /// <summary>
        ///     Generates the value using reflection. Use this value only at early stages of loading the configuration,
        ///     when the DI container is not yet initialized.
        /// </summary>
        /// <returns></returns>
        public object GenerateValue()
        {
            return _namedValueElement.GenerateValue();
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name => _namedValueElement.Name;

        /// <summary>
        ///     Gets the value as string. Examples are "2", "true", etc.
        /// </summary>
        /// <value>
        ///     The value as string.
        /// </value>
        [Obsolete("Will be removed after 5/31/2019")]
        string INamedValue.ValueAsString
        {
            get
            {
                if (_namedValueElement is IDeserializedValue deserializedValue)
                    return deserializedValue.ValueAsString;

                return string.Empty;
            }
        }

        /// <summary>
        ///     Can be null only if the parameter is declared with either 'object' or injectedObject elements, and the object type
        ///     referenced is in a disabled assembly.
        /// </summary>
        public ITypeInfo ValueTypeInfo => _namedValueElement.ValueTypeInfo;

        #endregion

        #region Member Functions

        public bool IsResolvedFromDiContainer => _namedValueElement.IsResolvedFromDiContainer;

        /// <summary>
        ///     Normally for injectedObject element this value is <see cref="ValueInstantiationType.ResolveFromDiContext" />, for
        ///     other elements (i.e., int16, int32, etc), the overridden value will be
        ///     <see cref="ValueInstantiationType.DeserializeFromStringValue" />
        /// </summary>
        [Obsolete("Will be removed after 5/31/2019")]
        public ValueInstantiationType ValueInstantiationType => _namedValueElement.ValueInstantiationType;

        /// <summary>
        ///     Can be null only if the parameter is declared with either 'object' or injectedObject elements, and the object type
        ///     referenced is in a disabled assembly.
        /// </summary>
        public Type ValueType => _namedValueElement.ValueTypeInfo.Type;

        #endregion
    }
}