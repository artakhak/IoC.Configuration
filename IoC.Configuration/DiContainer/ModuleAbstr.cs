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
using IoC.Configuration.DiContainer.BindingsForCode;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    ///     Normally, one would subclass from <see cref="ModuleAbstr" /> and override the method
    ///     <see cref="ModuleAbstr.AddServiceRegistrations" />.
    ///     Within the body of <see cref="ModuleAbstr.AddServiceRegistrations" />, one can use statements like:
    ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
    ///     ().SetResolutionScope(DiResolutionScope.Singleton);
    /// </summary>
    public abstract class ModuleAbstr : IDiModule
    {
        #region Member Variables

        private readonly List<BindingConfigurationForCode> _serviceBindingConfigurations = new List<BindingConfigurationForCode>();
        private IServiceRegistrationBuilder _serviceRegistrationBuilder;

        #endregion

        #region IDiModule Interface Implementation

        public void Init(IServiceRegistrationBuilder serviceRegistrationBuilder)
        {
            if (_serviceRegistrationBuilder != null)
            {
                var error = $"There can be only a single call to '{GetType().FullName}.{nameof(Init)}({typeof(IServiceRegistrationBuilder).FullName})'";
                LogHelper.Context.Log.Error(error);

                throw new Exception("Method was already executed.");
            }

            _serviceRegistrationBuilder = serviceRegistrationBuilder;
        }

        public IReadOnlyList<BindingConfigurationForCode> ServiceBindingConfigurations => _serviceBindingConfigurations;

        #endregion

        #region Current Type Interface

        /// <summary>
        ///     Override this method to register services. The body of overridden method might have statements like:
        ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
        ///     ().SetResolutionScope(DiResolutionScope.Singleton);
        /// </summary>
        protected abstract void AddServiceRegistrations();

        /// <summary>
        ///     Adds bindings and validates the added bindings.
        /// </summary>
        public virtual void Load()
        {
            _serviceRegistrationBuilder.BindingConfigurationAdded += BindingConfigurationAdded;

            AddServiceRegistrations();

            _serviceRegistrationBuilder.BindingConfigurationAdded -= BindingConfigurationAdded;

            foreach (var serviceBindingConfiguration in _serviceBindingConfigurations)
                serviceBindingConfiguration.Validate();
        }

        #endregion

        #region Member Functions

        /// <summary>
        ///     Creates a binding for service <typeparamref name="TService" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        [NotNull]
        protected IBindingGeneric<TService> Bind<TService>()
        {
            return _serviceRegistrationBuilder.Bind<TService>();
        }

        /// <summary>
        ///     Creates a binding for service <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        [NotNull]
        protected IBindingNonGeneric Bind(Type serviceType)
        {
            return _serviceRegistrationBuilder.Bind(serviceType);
        }

        private void BindingConfigurationAdded([CanBeNull] object sender, [NotNull] BindingConfigurationAddedEventArgs e)
        {
            _serviceBindingConfigurations.Add(e.BindingConfiguration);
        }

        #endregion
    }
}