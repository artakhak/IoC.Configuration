using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public interface IBindingImplementationGeneric<TService, TImplementation> : IBindingImplementation
    {
        /// <summary>
        ///     Use this member to add multiple implementations for the same service.
        /// </summary>
        [NotNull]
        IBindingGeneric<TService> Service { get; }

        [NotNull]
        IBindingImplementationGeneric<TService, TImplementation> OnImplementationObjectActivated([NotNull] Action<IDiContainer, TImplementation> onImplementationActivated);

        [NotNull]
        IBindingImplementationGeneric<TService, TImplementation> SetResolutionScope(DiResolutionScope resolutionScope);

#if DEBUG
// Will enable this code in release mode when Autofac implementation for this feature is available.
        //[NotNull]
        //IBindingImplementationGeneric<TService, TImplementation> WhenInjectedInto(Type targetType, bool considerAlsoTargetTypeSubclasses);

        //[NotNull]
        //IBindingImplementationGeneric<TService, TImplementation> WhenInjectedInto<T>(bool considerAlsoTargetTypeSubclasses);
#endif
    }
}