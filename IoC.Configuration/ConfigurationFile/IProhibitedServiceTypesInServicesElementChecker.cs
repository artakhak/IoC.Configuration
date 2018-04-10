using System;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    /// <summary>
    ///     Checks if the given type can be used as a service type under 'services' element in configuration file.
    /// </summary>
    public interface IProhibitedServiceTypesInServicesElementChecker
    {
        #region Current Type Interface

        /// <summary>
        ///     Checks if <paramref name="serviceType" /> can be used as a service type under
        ///     'services' element in configuration file.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns>
        ///     Returns true if <paramref name="serviceType" /> can be used as a service type under
        ///     'services' element in configuration file.
        ///     Returns false otherwise.
        /// </returns>
        bool IsServiceTypeAllowed([NotNull] Type serviceType);

        #endregion
    }
}