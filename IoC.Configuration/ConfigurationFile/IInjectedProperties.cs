using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IInjectedProperties : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<IInjectedPropertyElement> AllProperties { get; }

        #endregion
    }
}