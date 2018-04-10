using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder
{
    public class ContainerInfo : IContainerInfo
    {
        #region Member Variables

        [NotNull]
        private readonly DiContainerBuilderConfiguration _diContainerBuilderConfiguration;

        #endregion

        #region  Constructors

        public ContainerInfo([NotNull] DiContainerBuilderConfiguration diContainerBuilderConfiguration)
        {
            _diContainerBuilderConfiguration = diContainerBuilderConfiguration;
            DiContainer = _diContainerBuilderConfiguration.DiContainer;
        }

        #endregion

        #region IContainerInfo Interface Implementation

        public IDiContainer DiContainer { get; }

        public void Dispose()
        {
            _diContainerBuilderConfiguration.Dispose();
        }

        #endregion
    }
}