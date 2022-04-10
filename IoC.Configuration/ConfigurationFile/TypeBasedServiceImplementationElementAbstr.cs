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

using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class TypeBasedServiceImplementationElementAbstr : ServiceImplementationElementAbstr, ITypeBasedServiceImplementationElement
    {
        #region Member Variables

        private ITypeInfo _implementationTypeInfo;

        [NotNull]
        private readonly IImplementedTypeValidator _implementedTypeValidator;

        [NotNull]
        private readonly IInjectedPropertiesValidator _injectedPropertiesValidator;

        [NotNull]
        private readonly ITypeHelper _typeHelper;

        #endregion

        #region  Constructors

        public TypeBasedServiceImplementationElementAbstr([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                                          [NotNull] IImplementedTypeValidator implementedTypeValidator,
                                                          [NotNull] IInjectedPropertiesValidator injectedPropertiesValidator,
                                                          [NotNull] ITypeHelper typeHelper) : base(xmlElement, parent)
        {
            _implementedTypeValidator = implementedTypeValidator;
            _injectedPropertiesValidator = injectedPropertiesValidator;
            _typeHelper = typeHelper;
        }

        #endregion

        #region ITypeBasedServiceImplementationElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters parameters)
                Parameters = parameters;
            else if (child is IInjectedProperties injectedProperties)
                InjectedProperties = injectedProperties;
        }

        public override void Initialize()
        {
            base.Initialize();

            _implementationTypeInfo = _typeHelper.GetTypeInfo(this, ImplementationTypeAttributeName, ImplementationTypeAssemblyAttributeName, ImplementationTypeRefAttributeName);
            ValidateImplementationType();
        }

        public IInjectedProperties InjectedProperties { get; private set; }

        public IParameters Parameters { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            // If no constructor parameter was specified, we will be injecting by type.
            if (Parameters != null)
                if (!GlobalsCoreAmbientContext.Context.CheckTypeConstructorExistence(_implementationTypeInfo.Type,
                    Parameters.AllParameters.Select(x => x.ValueTypeInfo.Type).ToArray(),
                    out var constructorInfo, out var errorMessage))
                    throw new ConfigurationParseException(this, errorMessage);

            if (InjectedProperties != null)
                _injectedPropertiesValidator.ValidateInjectedProperties(this, _implementationTypeInfo.Type,
                    InjectedProperties.AllProperties, out var injectedPropertiesInfo);
        }

        public override ITypeInfo ValueTypeInfo => _implementationTypeInfo;

        #endregion

        #region Current Type Interface

        protected virtual string ImplementationTypeAssemblyAttributeName => ConfigurationFileAttributeNames.Assembly;

        protected virtual string ImplementationTypeAttributeName => ConfigurationFileAttributeNames.Type;
        protected virtual string ImplementationTypeRefAttributeName => ConfigurationFileAttributeNames.TypeRef;

        protected virtual void ValidateImplementationType()
        {
            _implementedTypeValidator.ValidateImplementationType(this, ValueTypeInfo);
        }

        #endregion
    }
}