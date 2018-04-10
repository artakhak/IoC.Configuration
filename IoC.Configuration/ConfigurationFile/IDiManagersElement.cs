using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IDiManagersElement : IConfigurationFileElement
    {
        #region Current Type Interface

        /// <summary>
        ///     The value is not null, if the configuration file is successfully loaded.
        /// </summary>
        [CanBeNull]
        IDiManagerElement ActiveDiManagerElement { get; }

        [NotNull]
        [ItemNotNull]
        IEnumerable<IDiManagerElement> AllDiManagers { get; }

        #endregion
    }
}