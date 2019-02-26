// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration
{
    /// <summary>
    ///     An implementation of <see cref="IConfigurationFileContentsProvider" />, that loads configuration file contents form
    ///     a file
    /// </summary>
    /// <seealso cref="IConfigurationFileContentsProvider" />
    public class FileBasedConfigurationFileContentsProvider : IConfigurationFileContentsProvider
    {
        #region  Constructors

        /// <summary>
        ///     A constructor.
        /// </summary>
        /// <param name="configurationFilePath"></param>
        public FileBasedConfigurationFileContentsProvider([NotNull] string configurationFilePath)
        {
            if (configurationFilePath == null)
            {
                LogHelper.Context.Log.Error(
                    $"The value of parameter '{nameof(ConfigurationFileSourceDetails)}' cannot be null.");
                throw new ArgumentNullException(nameof(configurationFilePath));
            }

            ConfigurationFileSourceDetails = configurationFilePath;
        }

        #endregion

        #region IConfigurationFileContentsProvider Interface Implementation

        public string ConfigurationFileSourceDetails { get; }


        /// <summary>
        ///     Returns IoC configuration file content as a string. The content should be a valid XML document and
        ///     should be successfully validated using the schema file
        ///     IoC.Configuration.Schema.2F7CE7FF-CB22-40B0-9691-EAC689C03A36.xsd.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="Stream" /> object for the configuration file contents.
        /// </returns>
        /// <exception cref="Exception">File failed to load.</exception>
        public string LoadConfigurationFileContents()
        {
            if (!File.Exists(ConfigurationFileSourceDetails))
            {
                LogHelper.Context.Log.Error(
                    $"The value of constructor parameter 'configurationFilePath' is invalid. File '{ConfigurationFileSourceDetails}' was not found.");

                throw new Exception("File failed to load.");
            }

            using (var streamReader = new StreamReader(ConfigurationFileSourceDetails))
                return streamReader.ReadToEnd();
        }

        #endregion
    }
}