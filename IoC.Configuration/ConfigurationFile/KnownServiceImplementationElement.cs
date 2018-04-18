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
using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class KnownServiceImplementationElement : ServiceImplementationElementAbstr, IServiceElement
    {
        #region Member Variables

        #endregion

        #region  Constructors

        public KnownServiceImplementationElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                                 [NotNull] Type implementedServiceType,
                                                 [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, assemblyLocator)
        {
            ServiceType = implementedServiceType;
            Implementations = new IServiceImplementationElement[] {this};
        }

        #endregion

        #region IServiceElement Interface Implementation

        public IEnumerable<IServiceImplementationElement> Implementations { get; }

        public override void Initialize()
        {
            base.Initialize();

            if (Enabled)
                if (!ServiceType.IsAssignableFrom(ImplementationType))
                    throw new ConfigurationParseException(this, $"Class '{ImplementationType.FullName}' does not implement interface '{ServiceType.FullName}'.");
        }

        public bool RegisterIfNotRegistered => false;

        [NotNull]
        public Type ServiceType { get; }

        #endregion

        #region Member Functions

        public override DiResolutionScope ResolutionScope => DiResolutionScope.Singleton;

        #endregion
    }
}