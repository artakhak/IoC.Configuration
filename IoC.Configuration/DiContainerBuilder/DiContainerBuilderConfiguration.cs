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
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainer.BindingsForCode;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;

namespace IoC.Configuration.DiContainerBuilder
{
    /// <summary>
    ///     A class that stores data for building dependency injection container.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public abstract class DiContainerBuilderConfiguration : IDisposable
    {
        [CanBeNull]
        private IDiContainer _diContainer;

        [CanBeNull]
        private IDiManager _diManager;

        [NotNull]
        protected readonly string _entryAssemblyFolder;

        [NotNull]
        private readonly HashSet<string> _executedMethods = new HashSet<string>(StringComparer.Ordinal);

        [CanBeNull]
        [ItemNotNull]
        private IEnumerable<object> _generatedNativeModules;

        [NotNull]
        [ItemNotNull]
        private readonly List<object> _nativeAndDiModules = new List<object>();

        [CanBeNull]
        private IApplicationHostBuilder _hostBuilder;

        [NotNull]
        protected readonly ServiceRegistrationBuilder _serviceRegistrationBuilder = new ServiceRegistrationBuilder();
       
        /// <summary>
        ///     A constructor.
        /// </summary>
        /// <param name="entryAssemblyFolder">
        ///     The location where the executable is.
        ///     For non test projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> can be used as a value for this parameter.
        ///     However, for tests projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> might be
        ///     be the folder where the test execution library is, so a different value might need to be passed.
        /// </param>
        public DiContainerBuilderConfiguration([NotNull] string entryAssemblyFolder)
        {
            _entryAssemblyFolder = entryAssemblyFolder;
        }

        public virtual void Dispose()
        {
            _diContainer?.Dispose();
        }

        protected abstract IEnumerable<object> GenerateAllNativeModules();

        /// <summary>
        ///     Override this to do initialization. This method should be called after the object is constructed.
        /// </summary>
        public virtual void Init()
        {
        }

        protected virtual void OnContainerStarted()
        {
        }

        /// <summary>
        ///     Adds DI modules of type <see cref="IDiContainer" /> to container builder.
        /// </summary>
        /// <param name="diModules">The DI modules.</param>
        public void AddDiModules([NotNull] [ItemNotNull] params IDiModule[] diModules)
        {
            foreach (var module in diModules)
                _nativeAndDiModules.Add(module);
        }

        /// <summary>
        ///     Adds native (such as Autofac or Ninject) modules to container builder.
        /// </summary>
        /// <param name="nativeModules">The native modules.</param>
        public void AddNativeModules([NotNull] [ItemNotNull] params object[] nativeModules)
        {
            CheckDiManagerInitialized();

            var diManager = GetDiManagerOrThrow();

            foreach (var nativeModule in nativeModules)
            {
                if (!diManager.ModuleType.IsAssignableFrom(nativeModule.GetType()))
                    GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"Invalid native module. Native module should be of a type '{diManager.ModuleType.FullName}' or a sub-type of this type.", "Invalid native module.");

                _nativeAndDiModules.Add(nativeModule);
            }
        }

        private void CheckDiManagerInitialized()
        {
            if (_diManager == null)
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"The value of property '{GetType().FullName}.{nameof(DiManager)}' is not initialized.");
        }

        protected void CheckMethodCalledOnce([NotNull] string methodOrPropertyName, bool isMethodName)
        {
            if (!_executedMethods.Add(methodOrPropertyName))
            {
                LogHelper.Context.Log.Error($"{(isMethodName ? "Method" : "Set property se")} '{GetType().FullName}.{methodOrPropertyName}' can be called only once.");
                throw new Exception("Multiple calls to a method.");
            }
        }

        /// <summary>
        ///     Gets the DI container.
        /// </summary>
        /// <value>
        ///     The DI container.
        /// </value>
        [CanBeNull]
        public IDiContainer DiContainer
        {
            get => _diContainer;
            set
            {
                CheckMethodCalledOnce(nameof(DiContainer), false);
                _diContainer = value;
            }
        }

        [Obsolete("This property should be used only in dynamically generated code. In all other cases either inject dependencies in constructors, or use IContainerInfo object returned by DiContainerBuilderConfiguration.StartContainer() (normally only in application entry code when the container is created).")]
        public static IDiContainer DiContainerStatic { get; private set; }

        /// <summary>
        ///     Gets or sets the DI manager.
        /// </summary>
        /// <value>
        ///     The DI manager.
        /// </value>
        [CanBeNull]
        public IDiManager DiManager
        {
            get => _diManager;
            protected set
            {
                CheckMethodCalledOnce(nameof(DiManager), false);
                _diManager = value;
            }
        }

        [NotNull]
        private IDiManager GetDiManagerOrThrow() => DiManager ?? throw new InvalidOperationException($"The value of '{nameof(DiManager)}' was not set!");

        /// <summary>
        ///     List of native module objects (such as Autofac or Ninject modules), as well as <see cref="IDiModule" /> objects
        /// </summary>
        [NotNull]
        [ItemNotNull]
        protected IReadOnlyList<object> NativeAndDiModules => _nativeAndDiModules;

        private void NotifyModulesOnContainerReady([NotNull] [ItemNotNull] IEnumerable<object> nativeModules, [NotNull] IDiContainer diContainer)
        {
            foreach (var nativeModule in nativeModules)
            {
                var onDiContainerReady = nativeModule.GetType().GetMethod(HelpersIoC.OnDiContainerReadyMethodName, new[] {typeof(IDiContainer)});

                if (onDiContainerReady != null && onDiContainerReady.IsPublic)
                    onDiContainerReady.Invoke(nativeModule, new object[] {diContainer});
            }
        }

        [CanBeNull]
        public IApplicationHostBuilder HostBuilder => _hostBuilder;

        public void SetHostBuilder([NotNull]IApplicationHostBuilder hostBuilder)
        {
            CheckMethodCalledOnce(nameof(SetHostBuilder), true);

            if (_diContainer != null)
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"The value of '{nameof(_diContainer)}' should not be set when {nameof(SetHostBuilder)} is non-null.");

            _hostBuilder = hostBuilder;
        }

        /// <summary>
        /// Registers the modules using <see cref="IHostBuilder"/>, builds the host by calling <see cref="IHostBuilder.Build()"/> and starts the container.
        /// </summary>
        internal void RegisterServiceProviderAndBuildApp()
        {
            try
            {
                if (_diContainer != null)
                {
                    GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"The value of '{nameof(_diContainer)}' should not be set when {nameof(_diContainer)} is non-null.");
                    // We will not get here, but helps the compiler.
                    throw new InvalidOperationException();
                }

                if (_hostBuilder == null)
                {
                    GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"The value of '{nameof(_hostBuilder)}' was not be set when {nameof(RegisterServiceProviderAndBuildApp)}() is executed.");
                    // We will not get here, but helps the compiler.
                    throw new InvalidOperationException();
                }

                CheckMethodCalledOnce(nameof(RegisterModulesWithDiManager), false);
                CheckDiManagerInitialized();

                var diManager = GetDiManagerOrThrow();

                _generatedNativeModules = GenerateAllNativeModules();

                LogHelper.Context.Log.Info("Registering modules for application builder.");

                // ReSharper disable once PossibleMultipleEnumeration
                // _generatedNativeModules will not be null here.
                diManager.BuildServiceProvider(_generatedNativeModules, _hostBuilder,
                    (diContainer) =>
                    {
                        if (_diContainer != null)
                            GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"The value of '{nameof(_diContainer)}' was set by the time callback 'diContainerCreated' executed.");

                        _diContainer = diContainer;
                    });

                /*LogHelper.Context.Log.Info("Registered modules for application builder. Building the host...");

                var host = _hostBuilder.Build();

                LogHelper.Context.Log.Info("Host was built.");

                // StartContainer() should be executed only after _hostBuilder.Build() is executed.
                var containerInfo = StartContainer();

                return new HostIntegratedContainerInfo(host, containerInfo);*/

            }
            catch (LoggerWasNotInitializedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Fatal(e);
                throw;
            }
        }

        /// <summary>
        ///     Registers the modules with DI manager.
        /// </summary>
        public void RegisterModulesWithDiManager()
        {
            try
            {
                CheckMethodCalledOnce(nameof(RegisterModulesWithDiManager), false);
                CheckDiManagerInitialized();

                if (_hostBuilder != null)
                {
                    GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"The value of '{nameof(_hostBuilder)}' should not be set when {nameof(RegisterModulesWithDiManager)}() is executed.");
                }

                var diManager = GetDiManagerOrThrow();

                if (_diContainer == null)
                    _diContainer = diManager.CreateDiContainer();

                _generatedNativeModules = GenerateAllNativeModules();

                LogHelper.Context.Log.Info($"Registering modules with container '{_diContainer.GetType().FullName}'.");
                diManager.BuildServiceProvider(_diContainer, _generatedNativeModules);
                LogHelper.Context.Log.Info($"Registered modules with to container '{_diContainer.GetType().FullName}'.");
            }
            catch (LoggerWasNotInitializedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Fatal(e);
                throw;
            }
        }

        [Obsolete("This property should be used only in dynamically generated code. In all other cases either inject dependencies in constructors, or use IContainerInfo object returned by DiContainerBuilderConfiguration.StartContainer() (normally only in application entry code when the container is created).")]
        public static ITypeBasedSimpleSerializerAggregator SerializerAggregatorStatic { get; private set; }

        /// <summary>
        ///     Starts the container.
        /// </summary>
        /// <returns></returns>
        public IContainerInfo StartContainer()
        {
            try
            {
                CheckMethodCalledOnce(nameof(StartContainer), false);

                GetDiManagerOrThrow().StartServiceProvider(_diContainer);
                _diContainer.StartMainLifeTimeScope();

                // NOTE, It is important that DiContainerStatic and SerializerAggregatorStatic are initialized first thing after 
                // _diContainer.StartMainLifeTimeScope() is called, since this objects might be needed when resolving services in other
                // method calls that follow.
#pragma warning disable CS0612, CS0618
                DiContainerStatic = _diContainer;
#pragma warning restore CS0612, CS0618

                NotifyModulesOnContainerReady(_generatedNativeModules, _diContainer);

                OnContainerStarted();
                return new ContainerInfo(this);
            }
            catch (LoggerWasNotInitializedException)
            {
                throw;
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Fatal(e);
                throw;
            }
        }
    }
}