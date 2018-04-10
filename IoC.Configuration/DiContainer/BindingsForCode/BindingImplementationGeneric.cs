using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class BindingImplementationGeneric<TService, TImplementation> : BindingImplementation, IBindingImplementationGeneric<TService, TImplementation>
    {
        public BindingImplementationGeneric([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                                            [NotNull] BindingImplementationConfigurationForCode bindingImplementationConfiguration,
                                            [NotNull] BindingGeneric<TService> serviceBinding) : base(serviceRegistrationBuilder, bindingImplementationConfiguration)
        {
            Service = serviceBinding;
        }

        public IBindingGeneric<TService> Service { get; }

        public IBindingImplementationGeneric<TService, TImplementation> OnImplementationObjectActivated(Action<IDiContainer, TImplementation> onImplementationActivated)
        {
            BindingImplementationConfiguration.OnImplementationObjectActivated =
                (typeResolver, implementationObject) => onImplementationActivated(typeResolver, (TImplementation) implementationObject);
            return this;
        }

        public IBindingImplementationGeneric<TService, TImplementation> SetResolutionScope(DiResolutionScope resolutionScope)
        {
            BindingImplementationConfiguration.ResolutionScope = resolutionScope;
            return this;
        }

#if DEBUG
// Will enable this code in release mode when Autofac implementation for this feature is available.
        //public IBindingImplementationGeneric<TService, TImplementation> WhenInjectedInto(Type targetType, bool considerAlsoTargetTypeSubclasses)
        //{
        //    BindingImplementationConfiguration.SetWhenInjectedIntoData(
        //        considerAlsoTargetTypeSubclasses ? ConditionalInjectionType.WhenInjectedInto :
        //        ConditionalInjectionType.WhenInjectedExactlyInto, targetType);
        //    return this;
        //}

        //public IBindingImplementationGeneric<TService, TImplementation> WhenInjectedInto<T>(bool considerAlsoTargetTypeSubclasses)
        //{
        //    BindingImplementationConfiguration.SetWhenInjectedIntoData(
        //        considerAlsoTargetTypeSubclasses ? ConditionalInjectionType.WhenInjectedInto :
        //            ConditionalInjectionType.WhenInjectedExactlyInto, typeof(T));
        //    return this;
        //} 
#endif
    }
}