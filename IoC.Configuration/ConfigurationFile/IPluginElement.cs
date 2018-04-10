using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IPluginElement : IConfigurationFileElement, INamedItem
    {
        #region Current Type Interface

        [NotNull]
        string GetPluginDirectory();

        #endregion
    }
}