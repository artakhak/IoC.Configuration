using System;
using System.Collections.Generic;
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ProhibitedServiceTypesInServicesElementChecker : IProhibitedServiceTypesInServicesElementChecker
    {
        #region Member Variables

        [NotNull]
        private readonly HashSet<Type> _prohibitedTypes;

        #endregion

        #region  Constructors

        public ProhibitedServiceTypesInServicesElementChecker([CanBeNull] [ItemNotNull] IEnumerable<Type> additionalProhibitedTypes = null)
        {
            if (additionalProhibitedTypes != null)
                _prohibitedTypes = new HashSet<Type>(additionalProhibitedTypes);
            else
                _prohibitedTypes = new HashSet<Type>();

            _prohibitedTypes.Add(typeof(IPlugin));
            _prohibitedTypes.Add(typeof(IStartupAction));
            _prohibitedTypes.Add(typeof(ISettingsRequestor));
        }

        #endregion

        #region IProhibitedServiceTypesInServicesElementChecker Interface Implementation

        public bool IsServiceTypeAllowed(Type serviceType)
        {
            return !_prohibitedTypes.Contains(serviceType);
        }

        #endregion
    }
}