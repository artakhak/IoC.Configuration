using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IServices : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<IServiceElement> AllServices { get; }

        [CanBeNull]
        IServiceElement GetServiceByServiceType([NotNull] Type serviceType);

        #endregion
    }
}