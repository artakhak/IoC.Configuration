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
        #region Member Variables

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

        [NotNull]
        protected readonly ServiceRegistrationBuilder _serviceRegistrationBuilder = new ServiceRegistrationBuilder();

        #endregion

        #region  Constructors

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

        #endregion

        #region Current Type Interface

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

        public virtual void ValidateConfiguration()
        {
        }

        #endregion

        #region Member Functions

        /// <summary>
        ///     Adds DI modules of type <see cref="IDiContainer" /> to container builder.
        /// </summary>
        /// <param name="diModules">The DI modules.</param>
        public void AddDiModules([NotNull] [ItemNotNull] params IDiModule[] diModules)
        {
            if (diModules == null)
                return;

            foreach (var module in diModules)
                _nativeAndDiModules.Add(module);
        }

        /// <summary>
        ///     Adds native (such as Autofac or Ninject) modules to container builder.
        /// </summary>
        /// <param name="nativeModules">The native modules.</param>
        public void AddNativeModules([NotNull] [ItemNotNull] params object[] nativeModules)
        {
            if (nativeModules == null)
                return;

            CheckDiManagerInitialized();

            foreach (var nativeModule in nativeModules)
            {
                if (!_diManager.ModuleType.IsAssignableFrom(nativeModule.GetType()))
                    GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"Invalid native module. Native module should be of a type '{_diManager.ModuleType.FullName}' or a sub-type of this type.", "Invalid native module.");

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

        /// <summary>
        ///     Registers the modules with DI manager.
        /// </summary>
        public void RegisterModulesWithDiManager()
        {
            try
            {
                CheckMethodCalledOnce(nameof(RegisterModulesWithDiManager), false);
                CheckDiManagerInitialized();

                if (_diContainer == null)
                    _diContainer = DiManager.CreateDiContainer();

                _generatedNativeModules = GenerateAllNativeModules();

                LogHelper.Context.Log.Info($"Registering modules with to container '{_diContainer.GetType().FullName}'.");
                DiManager.BuildServiceProvider(_diContainer, _generatedNativeModules);
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

                DiManager.StartServiceProvider(_diContainer);
                _diContainer.StartMainLifeTimeScope();

                // NOTE, It is important that DiContainerStatic and SerializerAggregatorStatic are initialized first thing after 
                // _diContainer.StartMainLifeTimeScope() is called, since this objects might be needed when resolving services in other
                // method calls that follow.
#pragma warning disable CS0612, CS0618
                DiContainerStatic = _diContainer;
                SerializerAggregatorStatic = _diContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
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

        #endregion
    }
}