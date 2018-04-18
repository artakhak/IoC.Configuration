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
using OROptimizer;

namespace IoC.Configuration.DiContainer
{
    public class BindingConfiguration<TBindingImplementationConfiguration> where TBindingImplementationConfiguration : BindingImplementationConfiguration
    {
        #region Member Variables

        [NotNull]
        [ItemNotNull]
        private readonly List<TBindingImplementationConfiguration> _implementations = new List<TBindingImplementationConfiguration>(10);

        private bool? _registerIfNotRegistered;

        #endregion

        #region  Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingConfiguration{TBindingImplementationConfiguration}"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        public BindingConfiguration([NotNull] Type serviceType)
        {
            ServiceType = serviceType;

            GlobalsCoreAmbientContext.Context.EnsureParameterNotNull(nameof(serviceType), ServiceType);
        }

        #endregion

        #region Current Type Interface

        /// <summary>
        ///     Override this to do more validation, before implementations are validated
        /// </summary>
        protected virtual void ValidateBeforeImplementationsValidated()
        {
        }

        #endregion

        #region Member Functions        
        /// <summary>
        /// Adds an implementation.
        /// </summary>
        /// <param name="bindingImplementationConfiguration">The binding implementation configuration.</param>
        public void AddImplementation([NotNull] TBindingImplementationConfiguration bindingImplementationConfiguration)
        {
            _implementations.Add(bindingImplementationConfiguration);
            BindingImplementationConfigurationAdded?.Invoke(this, new BindingImplementationConfigurationAddedEventArgs<TBindingImplementationConfiguration>(bindingImplementationConfiguration));
        }

        /// <summary>
        /// Occurs when <see cref="AddImplementation"/>(TBindingImplementationConfiguration) method is called.
        /// </summary>
        [CanBeNull]
        public event BindingImplementationConfigurationAddedEventHandler<TBindingImplementationConfiguration> BindingImplementationConfigurationAdded;

        /// <summary>
        /// Gets the implementations.
        /// </summary>
        /// <value>
        /// The implementations.
        /// </value>
        [NotNull, ItemNotNull]
        public IReadOnlyList<TBindingImplementationConfiguration> Implementations => _implementations;

        /// <summary>
        /// Gets a value indicating whether this instance is self bound service.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is self bound service; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelfBoundService => _implementations.Count == 1 && _implementations[0].TargetImplementationType == TargetImplementationType.Self;

        /// <summary>
        /// If the value is true, the binding will be registered only if binding for the service <see cref="ServiceType"/> has not been already registered.
        /// Note, multiple implementations can still be registered using this binding, if the binding for service was not registered.
        /// </summary>
        public bool RegisterIfNotRegistered
        {
            get => _registerIfNotRegistered ?? false;
            set
            {
                if (_registerIfNotRegistered != null)
                    GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                        $"Value of '{typeof(BindingConfiguration<TBindingImplementationConfiguration>)}.{nameof(RegisterIfNotRegistered)}' can be set only once for the same binding.");

                _registerIfNotRegistered = value;
            }
        }

        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        [NotNull]
        public Type ServiceType { get; }

        /// <summary>
        /// Validates the binding configuration data.
        /// </summary>
        /// <exception cref="Exception">Throws an exception if service binding data is invalid.</exception>
        public void Validate()
        {
            if (_implementations.Count == 0)
                GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(MessagesHelper.GetNoImplementationsForServiceMessage(ServiceType));

            if (RegisterIfNotRegistered && _implementations.Count > 1)
                throw new Exception(MessagesHelper.GetMultipleImplementationsWithRegisterIfNotRegisteredOptionMessage($"'{GetType().FullName}.{nameof(RegisterIfNotRegistered)}'"));

            ValidateBeforeImplementationsValidated();

            foreach (var implementationConfiguration in _implementations)
                implementationConfiguration.Validate();
        }

        #endregion
    }
}