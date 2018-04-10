using System.Collections.Generic;

namespace IoC.Configuration.ConfigurationFile
{
    public interface ISettingsElement : IConfigurationFileElement
    {
        #region Current Type Interface

        IEnumerable<ISettingElement> AllSettings { get; }

        #endregion
    }
}