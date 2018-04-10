using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public sealed class BindingConfigurationForCode : BindingConfiguration<BindingImplementationConfigurationForCode>
    {
        #region  Constructors

        public BindingConfigurationForCode([NotNull] Type serviceType) : base(serviceType)
        {
        }

        #endregion
    }
}