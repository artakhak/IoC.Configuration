using System.Collections.Generic;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IModulesElement : IConfigurationFileElement
    {
        #region Current Type Interface

        IEnumerable<IModuleElement> Modules { get; }

        #endregion
    }
}