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
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class ServiceRegistrationBuilder : IServiceRegistrationBuilder
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<Type, List<BindingConfigurationForCode>> _serviceTypeToBindingConfigurationsMap = new Dictionary<Type, List<BindingConfigurationForCode>>();

        #endregion

        #region IServiceRegistrationBuilder Interface Implementation

        /// <summary>
        ///     Creates a generic binding.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public IBindingGeneric<TService> Bind<TService>()
        {
            var bindingConfiguration = new BindingConfigurationForCode(typeof(TService));

            var binding = new BindingGeneric<TService>(this, bindingConfiguration);
            AddBinding(bindingConfiguration);
            return binding;
        }

        /// <summary>
        ///     Creates a non-generic binding.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IBindingNonGeneric Bind(Type serviceType)
        {
            var bindingConfiguration = new BindingConfigurationForCode(serviceType);
            AddBinding(bindingConfiguration);
            return new BindingNonGeneric(this, bindingConfiguration);
        }

        /// <summary>
        ///     Occurs when a binding for service is added.
        /// </summary>
        public event BindingConfigurationAddedEventHandler BindingConfigurationAdded;

        /// <summary>
        ///     Determines whether there is a binding for the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///     <c>true</c> if the specified service type has binding; otherwise, <c>false</c>.
        /// </returns>
        public bool HasBinding(Type serviceType)
        {
            if (!_serviceTypeToBindingConfigurationsMap.TryGetValue(serviceType, out var bindingConfigurations) || bindingConfigurations.Count == 0)
                return false;

            var implementations = bindingConfigurations[0].Implementations;

            if (implementations == null || implementations.Count == 0)
                return false;

            return true;
        }

        #endregion

        #region Member Functions

        private void AddBinding(BindingConfigurationForCode bindingConfiguration)
        {
            List<BindingConfigurationForCode> bindingConfigurations;
            if (!_serviceTypeToBindingConfigurationsMap.TryGetValue(bindingConfiguration.ServiceType, out bindingConfigurations))
                bindingConfigurations = new List<BindingConfigurationForCode>();
            else
                LogHelper.Context.Log.WarnFormat("A binding for service type '{0}' is aadded multiple times. This might result in unexpected behaviour. If the service should be bound to multiple implementations, use '{1}' in '{2}' or the similar member in generic type.",
                    bindingConfiguration.ServiceType.FullName, nameof(IBindingImplementationNonGeneric.Service), typeof(IBindingImplementationNonGeneric));

            bindingConfigurations.Add(bindingConfiguration);
            BindingConfigurationAdded?.Invoke(this, new BindingConfigurationAddedEventArgs(bindingConfiguration));
        }

        #endregion
    }
}