using System;
using System.Collections.Generic;
using System.Linq;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using Ninject.Modules;
using Ninject.Syntax;
using OROptimizer.Serializer;
using NinjectLib = Ninject;

namespace IoC.Configuration.Ninject
{
    public class NinjectModuleWrapper : NinjectModule
    {
        #region Member Variables

        private IDiContainer _diContainer;

        [NotNull]
        private readonly IDiModule _module;

        [NotNull]
        private static readonly Dictionary<Type, int> _moduleTypeToCounterMap = new Dictionary<Type, int>();

        private ITypeBasedSimpleSerializerAggregator _parameterSerializer;

        #endregion

        #region  Constructors

        public NinjectModuleWrapper([NotNull] IDiModule module)
        {
            {
                // Lets assign a unique name to module, since otherwize Ninject will complain in the Name is not unique.
                // The Name in IoC.Configuration will not necessarily be unique.

                var moduleType = module.GetType();

                var counter = 0;
                if (_moduleTypeToCounterMap.ContainsKey(moduleType))
                    counter = _moduleTypeToCounterMap[moduleType];

                Name = $"{module.GetType().FullName}-{counter}";
                _moduleTypeToCounterMap[moduleType] = ++counter;
            }

            _module = module;
            _module.Load();
        }

        #endregion

        #region Member Functions

        public override void Load()
        {
            foreach (var serviceBindingConfiguration in _module.ServiceBindingConfigurations)
            {
                if (serviceBindingConfiguration.RegisterIfNotRegistered && Kernel.GetBindings(serviceBindingConfiguration.ServiceType).Any())
                    continue;

                foreach (var implementationConfiguration in serviceBindingConfiguration.Implementations)
                {
                    var ninjectServiceBindingConfiguration = Bind(serviceBindingConfiguration.ServiceType);
                    IBindingWhenInNamedWithOrOnSyntax<object> ninjectImplementationConfiguration;

                    switch (implementationConfiguration.TargetImplementationType)
                    {
                        case TargetImplementationType.Type:
                            ninjectImplementationConfiguration = ninjectServiceBindingConfiguration.To(implementationConfiguration.ImplementationType);
                            break;
                        case TargetImplementationType.Self:
                            ninjectImplementationConfiguration = ninjectServiceBindingConfiguration.ToSelf();
                            break;

                        case TargetImplementationType.Delegate:
                            ninjectImplementationConfiguration = ninjectServiceBindingConfiguration.ToMethod(context =>
                                implementationConfiguration.ImplementationGeneratorFunction(_diContainer));
                            break;
                        default:
                            throw new Exception($"Unhandled value '{implementationConfiguration.TargetImplementationType}'.");
                    }

                    if (implementationConfiguration.WhenInjectedIntoType != null)
                        switch (implementationConfiguration.ConditionalInjectionType)
                        {
                            case ConditionalInjectionType.WhenInjectedInto:
                                ninjectImplementationConfiguration.WhenInjectedInto(implementationConfiguration.WhenInjectedIntoType);
                                break;

                            case ConditionalInjectionType.WhenInjectedExactlyInto:
                                ninjectImplementationConfiguration.WhenInjectedExactlyInto(implementationConfiguration.WhenInjectedIntoType);
                                break;
                        }

                    switch (implementationConfiguration.ResolutionScope)
                    {
                        case DiResolutionScope.Singleton:
                            ninjectImplementationConfiguration.InSingletonScope();
                            break;

                        case DiResolutionScope.Transient:
                            ninjectImplementationConfiguration.InTransientScope();
                            break;

                        case DiResolutionScope.ScopeLifetime:
                            ninjectImplementationConfiguration.InScope(context => _diContainer.CurrentLifeTimeScope);
                            break;

                        //case DiResolutionScope.Thread:
                        //    ninjectImplementationConfiguration.InThreadScope();
                        //    break;

                        default:
                            throw new Exception($"Unsupported resolution type.The value is '{implementationConfiguration.ResolutionScope}'.");
                    }

                    if (implementationConfiguration.OnImplementationObjectActivated != null)
                        ninjectImplementationConfiguration.OnActivation(activatedObject =>
                            implementationConfiguration.OnImplementationObjectActivated(_diContainer, activatedObject));
                }
            }
        }

        [NotNull]
        public override string Name { get; }

        public void OnDiContainerReady(IDiContainer diContainer)
        {
            _diContainer = diContainer;
            _parameterSerializer = _diContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
        }

        #endregion
    }
}