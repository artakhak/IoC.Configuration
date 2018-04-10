using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public interface IBindingNonGeneric
    {
        #region Current Type Interface

        [NotNull]
        IBindingNonGeneric OnlyIfNotRegistered();

        [NotNull]
        BindingImplementationNonGeneric To([NotNull] Type implementationType);

        [NotNull]
        BindingImplementationNonGeneric To([NotNull] Func<IDiContainer, object> resolverFunc);

        [NotNull]
        IBindingImplementationNonGeneric ToSelf();

        #endregion
    }
}