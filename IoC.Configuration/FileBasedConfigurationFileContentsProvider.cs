using System;
using System.IO;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration
{
    /// <summary>
    ///     An implementation of IConfigurationFileContentProvider, that loads configuration file contents form a file
    /// </summary>
    public class FileBasedConfigurationFileContentsProvider : IConfigurationFileContentsProvider
    {
        #region Member Variables

        #endregion

        #region  Constructors

        /// <summary>
        ///     A constructor.
        /// </summary>
        /// <param name="configurationFilePath"></param>
        public FileBasedConfigurationFileContentsProvider([NotNull] string configurationFilePath)
        {
            if (configurationFilePath == null)
            {
                LogHelper.Context.Log.Error($"The value of parameter '{nameof(ConfigurationFileSourceDetails)}' cannot be null.");
                throw new ArgumentNullException(nameof(configurationFilePath));
            }

            ConfigurationFileSourceDetails = configurationFilePath;
        }

        #endregion

        #region IConfigurationFileContentsProvider Interface Implementation

        [NotNull]
        public string ConfigurationFileSourceDetails { get; }

        public string LoadConfigurationFileContents()
        {
            if (!File.Exists(ConfigurationFileSourceDetails))
            {
                LogHelper.Context.Log.Error($"The value of constructor parameter 'configurationFilePath' is invalid. File '{ConfigurationFileSourceDetails}' was not found.");

                throw new Exception("File failed to load.");
            }

            using (var streamReader = new StreamReader(ConfigurationFileSourceDetails))
            {
                return streamReader.ReadToEnd();
            }

            //return new FileStream(_configurationFilePath, FileMode.Open);
        }

        #endregion
    }
}