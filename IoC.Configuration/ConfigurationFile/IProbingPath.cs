using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IProbingPath : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        string Path { get; }

        #endregion
    }
}