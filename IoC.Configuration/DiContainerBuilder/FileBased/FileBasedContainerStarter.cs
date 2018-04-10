using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public class FileBasedContainerStarter : FileBasedConfiguratorAbstr, IFileBasedContainerStarter
    {
        #region  Constructors

        public FileBasedContainerStarter([NotNull] FileBasedConfiguration fileBasedConfiguration) : base(fileBasedConfiguration)
        {
        }

        #endregion

        #region IFileBasedContainerStarter Interface Implementation

        public IContainerInfo Start()
        {
            return _fileBasedConfiguration.StartContainer();
        }

        #endregion
    }
}