using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.DiContainerBuilder.CodeBased
{
    public interface ICodeBasedDiModulesConfigurator : IRegisterModulesWithDiManagerForCodeBasedConfiguration
    {
        #region Current Type Interface

        [NotNull]
        ICodeBasedDiModulesConfigurator AddDiModules([NotNull] [ItemNotNull] params IDiModule[] diModules);

        [NotNull]
        ICodeBasedDiModulesConfigurator AddNativeModule([NotNull] string nativeModuleClassFullName,
                                                        [NotNull] string nativeModuleClassAssemblyFilePath,
                                                        [CanBeNull] [ItemNotNull] ParameterInfo[] nativeModuleConstructorParameters);

        /// <summary>
        ///     Add native modules, such as Autofac or Ninject modules.
        /// </summary>
        /// <param name="nativeModules"></param>
        [NotNull]
        ICodeBasedDiModulesConfigurator AddNativeModules([NotNull] [ItemNotNull] params object[] nativeModules);

        #endregion
    }
}