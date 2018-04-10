using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public interface IFileBasedDiModulesConfigurator : IRegisterModulesWithDiManagerForFileBasedConfiguration
    {
        #region Current Type Interface

        [NotNull]
        IFileBasedDiModulesConfigurator AddAdditionalDiModules([NotNull] [ItemNotNull] params IDiModule[] diModules);

        /// <summary>
        ///     Add native modules, such as Autofac or Ninject modules.
        /// </summary>
        /// <param name="nativeModules"></param>
        /// <returns></returns>
        [NotNull]
        IFileBasedDiModulesConfigurator AddNativeModules([NotNull] [ItemNotNull] params object[] nativeModules);

        #endregion
    }
}