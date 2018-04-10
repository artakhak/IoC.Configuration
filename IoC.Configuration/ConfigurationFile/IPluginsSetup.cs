using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IPluginsSetup : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<IPluginSetup> AllPluginSetups { get; }

        [CanBeNull]
        IPluginSetup GetPluginSetup([NotNull] string pluginName);

        [CanBeNull]
        IPluginSetup GetPluginSetup([NotNull] Type pluginType);

        #endregion
    }
}