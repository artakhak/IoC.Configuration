namespace IoC.Configuration.ConfigurationFile
{
    public interface IPluginSetup : IConfigurationFileElement
    {
        #region Current Type Interface

        IDependencyInjection DependencyInjection { get; }
        IPluginElement Plugin { get; }
        IPluginImplementationElement PluginImplementationElement { get; }
        ISettingsElement SettingsElement { get; }

        #endregion
    }
}