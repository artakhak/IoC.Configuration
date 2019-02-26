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
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class Services : ConfigurationFileElementAbstr, IServices
    {
        #region Member Variables

        [NotNull]
        private readonly LinkedList<IServiceElement> _allServices = new LinkedList<IServiceElement>();

        [NotNull]
        private readonly Dictionary<Type, IServiceElement> _serviceTypeToServiceMap = new Dictionary<Type, IServiceElement>();

        #endregion

        #region  Constructors

        public Services([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IServices Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IServiceElement serviceElement)
            {
                if (_serviceTypeToServiceMap.ContainsKey(serviceElement.ServiceTypeInfo.Type))
                    throw new ConfigurationParseException(child, $"Multiple occurrences of service with the value of attribute '{ConfigurationFileAttributeNames.Type}' equal to '{serviceElement.ServiceTypeInfo.TypeCSharpFullName}'.", this);

                if (serviceElement.ServiceTypeInfo.Type == typeof(ISettingsRequestor))
                    ThrowOnProhibitedServiceType(serviceElement, $"/{ConfigurationFileElementNames.RootElement}/{ConfigurationFileElementNames.SettingsRequestor}");

                if (serviceElement.ServiceTypeInfo.Type == typeof(IStartupAction) || serviceElement.ServiceTypeInfo.Type == typeof(IEnumerable<IStartupAction>))
                    ThrowOnProhibitedServiceType(serviceElement, $"/{ConfigurationFileElementNames.RootElement}/{ConfigurationFileElementNames.StartupActions}/{ConfigurationFileElementNames.StartupAction}");

                if (serviceElement.ServiceTypeInfo.Type == typeof(IPlugin))
                    ThrowOnProhibitedServiceType(serviceElement, $"/{ConfigurationFileElementNames.RootElement}/{ConfigurationFileElementNames.PluginsSetup}/{ConfigurationFileElementNames.PluginSetup}/{ConfigurationFileElementNames.PluginImplementation}");

                if (!IoCServiceFactoryAmbientContext.Context.GetProhibitedServiceTypesInServicesElementChecker().IsServiceTypeAllowed(serviceElement.ServiceTypeInfo.Type))
                    throw new ConfigurationParseException(serviceElement, $"Type '{serviceElement.ServiceTypeInfo.TypeCSharpFullName}' cannot be used a service type in 'service' element.");

                _serviceTypeToServiceMap[serviceElement.ServiceTypeInfo.Type] = serviceElement;
                _allServices.AddLast(serviceElement);
            }
        }

        public IEnumerable<IServiceElement> AllServices => _allServices;

        public IServiceElement GetServiceByServiceType(Type serviceType)
        {
            return _serviceTypeToServiceMap.TryGetValue(serviceType, out var service) ? service : null;
        }

        #endregion

        #region Member Functions

        /// <summary>
        /// </summary>
        /// <param name="serviceElement"></param>
        /// <param name="serviceImplementatipnLocation"></param>
        /// <exception cref="ConfigurationParseException">Always throws this exception.</exception>
        private string ThrowOnProhibitedServiceType([NotNull] IServiceElement serviceElement, string serviceImplementatipnLocation)
        {
            throw new ConfigurationParseException(serviceElement, $"Type '{serviceElement.ServiceTypeInfo.TypeCSharpFullName}' cannot be used a service type in 'service' element. An implementation for interface '{serviceElement.ServiceTypeInfo.TypeCSharpFullName}'should be defined in element '{serviceImplementatipnLocation}'.", this);
        }

        #endregion
    }
}