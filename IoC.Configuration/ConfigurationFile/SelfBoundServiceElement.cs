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
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class SelfBoundServiceElement : ServiceImplementationElement, ISelfBoundServiceElement
    {
        #region  Constructors

        public SelfBoundServiceElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                       [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, assemblyLocator)
        {
            Implementations = new IServiceImplementationElement[] {this};
        }

        #endregion

        #region ISelfBoundServiceElement Interface Implementation

        public IEnumerable<IServiceImplementationElement> Implementations { get; }

        public override void Initialize()
        {
            base.Initialize();

            if (Enabled)
            {
                if (OwningPluginElement == null)
                {
                    if (Assembly.OwningPluginElement != null)
                        throw new ConfigurationParseException(this, $"Type '{ServiceType.FullName}' is defined in assembly {Assembly} which belongs to plugin '{Assembly.OwningPluginElement.Name}'. The service should be defined under '{ConfigurationFileElementNames.Services}' element for plugin '{Assembly.OwningPluginElement.Name}'.");
                }
                else if (Assembly.OwningPluginElement != OwningPluginElement)
                {
                    throw new ConfigurationParseException(this, $"Type '{ServiceType.FullName}' is defined in assembly {Assembly} which does not be belong to plugin '{OwningPluginElement.Name}' that owns the service.");
                }

                RegisterIfNotRegistered = this.GetAttributeValue<bool>(ConfigurationFileAttributeNames.RegisterIfNotRegistered);
            }
        }

        public bool RegisterIfNotRegistered { get; private set; }

        public Type ServiceType => ImplementationType;

        #endregion
    }
}