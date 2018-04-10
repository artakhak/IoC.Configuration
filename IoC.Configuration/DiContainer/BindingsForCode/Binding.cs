using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public class Binding : IBinding
    {
        #region Member Variables

        [NotNull]
        protected readonly BindingConfigurationForCode BindingConfiguration;

        [NotNull]
        protected readonly IServiceRegistrationBuilder ServiceRegistrationBuilder;

        #endregion

        #region  Constructors

        public Binding([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder, [NotNull] BindingConfigurationForCode bindingConfiguration)
        {
            ServiceRegistrationBuilder = serviceRegistrationBuilder;
            BindingConfiguration = bindingConfiguration;
        }

        #endregion
    }
}