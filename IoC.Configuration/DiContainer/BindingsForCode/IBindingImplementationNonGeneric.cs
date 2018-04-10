using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public interface IBindingImplementationNonGeneric : IBindingImplementation
    {
        /// <summary>
        ///     Use this member to add multiple implementations for the same service.
        /// </summary>
        [NotNull]
        IBindingNonGeneric Service { get; }

        [NotNull]
        IBindingImplementationNonGeneric OnImplementationObjectActivated([NotNull] Action<IDiContainer, object> onImplementationActivated);

        [NotNull]
        IBindingImplementationNonGeneric SetResolutionScope(DiResolutionScope resolutionScope);

#if DEBUG
        //[NotNull]
        //IBindingImplementationNonGeneric WhenInjectedInto(Type targetType, bool considerAlsoTargetTypeSubclasses);

        //[NotNull]
        //IBindingImplementationNonGeneric WhenInjectedInto<T>(bool considerAlsoTargetTypeSubclasses); 
#endif
    }
}