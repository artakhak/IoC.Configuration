using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IModuleElement : IConfigurationFileElement
    {
        #region Current Type Interface

        /// <summary>
        ///     Is null if the module is disabled.
        ///     Has non null value otherwise.
        /// </summary>
        [CanBeNull]
        object DiModule { get; }

        #endregion
    }
}