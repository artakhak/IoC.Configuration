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

            if (child is IServiceElement)
                if (child.Enabled)
                {
                    var serviceElement = (IServiceElement) child;

                    if (_serviceTypeToServiceMap.ContainsKey(serviceElement.ServiceType))
                        throw new ConfigurationParseException(child, $"Multiple occurrences of service with the value of attribute '{ConfigurationFileAttributeNames.Type}' equal to '{serviceElement.ServiceType.FullName}'.", this);

                    if (serviceElement.ServiceType == typeof(ISettingsRequestor))
                        ThrowOnProhibitedServiceType(serviceElement, $"/{ConfigurationFileElementNames.RootElement}/{ConfigurationFileElementNames.SettingsRequestor}");

                    if (serviceElement.ServiceType == typeof(IStartupAction))
                        ThrowOnProhibitedServiceType(serviceElement, $"/{ConfigurationFileElementNames.RootElement}/{ConfigurationFileElementNames.StartupActions}/{ConfigurationFileElementNames.StartupAction}");

                    if (serviceElement.ServiceType == typeof(IPlugin))
                        ThrowOnProhibitedServiceType(serviceElement, $"/{ConfigurationFileElementNames.RootElement}/{ConfigurationFileElementNames.PluginsSetup}/{ConfigurationFileElementNames.PluginSetup}/{ConfigurationFileElementNames.PluginImplementation}");

                    if (!IoCServiceFactoryAmbientContext.Context.GetProhibitedServiceTypesInServicesElementChecker().IsServiceTypeAllowed(serviceElement.ServiceType))
                        throw new ConfigurationParseException(serviceElement, $"Type '{serviceElement.ServiceType.FullName}' cannot be used a service type in 'service' element.");

                    _serviceTypeToServiceMap[serviceElement.ServiceType] = serviceElement;
                }
        }

        public IEnumerable<IServiceElement> AllServices => _serviceTypeToServiceMap.Values;

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
            throw new ConfigurationParseException(serviceElement, $"Type '{serviceElement.ServiceType.FullName}' cannot be used a service type in 'service' element. An implementation for interface '{serviceElement.ServiceType.FullName}'should be defined in element '{serviceImplementatipnLocation}'.", this);
        }

        #endregion
    }
}