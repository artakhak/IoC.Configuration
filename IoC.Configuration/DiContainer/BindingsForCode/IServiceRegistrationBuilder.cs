using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public interface IServiceRegistrationBuilder
    {
        #region Current Type Interface

        [NotNull]
        IBindingGeneric<TService> Bind<TService>();

        [NotNull]
        IBindingNonGeneric Bind(Type serviceType);

        event BindingConfigurationAddedEventHandler BindingConfigurationAdded;

        bool HasBinding([NotNull] Type serviceType);

        #endregion
    }
}