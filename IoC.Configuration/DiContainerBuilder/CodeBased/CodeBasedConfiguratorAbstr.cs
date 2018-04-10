using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.CodeBased
{
    public abstract class CodeBasedConfiguratorAbstr
    {
        #region Member Variables

        [NotNull]
        protected readonly CodeBasedConfiguration _codeBasedConfiguration;

        #endregion

        #region  Constructors

        public CodeBasedConfiguratorAbstr([NotNull] CodeBasedConfiguration codeBasedConfiguration)
        {
            _codeBasedConfiguration = codeBasedConfiguration;
        }

        #endregion
    }
}