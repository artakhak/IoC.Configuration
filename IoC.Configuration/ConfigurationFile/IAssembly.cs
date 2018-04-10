using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IAssembly : INamedItem, IConfigurationFileElement
    {
        #region Current Type Interface

        /// <summary>
        ///     Absolute full path of assembly file.
        /// </summary>
        [NotNull]
        string AbsolutePath { get; }

        [NotNull]
        string Alias { get; }

        [CanBeNull]
        IPluginElement Plugin { get; }

        #endregion
    }
}