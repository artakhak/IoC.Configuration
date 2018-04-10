using Autofac;
using DynamicallyLoadedAssembly1.Implementations;
using IoC.Configuration;
using IoC.Configuration.DiContainer;
using IoC.Configuration.OnApplicationStart;
using OROptimizer.Serializer;
using SharedServices;

namespace DynamicImplementations_636583977886378414
{
    public class AdditionalServices : Module
    {
        #region Member Variables

        private IDiContainer _diContainer;
        private ITypeBasedSimpleSerializerAggregator _parameterSerializer;

        #endregion

        #region Member Functions

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<FakeSettingsRequestor>().As<ISettingsRequestor>().SingleInstance();
            builder.RegisterType<StartupAction1>().As<IStartupAction>().SingleInstance();
            builder.RegisterType<StartupAction2>().As<IStartupAction>().SingleInstance();
            builder.Register(context => new TestPluginAssembly1.Implementations.Plugin1(_parameterSerializer.Deserialize<long>("25"))).As<IPlugin>().SingleInstance();
            builder.Register(context => new TestPluginAssembly2.Implementations.Plugin2(_parameterSerializer.Deserialize<bool>("true"), _parameterSerializer.Deserialize<double>("25.3"), _parameterSerializer.Deserialize<string>("String value"))).As<IPlugin>().SingleInstance().OnActivated(e => { e.Instance.Property2 = _parameterSerializer.Deserialize<double>("5.3"); });
        }

        public void OnDiContainerReady(IDiContainer diContainer)
        {
            _diContainer = diContainer;
            _parameterSerializer = _diContainer.Resolve<ITypeBasedSimpleSerializerAggregator>();
        }

        #endregion
    }
}