using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class BindingNonGeneric : Binding, IBindingNonGeneric
    {
        #region  Constructors

        public BindingNonGeneric([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                                 [NotNull] BindingConfigurationForCode bindingConfiguration) : base(serviceRegistrationBuilder, bindingConfiguration)
        {
        }

        #endregion

        #region IBindingNonGeneric Interface Implementation

        public IBindingNonGeneric OnlyIfNotRegistered()
        {
            BindingConfiguration.RegisterIfNotRegistered = true;
            return this;
        }

        public BindingImplementationNonGeneric To(Type implementationType)
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateTypeBasedImplementationConfiguration(BindingConfiguration.ServiceType, implementationType);
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationNonGeneric(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        public BindingImplementationNonGeneric To(Func<IDiContainer, object> resolverFunc)
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateDelegateBasedImplementationConfiguration(BindingConfiguration.ServiceType, resolverFunc);
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationNonGeneric(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        public IBindingImplementationNonGeneric ToSelf()
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateSelfImplementationConfiguration(BindingConfiguration.ServiceType);
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationNonGeneric(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        #endregion
    }
}