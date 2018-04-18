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
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;

namespace IoC.Configuration.DiContainerBuilder.CodeBased
{
    /// <summary>
    /// A class that stores data for building dependency injection container using code based configuration.
    /// </summary>
    /// <seealso cref="IoC.Configuration.DiContainerBuilder.DiContainerBuilderConfiguration" />
    public class CodeBasedConfiguration : DiContainerBuilderConfiguration
    {
        #region Member Variables

        [NotNull, ItemNotNull]
        private readonly LinkedList<string> _assemblyProbingPaths = new LinkedList<string>();

        [NotNull]
        private AssemblyResolver _assemblyResolver;

        #endregion

        #region  Constructors
       
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBasedConfiguration"/> class.
        /// </summary>
        /// <param name="diManager">The DI manager.</param>
        /// <param name="entryAssemblyFolder">
        ///     The location where the executable is.
        ///     For non test projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> can be used as a value for this parameter.
        ///     However, for tests projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> might be
        ///     be the folder where the test execution library is, so a different value might need to be passed.
        /// </param>
        /// <param name="assemblyProbingPaths">The assembly probing paths.</param>
        public CodeBasedConfiguration([NotNull] IDiManager diManager,
                                      [NotNull] string entryAssemblyFolder,
                                      [CanBeNull] [ItemNotNull] params string[] assemblyProbingPaths) : base(entryAssemblyFolder)
        {
            InitProbingPaths(assemblyProbingPaths);
            DiManager = diManager;
        }

        /// <summary>
        /// </summary>
        /// <param name="diManagerClassFullName"></param>
        /// <param name="diManagerClassAssemblyFilePath"></param>
        /// <param name="diManagerConstructorParameters"></param>
        /// <param name="entryAssemblyFolder">
        ///     The location where the executable is.
        ///     For non test projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> can be used as a value for this parameter.
        ///     However, for tests projects <see cref="IGlobalsCore.EntryAssemblyFolder" /> might be
        ///     be the folder where the test execution library is, so a different value might need to be passed.
        /// </param>
        /// <param name="assemblyProbingPaths"></param>
        public CodeBasedConfiguration([NotNull] string diManagerClassFullName,
                                      [NotNull] string diManagerClassAssemblyFilePath,
                                      [CanBeNull] [ItemNotNull] ParameterInfo[] diManagerConstructorParameters,
                                      [NotNull] string entryAssemblyFolder,
                                      [CanBeNull] [ItemNotNull] params string[] assemblyProbingPaths) : base(entryAssemblyFolder)
        {
            InitProbingPaths(assemblyProbingPaths);
            DiManager = GlobalsCoreAmbientContext.Context.CreateInstance<IDiManager>(diManagerClassFullName, diManagerClassAssemblyFilePath, diManagerConstructorParameters);
        }

        #endregion

        #region Member Functions

        public void AddNativeModule([NotNull] string nativeModuleClassFullName,
                                    [NotNull] string nativeModuleClassAssemblyFilePath,
                                    [CanBeNull] [ItemNotNull] ParameterInfo[] nativeModuleConstructorParameters)
        {
            AddNativeModules(GlobalsCoreAmbientContext.Context.CreateInstance<object>(nativeModuleClassFullName, nativeModuleClassAssemblyFilePath, nativeModuleConstructorParameters));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _assemblyResolver.Dispose();
        }


        protected override IEnumerable<object> GenerateAllNativeModules()
        {
            var allNativeModules = new List<object>(NativeAndDiModules.Count + 5);


            foreach (var nativeOrDiModule in NativeAndDiModules)
                if (nativeOrDiModule is IDiModule)
                    allNativeModules.Add(GenerateNativeModule((IDiModule) nativeOrDiModule));
                else
                    allNativeModules.Add(nativeOrDiModule);

            allNativeModules.Add(GenerateNativeModule(new DefaultModule(this)));
            return allNativeModules;
        }

        private object GenerateNativeModule([NotNull] IDiModule diModule)
        {
            diModule.Init(_serviceRegistrationBuilder);
            return DiManager.GenerateNativeModule(diModule);
        }

        private void InitProbingPaths([CanBeNull] [ItemNotNull] params string[] assemblyProbingPaths)
        {
            var assemblyProbingPathsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _assemblyProbingPaths.AddLast(_entryAssemblyFolder);

            if (assemblyProbingPaths != null)
                foreach (var assemblyProbingPath in assemblyProbingPaths)
                {
                    if (assemblyProbingPathsSet.Contains(assemblyProbingPath))
                    {
                        LogHelper.Context.Log.Warn($"Probing path '{assemblyProbingPath}' was already added.");
                        continue;
                    }

                    assemblyProbingPathsSet.Add(assemblyProbingPath);
                    _assemblyProbingPaths.AddLast(assemblyProbingPath);
                }

            _assemblyResolver = new AssemblyResolver(_assemblyProbingPaths);
        }

        #endregion

        #region Nested Types

        private class DefaultModule : ModuleAbstr
        {
            #region Member Variables

            [NotNull]
            private readonly CodeBasedConfiguration _codeBasedConfiguration;

            #endregion

            #region  Constructors

            public DefaultModule([NotNull] CodeBasedConfiguration codeBasedConfiguration)
            {
                _codeBasedConfiguration = codeBasedConfiguration;
            }

            #endregion

            #region Member Functions

            /// <summary>
            ///     Use OnlyIfNotRegistered with all binding configurations, to use custom binding that the user might have specified
            ///     in configuration
            ///     file or in modules.
            /// </summary>
            protected override void AddServiceRegistrations()
            {
                Bind<ITypeBasedSimpleSerializerAggregator>().OnlyIfNotRegistered().To(typeResolver => TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator())
                                                            .SetResolutionScope(DiResolutionScope.Singleton);
                Bind<IDiContainer>().OnlyIfNotRegistered().To(typeResolver => _codeBasedConfiguration.DiContainer).SetResolutionScope(DiResolutionScope.Singleton);
            }

            #endregion
        }

        #endregion
    }
}