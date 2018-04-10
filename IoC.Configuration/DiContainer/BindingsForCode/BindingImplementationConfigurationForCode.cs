using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public sealed class BindingImplementationConfigurationForCode : BindingImplementationConfiguration
    {
        #region Member Variables

        [CanBeNull]
        private Action<IDiContainer, object> _onImplementationObjectActivated;

        #endregion

        #region  Constructors

        private BindingImplementationConfigurationForCode([NotNull] Type serviceType, TargetImplementationType targetImplementationType, [CanBeNull] Type implementationType, [CanBeNull] Func<IDiContainer, object> implementationGeneratorFunction) :
            base(targetImplementationType, implementationType)
        {
            GlobalsCoreAmbientContext.Context.EnsureParameterNotNull(nameof(serviceType), serviceType);

            switch (TargetImplementationType)
            {
                case TargetImplementationType.Self:
                case TargetImplementationType.Type:

                    if (implementationType == null)
                        GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                            string.Format("The value of '{0}' cannot be null if '{1}' is '{2}'.",
                                nameof(implementationType), nameof(targetImplementationType),
                                targetImplementationType));

                    if (!serviceType.IsAssignableFrom(implementationType) ||
                        implementationType.IsAbstract || implementationType.IsInterface ||
                        implementationType.GetConstructors().FirstOrDefault(x => x.IsPublic) == null)
                    {
                        var errorMessageStrBldr = new StringBuilder();

                        errorMessageStrBldr.AppendLine($"Implementation '{implementationType}' specified for service '{serviceType.FullName}' is invalid.");
                        errorMessageStrBldr.AppendLine("The service type should be assignable from implementation type and the implementation type should be an instantiable class (e.g., not an interface or an abstract class).");
                        errorMessageStrBldr.AppendLine("Also, the implementation type should have at least one public constructor.");

                        GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(errorMessageStrBldr.ToString());
                    }

                    break;

                case TargetImplementationType.Delegate:
                    if (implementationGeneratorFunction == null)
                        GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                            string.Format("The value of '{0}' cannot be null if '{1}' is '{2}'.",
                                nameof(implementationGeneratorFunction), nameof(targetImplementationType),
                                targetImplementationType));

                    ImplementationGeneratorFunction = implementationGeneratorFunction;
                    break;
            }

            switch (TargetImplementationType)
            {
                case TargetImplementationType.Delegate:
                    if (implementationGeneratorFunction == null)
                        GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException(
                            string.Format("The value of '{0}' cannot be null if '{1}' is '{2}'.",
                                nameof(implementationGeneratorFunction), nameof(targetImplementationType),
                                targetImplementationType));

                    ImplementationGeneratorFunction = implementationGeneratorFunction;
                    break;
            }
        }

        #endregion

        #region Member Functions

        public static BindingImplementationConfigurationForCode CreateDelegateBasedImplementationConfiguration([NotNull] Type serviceType, [NotNull] Func<IDiContainer, object> implementationGeneratorFunction)
        {
            return new BindingImplementationConfigurationForCode(serviceType, TargetImplementationType.Delegate, null, implementationGeneratorFunction);
        }

        public static BindingImplementationConfigurationForCode CreateSelfImplementationConfiguration([NotNull] Type serviceType)
        {
            return new BindingImplementationConfigurationForCode(serviceType, TargetImplementationType.Self, serviceType, null);
        }

        public static BindingImplementationConfigurationForCode CreateTypeBasedImplementationConfiguration([NotNull] Type serviceType, [NotNull] Type implementationType)
        {
            return new BindingImplementationConfigurationForCode(serviceType, TargetImplementationType.Type, implementationType, null);
        }

        [CanBeNull]
        public Func<IDiContainer, object> ImplementationGeneratorFunction { get; }

        /// <summary>
        ///     Will be called after the implementation object is activated.
        ///     Use this for property injection.
        ///     The value of <see cref="object" /> parameter is the activated implementation instance.
        /// </summary>
        [CanBeNull]
        public Action<IDiContainer, object> OnImplementationObjectActivated
        {
            get => _onImplementationObjectActivated;
            set
            {
                CheckValueNotNullAndSetOnce(nameof(OnImplementationObjectActivated), value);
                _onImplementationObjectActivated = value;
            }
        }

        #endregion
    }
}