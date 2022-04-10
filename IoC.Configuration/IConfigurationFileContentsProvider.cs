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
        ///     database record id, that has the configuration. The value of this property might be used for diagnostics purposes,
        ///     if the configuration fails to be loaded.
        /// </summary>
        [NotNull]
        string ConfigurationFileSourceDetails { get; }

        /// <summary>
        ///     Returns IoC configuration file content as a string. The content should be a valid XML document and
        ///     should be successfully validated using the schema file
        ///     IoC.Configuration.Schema.7579ADB2-0FBD-4210-A8CA-EE4B4646DB3F.xsd.
        /// </summary>
        /// <returns>Returns a <see cref="String" /> object for the configuration file contents.</returns>
        /// <exception cref="Exception">Throws an exception if the xml file load fails.</exception>
        [NotNull]
        string LoadConfigurationFileContents();

        #endregion
    }
}