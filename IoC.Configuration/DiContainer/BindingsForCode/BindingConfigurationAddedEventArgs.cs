using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class BindingConfigurationAddedEventArgs : EventArgs
    {
        #region  Constructors

        public BindingConfigurationAddedEventArgs([NotNull] BindingConfigurationForCode bindingConfiguration)
        {
            BindingConfiguration = bindingConfiguration;
        }

        #endregion

        #region Member Functions

        [NotNull]
        public BindingConfigurationForCode BindingConfiguration { get; }

        #endregion
    }
}