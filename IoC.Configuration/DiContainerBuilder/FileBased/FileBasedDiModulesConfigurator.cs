using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public class FileBasedDiModulesConfigurator : FileBasedConfiguratorAbstr, IFileBasedDiModulesConfigurator
    {
        #region  Constructors

        public FileBasedDiModulesConfigurator([NotNull] FileBasedConfiguration fileBasedConfiguration) : base(fileBasedConfiguration)
        {
        }

        #endregion

        #region IFileBasedDiModulesConfigurator Interface Implementation

        public IFileBasedDiModulesConfigurator AddAdditionalDiModules(params IDiModule[] diModules)
        {
            _fileBasedConfiguration.AddDiModules(diModules);
            return this;
        }

        public IFileBasedDiModulesConfigurator AddNativeModules(params object[] nativeModules)
        {
            _fileBasedConfiguration.AddNativeModules(nativeModules);
            return this;
        }

        public IFileBasedContainerStarter RegisterModules()
        {
            _fileBasedConfiguration.RegisterModulesWithDiManager();
            return new FileBasedContainerStarter(_fileBasedConfiguration);
        }

        #endregion
    }
}