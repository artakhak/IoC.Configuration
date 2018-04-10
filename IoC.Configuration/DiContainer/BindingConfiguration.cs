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

        public void AddImplementation([NotNull] TBindingImplementationConfiguration bindingImplementationConfiguration)
        {
            _implementations.Add(bindingImplementationConfiguration);
            BindingImplementationConfigurationAdded?.Invoke(this, new BindingImplementationConfigurationAddedEventArgs<TBindingImplementationConfiguration>(bindingImplementationConfiguration));
        }

        [CanBeNull]
        public event BindingImplementationConfigurationAddedEventHandler<TBindingImplementationConfiguration> BindingImplementationConfigurationAdded;

        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<TBindingImplementationConfiguration> Implementations => _implementations;

        public bool IsSelfBoundService => _implementations.Count == 1 && _implementations[0].TargetImplementationType == TargetImplementationType.Self;

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

        [NotNull]
        public Type ServiceType { get; }

        /// <summary>
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