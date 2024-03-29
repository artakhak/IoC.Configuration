﻿// This software is part of the IoC.Configuration library
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

using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder
{
    /// <summary>
    ///     Container data for the loaded DI
    /// </summary>
    /// <seealso cref="IoC.Configuration.DiContainerBuilder.IContainerInfo" />
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

        /// <summary>
        ///     An instance of <see cref="T:IoC.Configuration.DiContainer.IDiContainer" /> for the loaded container.
        /// </summary>
        /// <value>
        ///     The di container.
        /// </value>
        public IDiContainer DiContainer { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _diContainerBuilderConfiguration.Dispose();
        }

        #endregion
    }
}