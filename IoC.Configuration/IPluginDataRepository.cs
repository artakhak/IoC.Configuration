using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public interface IPluginDataRepository
    {
        #region Current Type Interface

        [CanBeNull]
        IPluginData GetPluginData([NotNull] string pluginName);

        [CanBeNull]
        IPluginData GetPluginData<TPluginImplementation>();

        [CanBeNull]
        IPluginData GetPluginData(Type pluginImplementationType);

        [NotNull]
        [ItemNotNull]
        IEnumerable<IPluginData> Plugins { get; }

        #endregion
    }
}