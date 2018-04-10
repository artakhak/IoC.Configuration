using System;
using System.IO;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    ///     An interface that provides IoC configuration file content.
    /// </summary>
    public interface IConfigurationFileContentsProvider
    {
        #region Current Type Interface

        /// <summary>
        ///     Some details of the source, where the configuration file contents are loaded from. Examples are: file path or
        ///     database record
        ///     id, that has the configuration.
        /// </summary>
        [NotNull]
        string ConfigurationFileSourceDetails { get; }

        /// <summary>
        ///     Returns IoC configuration file content as a string. The content should be a valid XML document and
        ///     should be successfully validated using the schema file IoC.Configuration.Schema.xsd.
        /// </summary>
        /// <returns>Returns a <see cref="Stream" /> object for the configuration file contents.</returns>
        /// <exception cref="Exception">Throws an exception if the stream fails to be created.</exception>
        [NotNull]
        string LoadConfigurationFileContents();

        #endregion
    }
}