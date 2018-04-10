using Autofac;
using IoC.Configuration.DiContainer;
using OROptimizer.Serializer;
using TestPluginAssembly1.Implementations;
using TestPluginAssembly1.Interfaces;

namespace DynamicImplementations_636583977886378414.Plugin1
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
            builder.Register(context => new Door(_parameterSerializer.Deserialize<int>("3"), _parameterSerializer.Deserialize<double>("180"))).As<IDoor>().InstancePerDependency();
            builder.Register(context => new Room(_parameterSerializer.Deserialize<IDoor>("5,185.1"), context.Resolve<IDoor>())).As<IRoom>().InstancePerDependency().OnActivated(e => { e.Instance.Door2 = _parameterSerializer.Deserialize<IDoor>("7,187.3"); });
        }

        public void OnDiContainerReady(IDiContainer diContainer)
        {
            _diContainer = diContainer;
            _parameterSerializer = _diContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
        }

        #endregion
    }
}