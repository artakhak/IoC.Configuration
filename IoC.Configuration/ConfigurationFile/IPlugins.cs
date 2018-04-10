using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IPlugins : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<IPluginElement> AllPlugins { get; }

        [CanBeNull]
        IPluginElement GetPlugin([NotNull] string name);

        [NotNull]
        string PluginsDirectory { get; }

        #endregion
    }
}