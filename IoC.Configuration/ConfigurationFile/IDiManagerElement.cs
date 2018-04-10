using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IDiManagerElement : INamedItem, IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        IDiManager DiManager { get; }

        #endregion
    }
}