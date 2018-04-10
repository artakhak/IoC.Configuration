using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IStartupActionsElement : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        IEnumerable<IStartupActionElement> StartupActions { get; }

        #endregion
    }
}