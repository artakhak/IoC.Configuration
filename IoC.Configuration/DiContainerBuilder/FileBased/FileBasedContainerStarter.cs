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

using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public class FileBasedContainerStarter : FileBasedConfiguratorAbstr, IFileBasedContainerStarter
    {
        #region  Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileBasedContainerStarter" /> class.
        /// </summary>
        /// <param name="fileBasedConfiguration">The file based configuration.</param>
        public FileBasedContainerStarter([NotNull] FileBasedConfiguration fileBasedConfiguration) : base(fileBasedConfiguration)
        {
        }

        #endregion

        #region IFileBasedContainerStarter Interface Implementation

        /// <summary>
        ///     Loads the DI container from the configuration file.
        /// </summary>
        /// <returns>Returns an instance of <see cref="IContainerInfo" />.</returns>
        public IContainerInfo Start()
        {
            return _fileBasedConfiguration.StartContainer();
        }

        #endregion
    }
}