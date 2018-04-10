using Autofac;
using IoC.Configuration.DiContainer;
using OROptimizer.Serializer;
using TestPluginAssembly2.Implementations;
using TestPluginAssembly2.Interfaces;

namespace DynamicImplementations_636583977886378414.Plugin2
{
    public class PluginServicesModule : Module
    {
        #region Member Variables

        private IDiContainer _diContainer;
        private ITypeBasedSimpleSerializerAggregator _parameterSerializer;

        #endregion

        #region Member Functions

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register(context => new Wheel(_parameterSerializer.Deserialize<int>("5"), _parameterSerializer.Deserialize<double>("48"))).As<IWheel>().InstancePerDependency();
            builder.Register(context => new Car(_parameterSerializer.Deserialize<IWheel>("248,40"))).As<ICar>().InstancePerDependency().OnActivated(e =>
            {
                e.Instance.Wheel1 = _parameterSerializer.Deserialize<IWheel>("27,45");
                e.Instance.Wheel2 = e.Context.Resolve<IWheel>();
            });
        }

        public void OnDiContainerReady(IDiContainer diContainer)
        {
            _diContainer = diContainer;
            _parameterSerializer = _diContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
        }

        #endregion
    }
}