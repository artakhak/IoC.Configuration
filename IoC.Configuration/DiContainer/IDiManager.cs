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
using System.Collections.Generic;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;
using OROptimizer.ServiceResolver;

namespace IoC.Configuration.DiContainer
{
    public interface IDiManager
    {
        #region Current Type Interface

        /// <summary>
        ///     This method registers modules with service provider.
        /// </summary>
        /// <param name="diContainer">The DI container.</param>
        /// <param name="modules">The modules to register.</param>
        /// <exception cref="Exception"></exception>
        void BuildServiceProvider([NotNull] IDiContainer diContainer, [NotNull] [ItemNotNull] IEnumerable<object> modules);

        /// <summary>
        ///     Creates a native container object of type <see cref="IDiContainer" />. The object can be used to pass as a
        ///     parameter in class constructors, however it might not be in a state to be used to resolve services.
        ///     Before services can be resolved, call the method
        ///     <see cref="BuildServiceProvider(IDiContainer, IEnumerable{object})" /> and then
        ///     <see cref="StartServiceProvider(IDiContainer)" />.
        /// </summary>
        /// <returns></returns>
        IDiContainer CreateDiContainer();

        /// <summary>
        ///     DI container name, such as Autofac or Nnject.
        /// </summary>
        string DiContainerName { get; }

        /// <summary>
        ///     Generates a C# file contents for a native module, such as Autofac or Ninject module.
        ///     The generated class should have a parameterless constructor. If the generated module class has a public
        ///     method <see cref="HelpersIoC.OnDiContainerReadyMethodName" />(IoC.Configuration.DiContainer.IDiContainer
        ///     diContainer),
        ///     it will be executed with a parameter <see cref="IDiContainer" /> when the container is ready to resolve services.
        /// </summary>
        /// <param name="dynamicAssemblyBuilder">
        ///     The dynamic assembly builder.
        ///     The implementation can use this parameter to add referenced files to dynamically generated assembly which will
        ///     contain the generated module class.
        /// </param>
        /// <param name="assemblyLocator">The assembly locator.</param>
        /// <param name="moduleClassNamespace">The module class namespace.</param>
        /// <param name="moduleClassName">Name of the module class.</param>
        /// <param name="moduleServiceConfigurationElements">
        ///     Collection of all binding configurations for services to use when
        ///     building the module.
        /// </param>
        /// <returns>Returns a C# file contents for a native module, such as Autofac or Ninject module.</returns>
        /// <exception cref="Exception">Throw this exception if module fails to get generated.</exception>
        [NotNull]
        string GenerateModuleClassCode([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
                                       [NotNull] IAssemblyLocator assemblyLocator,
                                       [NotNull] string moduleClassNamespace, [NotNull] string moduleClassName,
                                       IEnumerable<BindingConfigurationForFile> moduleServiceConfigurationElements);

        /// <summary>
        ///     This class takes an object of type <see cref="IDiModule" />  and converts it to
        ///     a native module of some DI container, such as Autofac or Ninject module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns>Returns a native DI module such an Autofac Module class object.</returns>
        object GenerateNativeModule([NotNull] IDiModule module);


        /// <summary>
        ///     Returns a module object, such as Autofac or Ninjectmodule, which sets all the requierd binding.
        ///     For example this module can add binding for <see cref="IDiContainer" /> among other things.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        object GetRequiredBindingsModule();

        /// <summary>
        ///     For example for Autofac based implementation of <see cref="ModuleType" /> will return type of Autofac.Module.
        /// </summary>
        Type ModuleType { get; }

        /// <summary>
        ///     Starts the container. After this method is called,  calls to <see cref="IDiContainer"/>.<see cref="IServiceResolver.Resolve(Type)" /> can be made
        ///     to resolve services.
        /// </summary>
        /// <param name="diContainer">Dependency injection container to start.</param>
        void StartServiceProvider([NotNull] IDiContainer diContainer);

        #endregion
    }
}