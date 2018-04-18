using Autofac;
namespace DynamicImplementations_636595161441212446.Plugin1
{
public class PluginServicesModule : Autofac.Module
{
private IoC.Configuration.DiContainer.IDiContainer _diContainer;
private OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator _parameterSerializer;
public void OnDiContainerReady(IoC.Configuration.DiContainer.IDiContainer diContainer)
{
_diContainer=diContainer;
_parameterSerializer = _diContainer.Resolve<OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator>();
}
protected override void Load(Autofac.ContainerBuilder builder)
{
base.Load(builder);
builder.Register(context => new TestPluginAssembly1.Implementations.Door(_parameterSerializer.Deserialize<System.Int32>("3"), _parameterSerializer.Deserialize<System.Double>("180"))).As<TestPluginAssembly1.Interfaces.IDoor>().InstancePerDependency();
builder.Register(context => new TestPluginAssembly1.Implementations.Room(_parameterSerializer.Deserialize<TestPluginAssembly1.Interfaces.IDoor>("5,185.1"), context.Resolve<TestPluginAssembly1.Interfaces.IDoor>())).As<TestPluginAssembly1.Interfaces.IRoom>().InstancePerDependency().OnActivated(e =>
{
e.Instance.Door2=_parameterSerializer.Deserialize<TestPluginAssembly1.Interfaces.IDoor>("7,187.3");
});
}
}
}
