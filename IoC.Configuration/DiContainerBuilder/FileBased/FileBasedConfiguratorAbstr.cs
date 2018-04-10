using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public abstract class FileBasedConfiguratorAbstr
    {
        #region Member Variables

        [NotNull]
        protected readonly FileBasedConfiguration _fileBasedConfiguration;

        #endregion

        #region  Constructors

        public FileBasedConfiguratorAbstr([NotNull] FileBasedConfiguration fileBasedConfiguration)
        {
            _fileBasedConfiguration = fileBasedConfiguration;
        }

        #endregion
    }
}