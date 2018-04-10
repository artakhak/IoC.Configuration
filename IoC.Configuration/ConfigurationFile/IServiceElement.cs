using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IServiceElement : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<IServiceImplementationElement> Implementations { get; }

        bool RegisterIfNotRegistered { get; }

        [NotNull]
        Type ServiceType { get; }

        #endregion
    }
}