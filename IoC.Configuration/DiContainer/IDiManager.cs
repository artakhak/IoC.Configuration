using System;
using System.Collections.Generic;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.DiContainer
{
    public interface IDiManager
    {
        #region Current Type Interface

        /// <summary>
        ///     This method registers modules with service provider.
        /// </summary>
        /// <param name="diContainer"></param>
        /// <param name="modules"></param>
        /// <exception cref="Exception"></exception>
        void BuildServiceProvider([NotNull] IDiContainer diContainer, [NotNull] [ItemNotNull] IEnumerable<object> modules);

        /// <summary>
        ///     Creates a native container object of type <see cref="IDiContainer" />. The object can be used to pass as a
        ///     parameter in class constructors,
        ///     however it might not be in a state to be used to resolve services. before services can be resolved, call
        ///     the <see cref="BuildServiceProvider" /> method.
        /// </summary>
        /// <returns></returns>
        IDiContainer CreateDiContainer();

        string DiContainerName { get; }

        /// <summary>
        ///     Generates a new DI Module with module class name specified in parameter <paramref name="moduleClassName" />
        ///     Adds the generated module C# file using <see cref="IDynamicAssemblyBuilder.AddCSharpFile" /> method.
        ///     The generated class should have a parameterless constructor. If the generated module class has a public
        ///     method <see cref="HelpersIoC.OnDiContainerReadyMethodName" />(IoC.Configuration.DiContainer.IDiContainer
        ///     diContainer),
        ///     it will be executed with a parameter <see cref="IDiContainer" /> when the container is ready to resolve services.
        /// </summary>
        /// <param name="dynamicAssemblyBuilder"></param>
        /// <param name="assemblyLocator"></param>
        /// <param name="moduleClassNamespace"></param>
        /// <param name="moduleClassName"></param>
        /// <param name="moduleServiceConfigurationElements">Collection of all services to use when building the module</param>
        /// <exception cref="Exception">Throw this exception if module fails to get generated.</exception>
        /// <returns>Returns generated module class code.</returns>
        [NotNull]
        string GenerateModuleClassCode([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder,
                                       [NotNull] IAssemblyLocator assemblyLocator,
                                       [NotNull] string moduleClassNamespace, [NotNull] string moduleClassName,
                                       IEnumerable<BindingConfigurationForFile> moduleServiceConfigurationElements);

        /// <summary>
        ///     This class takes an object of type <see cref="IDiModule" />  and converts it to
        ///     a module of some DI container, such as Autofac or Ninject.
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
        ///     Starts the container. After this method is called,  calls to <see cref="IDiContainer.Resolve(Type)" /> can be made
        ///     to resolve services.
        /// </summary>
        /// <param name="diContainer">Dependency injection container to start.</param>
        void StartServiceProvider([NotNull] IDiContainer diContainer);

        #endregion
    }
}