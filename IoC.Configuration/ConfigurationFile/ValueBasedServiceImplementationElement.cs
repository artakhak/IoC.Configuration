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
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ValueBasedServiceImplementationElement : ServiceImplementationElementAbstr, IValueBasedServiceImplementationElement, ICanHaveCollectionChildElement
    {
        #region Member Variables

        private DiResolutionScope _resolutionScope;

        #endregion

        #region  Constructors

        public ValueBasedServiceImplementationElement([NotNull] XmlElement xmlElement, [NotNull] IServiceElement parentServiceElement) : base(xmlElement, parentServiceElement)
        {
            ServiceElement = parentServiceElement;
        }

        #endregion

        #region ICanHaveCollectionChildElement Interface Implementation

        ITypeInfo ICanHaveCollectionChildElement.ExpectedChildTypeInfo => ServiceElement.ServiceTypeInfo;

        #endregion

        #region IValueBasedServiceImplementationElement Interface Implementation

        public override DiResolutionScope ResolutionScope => _resolutionScope;

        [NotNull]
        public IServiceElement ServiceElement { get; }

        public override void ValidateAfterChildrenAdded()
        {
            // We need to get the value _valueInitializerElement before calling base.ValidateAfterChildrenAdded(),
            // since the base class calls Enabled which uses  ValueTypeInfo, which is overridden in this class.
            _resolutionScope = this.GetAttributeEnumValue<DiResolutionScope>(ConfigurationFileAttributeNames.Scope);
            if (Children.Count > 0)
                ValueInitializerElement = Children[0] as IValueInitializerElement;

            if (ValueInitializerElement == null)
                throw new ConfigurationParseException(this, "Value is missing.");

            base.ValidateAfterChildrenAdded();
        }

        public IValueInitializerElement ValueInitializerElement { get; private set; }

        public override ITypeInfo ValueTypeInfo => ValueInitializerElement.ValueTypeInfo;

        #endregion
    }
}