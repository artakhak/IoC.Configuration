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
    public class FileBasedDiContainerConfigurator : FileBasedConfiguratorAbstr, IFileBasedDiContainerConfigurator
    {
        #region  Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedDiContainerConfigurator"/> class.
        /// </summary>
        /// <param name="fileBasedConfiguration">The file based configuration.</param>
        public FileBasedDiContainerConfigurator([NotNull] FileBasedConfiguration fileBasedConfiguration) : base(fileBasedConfiguration)
        {
        }

        #endregion

        #region IFileBasedDiContainerConfigurator Interface Implementation        
        /// <summary>
        /// Registers the modules.
        /// </summary>
        /// <returns></returns>
        public IFileBasedContainerStarter RegisterModules()
        {
            _fileBasedConfiguration.RegisterModulesWithDiManager();
            return new FileBasedContainerStarter(_fileBasedConfiguration);
        }

        /// <summary>
        /// Will use <see cref="IDiContainer"/> instance passed as a parameter when configuring the container.
        /// Use the method <see cref="WithDiContainer(IDiContainer)"/> if possible.
        /// </summary>
        /// <param name="diContainer">An instance of <see cref="IDiContainer"/>.</param>
        /// <returns>Returns an instance of <see cref="IFileBasedDiModulesConfigurator"/></returns>
        public IFileBasedDiModulesConfigurator WithDiContainer(IDiContainer diContainer)
        {
            _fileBasedConfiguration.DiContainer = diContainer;
            return new FileBasedDiModulesConfigurator(_fileBasedConfiguration);
        }

        /// <summary>
        /// The container will be automatically created. This is the preferred way to build a container.
        /// Use <see cref="M:IoC.Configuration.DiContainerBuilder.FileBased.IFileBasedDiContainerConfigurator.WithDiContainer(IoC.Configuration.DiContainer.IDiContainer)" /> only if the application already has a container, and we need to use it.
        /// </summary>
        /// <returns>
        /// Returns an instance of <see cref="T:IoC.Configuration.DiContainerBuilder.FileBased.IFileBasedDiModulesConfigurator" />
        /// </returns>
        public IFileBasedDiModulesConfigurator WithoutPresetDiContainer()
        {
            return new FileBasedDiModulesConfigurator(_fileBasedConfiguration);
        }

        #endregion
    }
}