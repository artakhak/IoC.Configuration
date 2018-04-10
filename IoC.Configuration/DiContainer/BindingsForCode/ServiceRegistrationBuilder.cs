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

        public IBindingGeneric<TService> Bind<TService>()
        {
            var bindingConfiguration = new BindingConfigurationForCode(typeof(TService));

            var binding = new BindingGeneric<TService>(this, bindingConfiguration);
            AddBinding(bindingConfiguration);
            return binding;
        }

        public IBindingNonGeneric Bind(Type serviceType)
        {
            var bindingConfiguration = new BindingConfigurationForCode(serviceType);
            AddBinding(bindingConfiguration);
            return new BindingNonGeneric(this, bindingConfiguration);
        }

        public event BindingConfigurationAddedEventHandler BindingConfigurationAdded;

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