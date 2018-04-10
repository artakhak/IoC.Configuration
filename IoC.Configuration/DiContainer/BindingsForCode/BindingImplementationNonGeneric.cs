using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class BindingImplementationNonGeneric : BindingImplementation, IBindingImplementationNonGeneric
    {
        public BindingImplementationNonGeneric([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                                               [NotNull] BindingImplementationConfigurationForCode bindingImplementationConfiguration,
                                               [NotNull] BindingNonGeneric serviceBinding) : base(serviceRegistrationBuilder, bindingImplementationConfiguration)
        {
            Service = serviceBinding;
        }

        public IBindingNonGeneric Service { get; }

        public IBindingImplementationNonGeneric OnImplementationObjectActivated(Action<IDiContainer, object> onImplementationActivated)
        {
            BindingImplementationConfiguration.OnImplementationObjectActivated = onImplementationActivated;
            return this;
        }

        public IBindingImplementationNonGeneric SetResolutionScope(DiResolutionScope resolutionScope)
        {
            BindingImplementationConfiguration.ResolutionScope = resolutionScope;
            return this;
        }

#if DEBUG
// Will enable this code in release mode when Autofac implementation for this feature is available.
        //public IBindingImplementationNonGeneric WhenInjectedInto(Type targetType, bool considerAlsoTargetTypeSubclasses)
        //{
        //    this.BindingImplementationConfiguration.SetWhenInjectedIntoData(
        //        considerAlsoTargetTypeSubclasses ? ConditionalInjectionType.WhenInjectedInto :
        //            ConditionalInjectionType.WhenInjectedExactlyInto, targetType);
        //    return this;
        //}
        
        //public IBindingImplementationNonGeneric WhenInjectedInto<T>(bool considerAlsoTargetTypeSubclasses)
        //{
        //    this.BindingImplementationConfiguration.SetWhenInjectedIntoData(
        //        considerAlsoTargetTypeSubclasses ? ConditionalInjectionType.WhenInjectedInto :
        //            ConditionalInjectionType.WhenInjectedExactlyInto, typeof(T));
        //    return this;
        //} 
#endif
    }
}