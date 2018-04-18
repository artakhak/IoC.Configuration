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
    public class FileBasedDiModulesConfigurator : FileBasedConfiguratorAbstr, IFileBasedDiModulesConfigurator
    {
        #region  Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedDiModulesConfigurator"/> class.
        /// </summary>
        /// <param name="fileBasedConfiguration">The file based configuration.</param>
        public FileBasedDiModulesConfigurator([NotNull] FileBasedConfiguration fileBasedConfiguration) : base(fileBasedConfiguration)
        {
        }

        #endregion

        #region IFileBasedDiModulesConfigurator Interface Implementation        
        /// <summary>
        /// Adds the additional <see cref="IDiModule"/> modules to be loaded into a container.
        /// </summary>
        /// <param name="diModules">The <see cref="IDiModule"/> modules to be loaded into a container.</param>
        /// <returns>Returns an instance of <see cref="IFileBasedDiModulesConfigurator"/></returns>
        public IFileBasedDiModulesConfigurator AddAdditionalDiModules(params IDiModule[] diModules)
        {
            _fileBasedConfiguration.AddDiModules(diModules);
            return this;
        }

        /// <summary>
        /// Add native modules, such as Autofac or Ninject modules to be loaded into a container.
        /// </summary>
        /// <param name="nativeModules">The native modules, such as Autofac or Ninject modules to be loaded into a container.</param>
        /// <returns>Returns an instance of <see cref="IFileBasedDiModulesConfigurator"/></returns>
        public IFileBasedDiModulesConfigurator AddNativeModules(params object[] nativeModules)
        {
            _fileBasedConfiguration.AddNativeModules(nativeModules);
            return this;
        }

        /// <summary>
        /// Registers the modules.
        /// </summary>
        /// <returns>returns an instance of <see cref="IFileBasedContainerStarter"/>.</returns>
        public IFileBasedContainerStarter RegisterModules()
        {
            _fileBasedConfiguration.RegisterModulesWithDiManager();
            return new FileBasedContainerStarter(_fileBasedConfiguration);
        }

        #endregion
    }
}