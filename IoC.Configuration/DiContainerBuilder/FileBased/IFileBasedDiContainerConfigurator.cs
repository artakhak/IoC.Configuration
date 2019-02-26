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

using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public interface IFileBasedDiContainerConfigurator : IRegisterModulesWithDiManagerForFileBasedConfiguration
    {
        #region Current Type Interface

        /// <summary>
        ///     Will use <see cref="IDiContainer" /> instance passed as a parameter when configuring the container.
        ///     Use the method <see cref="WithDiContainer(IDiContainer)" /> if possible.
        /// </summary>
        /// <param name="diContainer">An instance of <see cref="IDiContainer" />.</param>
        /// <returns>Returns an instance of <see cref="IFileBasedDiModulesConfigurator" /></returns>
        IFileBasedDiModulesConfigurator WithDiContainer([NotNull] IDiContainer diContainer);

        /// <summary>
        ///     The container will be automatically created. This is the preferred way to build a container.
        ///     Use <see cref="WithDiContainer" /> only if the application already has a container, and we need to use it.
        /// </summary>
        /// <returns>Returns an instance of <see cref="IFileBasedDiModulesConfigurator" /></returns>
        IFileBasedDiModulesConfigurator WithoutPresetDiContainer();

        #endregion
    }
}