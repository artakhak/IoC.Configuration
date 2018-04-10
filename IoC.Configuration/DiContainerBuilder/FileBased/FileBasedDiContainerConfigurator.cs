using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public class FileBasedDiContainerConfigurator : FileBasedConfiguratorAbstr, IFileBasedDiContainerConfigurator
    {
        #region  Constructors

        public FileBasedDiContainerConfigurator([NotNull] FileBasedConfiguration fileBasedConfiguration) : base(fileBasedConfiguration)
        {
        }

        #endregion

        #region IFileBasedDiContainerConfigurator Interface Implementation

        public IFileBasedContainerStarter RegisterModules()
        {
            _fileBasedConfiguration.RegisterModulesWithDiManager();
            return new FileBasedContainerStarter(_fileBasedConfiguration);
        }

        public IFileBasedDiModulesConfigurator WithDiContainer(IDiContainer diContainer)
        {
            _fileBasedConfiguration.DiContainer = diContainer;
            return new FileBasedDiModulesConfigurator(_fileBasedConfiguration);
        }

        public IFileBasedDiModulesConfigurator WithoutPresetDiContainer()
        {
            return new FileBasedDiModulesConfigurator(_fileBasedConfiguration);
        }

        #endregion
    }
}