using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IWebApi : IConfigurationFileElement
    {
        #region Current Type Interface

        [CanBeNull]
        IWebApiControllerAssemblies ControllerAssemblies { get; }

        #endregion
    }
}