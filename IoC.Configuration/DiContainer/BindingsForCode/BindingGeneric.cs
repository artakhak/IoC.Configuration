using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class BindingGeneric<TService> : Binding, IBindingGeneric<TService>
    {
        #region  Constructors

        public BindingGeneric([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                              [NotNull] BindingConfigurationForCode bindingConfiguration) : base(serviceRegistrationBuilder, bindingConfiguration)
        {
        }

        #endregion

        #region IBindingGeneric<TService> Interface Implementation

        public IBindingGeneric<TService> OnlyIfNotRegistered()
        {
            BindingConfiguration.RegisterIfNotRegistered = true;
            return this;
        }

        public IBindingImplementationGeneric<TService, TImplementation> To<TImplementation>() where TImplementation : TService
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateTypeBasedImplementationConfiguration(BindingConfiguration.ServiceType, typeof(TImplementation));
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationGeneric<TService, TImplementation>(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        public IBindingImplementationGeneric<TService, TService> To(Func<IDiContainer, TService> resolverFunc)
        {
            return To<TService>(resolverFunc);
        }

        public IBindingImplementationGeneric<TService, TImplementation> To<TImplementation>(Func<IDiContainer, TImplementation> resolverFunc) where TImplementation : TService
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateDelegateBasedImplementationConfiguration(BindingConfiguration.ServiceType,
                typeResolver => resolverFunc(typeResolver));

            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationGeneric<TService, TImplementation>(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        public IBindingImplementationGeneric<TService, TService> ToSelf()
        {
            var bindingImplementationConfiguration = BindingImplementationConfigurationForCode.CreateSelfImplementationConfiguration(BindingConfiguration.ServiceType);
            BindingConfiguration.AddImplementation(bindingImplementationConfiguration);
            return new BindingImplementationGeneric<TService, TService>(ServiceRegistrationBuilder, bindingImplementationConfiguration, this);
        }

        #endregion
    }
}