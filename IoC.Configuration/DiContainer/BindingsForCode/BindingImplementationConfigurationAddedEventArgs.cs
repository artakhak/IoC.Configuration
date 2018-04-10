using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class BindingImplementationConfigurationAddedEventArgs<TBindingImplementationConfiguration> : EventArgs where TBindingImplementationConfiguration : BindingImplementationConfiguration
    {
        #region  Constructors

        public BindingImplementationConfigurationAddedEventArgs([NotNull] TBindingImplementationConfiguration bindingImplementationConfiguration)
        {
            BindingImplementationConfiguration = bindingImplementationConfiguration;
        }

        #endregion

        #region Member Functions

        [NotNull]
        public TBindingImplementationConfiguration BindingImplementationConfiguration { get; }

        #endregion
    }
}