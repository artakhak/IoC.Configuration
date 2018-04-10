using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public interface IBindingGeneric<TService> : IBinding
    {
        #region Current Type Interface

        [NotNull]
        IBindingGeneric<TService> OnlyIfNotRegistered();

        [NotNull]
        IBindingImplementationGeneric<TService, TImplementation> To<TImplementation>() where TImplementation : TService;

        [NotNull]
        IBindingImplementationGeneric<TService, TService> To([NotNull] Func<IDiContainer, TService> resolverFunc);

        /// <summary>
        ///     TImplementation should be either <typeparamref name="TService" />, or a type that implements or derives from
        ///     <typeparamref name="TService" />.
        ///     Note, normally we can use value of TService for TImplementation not to restrict what the function
        ///     <paramref name="resolverFunc" /> returns, as long as it returns <typeparamref name="TService" /> object.
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="resolverFunc"></param>
        /// <returns></returns>
        [NotNull]
        IBindingImplementationGeneric<TService, TImplementation> To<TImplementation>([NotNull] Func<IDiContainer, TImplementation> resolverFunc) where TImplementation : TService;

        [NotNull]
        IBindingImplementationGeneric<TService, TService> ToSelf();

        #endregion
    }
}