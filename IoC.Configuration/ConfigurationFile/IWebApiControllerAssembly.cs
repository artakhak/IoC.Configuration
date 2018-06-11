using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IWebApiControllerAssembly
    {
        #region Current Type Interface

        [NotNull]
        IAssembly Assembly { get; }

        /// <summary>
        ///     Gets the loaded assembly. The value is non-null only if assembly exists and is enabled.
        ///     If assembly does not exist, an error will be log.
        ///     Assembly might be disabled if the assembly belongs to a plugin which is disabled.
        /// </summary>
        System.Reflection.Assembly LoadedAssembly { get; }

        #endregion
    }
}