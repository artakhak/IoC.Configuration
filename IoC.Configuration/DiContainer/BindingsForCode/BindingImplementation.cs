using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class BindingImplementation : IBindingImplementation
    {
        #region Member Variables

        [NotNull]
        protected readonly BindingImplementationConfigurationForCode BindingImplementationConfiguration;

        [NotNull]
        protected readonly IServiceRegistrationBuilder ServiceRegistrationBuilder;

        #endregion

        #region  Constructors

        public BindingImplementation([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder,
                                     [NotNull] BindingImplementationConfigurationForCode bindingImplementationConfiguration)
        {
            ServiceRegistrationBuilder = serviceRegistrationBuilder;
            BindingImplementationConfiguration = bindingImplementationConfiguration;
        }

        #endregion
    }
}