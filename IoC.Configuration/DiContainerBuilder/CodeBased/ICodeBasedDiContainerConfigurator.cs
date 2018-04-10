using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.CodeBased
{
    public interface ICodeBasedDiContainerConfigurator
    {
        #region Current Type Interface

        ICodeBasedDiModulesConfigurator WithDiContainer([NotNull] IDiContainer diContainer);

        /// <summary>
        ///     The container will be automatically created. This is the preferred way to build a container.
        ///     Use <see cref="WithDiContainer" /> only if the application already has a container, and we need to use it.
        /// </summary>
        /// <returns></returns>
        ICodeBasedDiModulesConfigurator WithoutPresetDiContainer();

        #endregion
    }
}