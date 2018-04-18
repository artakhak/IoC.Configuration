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
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public sealed class BindingConfigurationForFile : BindingConfiguration<BindingImplementationConfigurationForFile>
    {
        #region  Constructors

        private BindingConfigurationForFile([NotNull] Type serviceType, bool registerOnlyIfNotRegistered,
                                            [NotNull] [ItemNotNull] IEnumerable<IServiceImplementationElement> serviceImplementations)
            : base(serviceType)
        {
            RegisterIfNotRegistered = registerOnlyIfNotRegistered;

            foreach (var serviceImplementation in serviceImplementations)
            {
                if (!serviceImplementation.Enabled)
                    continue;

                AddImplementation(new BindingImplementationConfigurationForFile(serviceImplementation));
            }

            if (Implementations.Count == 0)
                LogHelper.Context.Log.WarnFormat("No implementation is provided for service '{0}' either because all the implementations are disabled or none exists.", ServiceType.FullName);
        }

        #endregion

        #region Member Functions        
        /// <summary>
        /// Creates the binding configuration for file.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="registerOnlyIfNotRegistered">if set to <c>true</c> [register only if not registered].</param>
        /// <param name="serviceImplementations">The service implementations.</param>
        /// <returns></returns>
        public static BindingConfigurationForFile CreateBindingConfigurationForFile([NotNull] Type serviceType, bool registerOnlyIfNotRegistered,
                                                                                    [NotNull] [ItemNotNull] IEnumerable<IServiceImplementationElement> serviceImplementations)
        {
            var bindingConfigurationForFile = new BindingConfigurationForFile(serviceType, registerOnlyIfNotRegistered, serviceImplementations);
            bindingConfigurationForFile.Validate();
            return bindingConfigurationForFile;
        }

        #endregion
    }
}