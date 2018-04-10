using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.CodeBased
{
    public class CodeBasedDiContainerConfigurator : CodeBasedConfiguratorAbstr, ICodeBasedDiContainerConfigurator
    {
        #region  Constructors

        public CodeBasedDiContainerConfigurator([NotNull] CodeBasedConfiguration codeBasedConfiguration) : base(codeBasedConfiguration)
        {
        }

        #endregion

        #region ICodeBasedDiContainerConfigurator Interface Implementation

        public ICodeBasedDiModulesConfigurator WithDiContainer(IDiContainer diContainer)
        {
            _codeBasedConfiguration.DiContainer = diContainer;
            return new CodeBasedDiModulesConfigurator(_codeBasedConfiguration);
        }

        public ICodeBasedDiModulesConfigurator WithoutPresetDiContainer()
        {
            return new CodeBasedDiModulesConfigurator(_codeBasedConfiguration);
        }

        #endregion

        #region Member Functions

        public ICodeBasedContainerStarter RegisterModules()
        {
            _codeBasedConfiguration.RegisterModulesWithDiManager();
            return new CodeBasedContainerStarter(_codeBasedConfiguration);
        }

        #endregion
    }
}