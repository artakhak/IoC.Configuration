using System;

namespace IoC.Configuration
{
    public interface IPlugin : ISettingsRequestor, IDisposable
    {
        #region Current Type Interface

        void Initialize();

        /// <summary>
        ///     Sets/gets the <see cref="IPluginData" /> object corresponding to plugin, retrieved from configuration.
        ///     The implementation should ensure that the plugin data can be set only once, when the configuration is loaded.
        ///     The implementation can subclass from <see cref="PluginAbstr" /> to re-use this implementation.
        /// </summary>
        IPluginData PluginData { get; set; }

        #endregion
    }
}