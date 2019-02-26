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

namespace IoC.Configuration.ConfigurationFile
{
    public class ParameterElement : ValueInitializerElementDecorator, IParameterElement
    {
        #region Member Variables

        [NotNull]
        private readonly IValueInitializerElement _decoratedValueInitializerElement;

        #endregion

        #region  Constructors

        public ParameterElement([NotNull] IValueInitializerElement decoratedValueInitializerElement) : base(decoratedValueInitializerElement)
        {
            _decoratedValueInitializerElement = decoratedValueInitializerElement;
        }

        #endregion

        #region IParameterElement Interface Implementation

        [Obsolete("Will be removed after 5/31/2019")]
        object IParameterElement.DeserializedValue
        {
            get
            {
                if (_decoratedValueInitializerElement is IDeserializedValue deserializedValue)
                    return deserializedValue.DeserializedValue;

                return null;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            Name = this.GetNameAttributeValue();
        }

        public string Name { get; private set; }

        [Obsolete("Will be removed after 5/31/2019")]
        ValueInstantiationType INamedValueElement.ValueInstantiationType
        {
            get
            {
                if (IsResolvedFromDiContainer)
                    return ValueInstantiationType.ResolveFromDiContext;

                if (_decoratedValueInitializerElement is INamedValueElement namedValueElement)
                    return namedValueElement.ValueInstantiationType;

                if (_decoratedValueInitializerElement is IDeserializedValue deserializedValue)
                    return ValueInstantiationType.DeserializeFromStringValue;

                return ValueInstantiationType.Other;
            }
        }

        #endregion
    }
}