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
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public class ProxyServiceElement : ConfigurationFileElementAbstr, IServiceElement,
                                       ICanHaveChildElementsThatUsePluginTypeInNonPluginSection

    {
        #region Member Variables

        private readonly List<IServiceImplementationElement> _implementations = new List<IServiceImplementationElement>();

        [NotNull]
        private readonly ITypeHelper _typeHelper;

        [NotNull]
        private readonly ITypeMemberLookupHelper _typeMemberLookupHelper;

        [NotNull]
        private readonly IValidateServiceUsageInPlugin _validateServiceUsageInPlugin;

        #endregion

        #region  Constructors

        public ProxyServiceElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                   [NotNull] ITypeHelper typeHelper,
                                   [NotNull] IValidateServiceUsageInPlugin validateServiceUsageInPlugin,
                                   [NotNull] ITypeMemberLookupHelper typeMemberLookupHelper) : base(xmlElement, parent)
        {
            _validateServiceUsageInPlugin = validateServiceUsageInPlugin;
            _typeHelper = typeHelper;
            _typeMemberLookupHelper = typeMemberLookupHelper;
        }

        #endregion

        #region IServiceElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);
            if (child is IServiceToProxyImplementationElement serviceToProxyImplementation)
            {
                if (ServiceTypeInfo.Type == serviceToProxyImplementation.ValueTypeInfo.Type)
                    throw new ConfigurationParseException(serviceToProxyImplementation,
                        $"The types specified in elements '{ConfigurationFileElementNames.ProxyService}' and '{ConfigurationFileElementNames.ServiceToProxy}' cannot be the same. Both elements use specify the type '{ServiceTypeInfo.Type.GetTypeNameInCSharpClass()}'.", this);

                if (!ServiceTypeInfo.Type.IsAssignableFrom(serviceToProxyImplementation.ValueTypeInfo.Type))
                    throw new ConfigurationParseException(serviceToProxyImplementation,
                        string.Format("The type specified element '{0}' for service to proxy should be assignable from the type specified element '{1}' for proxy service.",
                            ConfigurationFileElementNames.ServiceToProxy,
                            ConfigurationFileElementNames.ProxyService), this);

                var pluginTypesUsedInProxyService = ServiceTypeInfo.GetUniquePluginTypes();
                var pluginTypesUsedInProxiedService = serviceToProxyImplementation.ValueTypeInfo.GetUniquePluginTypes();

                if (pluginTypesUsedInProxyService.Count == 0)
                {
                    if (pluginTypesUsedInProxiedService.Count > 0)
                        throw new ConfigurationParseException(serviceToProxyImplementation,
                            string.Format("Non plugin proxy service '{0}' cannot proxy a plugin service '{1}' which belongs to plugin '{2}'.",
                                ServiceTypeInfo.TypeCSharpFullName,
                                serviceToProxyImplementation.ValueTypeInfo,
                                pluginTypesUsedInProxiedService[0].Assembly.Plugin.Name),
                            this);
                }
                else
                {
                    if (pluginTypesUsedInProxiedService.Count == 0)
                        throw new ConfigurationParseException(serviceToProxyImplementation,
                            string.Format("Proxy service '{0}' belongs to plugin '{1}' and cannot proxy a non-plugin service '{1}'.",
                                ServiceTypeInfo.TypeCSharpFullName,
                                pluginTypesUsedInProxiedService[0].Assembly.Plugin.Name, serviceToProxyImplementation.ValueTypeInfo),
                            this);
                }

                _implementations.Add(serviceToProxyImplementation);
            }
        }

        public IEnumerable<IServiceImplementationElement> Implementations => _implementations;

        public override void Initialize()
        {
            base.Initialize();

            ServiceTypeInfo = _typeHelper.GetTypeInfo(this, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly, ConfigurationFileAttributeNames.TypeRef);
            RegisterIfNotRegistered = this.GetAttributeValue<bool>(ConfigurationFileAttributeNames.RegisterIfNotRegistered);

            _validateServiceUsageInPlugin.Validate(this, ServiceTypeInfo);
        }

        public bool RegisterIfNotRegistered { get; private set; }
        Type IServiceElement.ServiceType => ServiceTypeInfo.Type;
        public ITypeInfo ServiceTypeInfo { get; private set; }

        #endregion
    }
}