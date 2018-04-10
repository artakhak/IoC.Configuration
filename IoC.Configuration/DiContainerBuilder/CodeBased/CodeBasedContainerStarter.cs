using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.CodeBased
{
    public class CodeBasedContainerStarter : CodeBasedConfiguratorAbstr, ICodeBasedContainerStarter
    {
        #region  Constructors

        public CodeBasedContainerStarter([NotNull] CodeBasedConfiguration codeBasedConfiguration) : base(codeBasedConfiguration)
        {
        }

        #endregion

        #region ICodeBasedContainerStarter Interface Implementation

        public IContainerInfo Start()
        {
            return _codeBasedConfiguration.StartContainer();
        }

        #endregion
    }
}