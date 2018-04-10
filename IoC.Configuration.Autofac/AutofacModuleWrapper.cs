using System;
using Autofac;
using Autofac.Builder;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainer.BindingsForCode;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.Autofac
{
    public class AutofacModuleWrapper : Module
    {
        #region Member Variables

        private IDiContainer _diContainer;

        [NotNull]
        private readonly IDiModule _module;

        private ITypeBasedSimpleSerializerAggregator _parameterSerializer;

        #endregion

        #region  Constructors

        public AutofacModuleWrapper([NotNull] IDiModule module)
        {
            _module = module;
            _module.Load();
        }

        #endregion

        #region Member Functions

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            foreach (var serviceBindingConfiguration in _module.ServiceBindingConfigurations)
            foreach (var implementationConfiguration in serviceBindingConfiguration.Implementations)
                switch (implementationConfiguration.TargetImplementationType)
                {
                    case TargetImplementationType.Type:
                    case TargetImplementationType.Self:
                    {
                        var registration = builder.RegisterType(implementationConfiguration.ImplementationType);

                        SetResolutionScope(registration, implementationConfiguration.ResolutionScope);
                        SetInstanceActivatedAction(registration, implementationConfiguration.OnImplementationObjectActivated);

                        if (implementationConfiguration.TargetImplementationType == TargetImplementationType.Type)
                            registration.As(serviceBindingConfiguration.ServiceType);
                        else
                            registration.AsSelf();

                        SetRegisterIfNotRegistered(registration, serviceBindingConfiguration);
                    }

                        break;

                    case TargetImplementationType.Delegate:
                    {
                        var registration = builder.Register(context =>
                            implementationConfiguration.ImplementationGeneratorFunction(_diContainer));

                        SetResolutionScope(registration, implementationConfiguration.ResolutionScope);
                        SetInstanceActivatedAction(registration, implementationConfiguration.OnImplementationObjectActivated);
                        registration.As(serviceBindingConfiguration.ServiceType);

                        SetRegisterIfNotRegistered(registration, serviceBindingConfiguration);
                    }

                        break;
                    default:
                        throw new Exception($"Unhandled value '{implementationConfiguration.TargetImplementationType}'.");
                }
        }

        public void OnDiContainerReady(IDiContainer diContainer)
        {
            _diContainer = diContainer;
            _parameterSerializer = _diContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
        }

        private void SetInstanceActivatedAction<TLimit, TActivatorData, TRegistrationStyle>(
        IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration, Action<IDiContainer, object> implementationActivatedAction)
        {
            if (implementationActivatedAction != null)
                registration.OnActivated(e => { implementationActivatedAction(_diContainer, e.Instance); });
        }

        private void SetRegisterIfNotRegistered<TLimit, TActivatorData, TRegistrationStyle>(
        IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration, BindingConfigurationForCode bindingConfiguration)
        {
            if (bindingConfiguration.RegisterIfNotRegistered)
                registration.IfNotRegistered(bindingConfiguration.ServiceType);
        }

        private void SetResolutionScope<TLimit, TActivatorData, TRegistrationStyle>(
        IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration, DiResolutionScope? resolutionScope)
        {
            if (resolutionScope == null)
                return;

            switch (resolutionScope.Value)
            {
                case DiResolutionScope.Singleton:
                    registration.SingleInstance();
                    break;

                case DiResolutionScope.Transient:
                    registration.InstancePerDependency();
                    break;

                case DiResolutionScope.ScopeLifetime:
                    registration.InstancePerLifetimeScope();
                    break;

                default:
                    throw new Exception($"Unsupported resolution type.The value is '{resolutionScope}'.");
            }
        }

        #endregion
    }
}