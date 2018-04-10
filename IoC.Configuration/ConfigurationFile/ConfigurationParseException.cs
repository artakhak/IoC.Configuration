using System;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ConfigurationParseException : Exception
    {
        #region  Constructors

        public ConfigurationParseException([NotNull] IConfigurationFileElement configurationFileElement, [NotNull] string message, IConfigurationFileElement parentElement = null) : base(configurationFileElement.GenerateElementError(message, parentElement))
        {
            ConfigurationFileElement = configurationFileElement;
            ParentConfigurationFileElement = parentElement;
        }

        public ConfigurationParseException([NotNull] string message) : base(message)
        {
        }

        #endregion

        #region Member Functions

        [CanBeNull]
        public IConfigurationFileElement ConfigurationFileElement { get; }

        [CanBeNull]
        public IConfigurationFileElement ParentConfigurationFileElement { get; }

        #endregion
    }
}