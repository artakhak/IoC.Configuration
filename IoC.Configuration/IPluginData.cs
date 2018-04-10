using JetBrains.Annotations;

namespace IoC.Configuration
{
    public interface IPluginData
    {
        #region Current Type Interface

        [NotNull]
        IPlugin Plugin { get; }

        [NotNull]
        string PluginName { get; }

        [NotNull]
        ISettings Settings { get; }

        #endregion
    }
}